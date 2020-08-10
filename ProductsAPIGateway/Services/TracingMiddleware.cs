using Microsoft.AspNetCore.Http;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using System.IO;
using System.Text;

namespace Services
{
    public class TracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITracer _tracer;
        private readonly IDiagnosticContext _diagnosticContext;

        public TracingMiddleware(RequestDelegate next, ITracer tracer, IDiagnosticContext diagnosticContext)
        {
            _next = next;
            _tracer = tracer;
            _diagnosticContext = diagnosticContext;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var scope = BuildServerScope(context))
            {
                await LogRequest(scope, context);
                SetupTraceHeaders(scope, context);
                await LogResponse(scope, context);
            }
        }



        #region => Setup Trace Headers
        private void SetupTraceHeaders(IScope scope, HttpContext context)
        {
            if (scope == null || context == null)
                return;
            
            scope.Span.SetTag(Tags.Component, "Tracing.Middleware.Action");
            scope.Span.SetTag(Tags.HttpMethod, context.Request.Method);

            // Setup B3 headers
            var traceHeaders = new Dictionary<string, string>();
            _tracer.Inject(scope.Span.Context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(traceHeaders));
            context.Items.Add("OpenTraceHeaders", traceHeaders);

            // Set diagnostics context for LoggingFactory
            _diagnosticContext.Set("TraceId", scope.Span.Context.TraceId);
            _diagnosticContext.Set("SpanId", scope.Span.Context.SpanId);

            // Set B3 Response headers prior to the _next delegate 
            // Response will already be buffering
            foreach (var pair in traceHeaders)
            {
                context.Response.Headers.Add(pair.Key, pair.Value);
            }
        }
        #endregion

        #region => Build Server Scope
        private IScope BuildServerScope(HttpContext context)
        {
            ISpanBuilder spanBuilder = null;

            var operationName = GetOperationName(context);

            var requestTraceHeaders = context.GetTraceHeaders();
            if (requestTraceHeaders?.Any() ?? false)
            {
                try
                {
                    spanBuilder = _tracer.BuildSpan(operationName);
                    var parentSpanContext = _tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(requestTraceHeaders));
                    if (parentSpanContext != null)
                    {
                        spanBuilder = spanBuilder.AsChildOf(parentSpanContext);
                    }
                }
                catch { }
            }

            if (spanBuilder == null)
            {
                spanBuilder = _tracer.BuildSpan(operationName);
            }

            return spanBuilder.WithTag(Tags.SpanKind, Tags.SpanKindServer).StartActive(true);
        }
        #endregion

        #region => GetOperationName
        private string GetOperationName(HttpContext context)
        {
            string operationName = null;
            var routeData = context?.GetRouteData()?.Values;

            if (routeData != null)
            {
                if (routeData.TryGetValue("url", out object urlValue))
                {
                    operationName = urlValue as string;
                }
                else if (routeData.TryGetValue("controller", out object controllerValue) && routeData.TryGetValue("action", out object actionValue))
                {
                    operationName = $"{controllerValue}.{actionValue}";
                }
            }

            return operationName ?? String.Concat("HTTP ", context.Request.Method);
        }
        #endregion

        #region => LogRequest
        private async Task LogRequest(IScope scope, HttpContext context)
        {
            if (scope == null || context == null)
                return;

            var requestLogData = new Dictionary<string, object>
            {
                { "Request.Method", context.Request.Method },
                { "Request.Uri", $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}" }
            };

            if (!context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                if (context.Request.HasFormContentType)
                {
                    var formValues = context.Request.Form?.Select(xx => new KeyValuePair<string, string>(xx.Key, xx.Value))?.ToList();
                    if (formValues.Count > 0)
                    {
                        formValues.ForEach(xx => requestLogData.Add($"Request.Form.{xx.Key}", xx.Value));
                    }
                }
                else
                {
                    var requestBody = await GetRequestBody(context.Request);
                    if (!string.IsNullOrEmpty(requestBody))
                    {
                        requestLogData.Add("Request.Body", requestBody);
                    }
                }
            }

            scope.Span.Log(requestLogData);
        }
        #endregion

        #region => GetRequestBody
        private async Task<string> GetRequestBody(HttpRequest request)
        {
            if (request == null)
                return null;

            var body = "";
            request.EnableBuffering(1024 * 100);
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }
            return body;
        }
        #endregion

        #region => LogResponse
        private async Task LogResponse(IScope scope, HttpContext context)
        {
            if (scope == null || context == null)
                return;

            var responseBody = await GetResponseBody(context);

            var data = new Dictionary<string, object> { };

            data.Add("Response.StatusCode", context.Response.StatusCode);

            if (!string.IsNullOrEmpty(responseBody))
            {
                data.Add("Response.Body", responseBody);
            }

            scope.Span.Log(data);
        }
        #endregion

        #region => GetResponseBody
        private async Task<string> GetResponseBody(HttpContext context)
        {
            if (context?.Response == null)
                return null;

            var body = "";
            // Store the original stream reference
            var originalResponseBody = context.Response.Body;

            using (var responseStream = new MemoryStream())
            {
                context.Response.Body = responseStream;

                // Execute Delegate (intercepted)
                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                body = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Restore Response body from stream
                await responseStream.CopyToAsync(originalResponseBody);
            }

            return body;
        }
        #endregion
    }
}
