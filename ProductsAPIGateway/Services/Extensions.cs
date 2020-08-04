using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public static class Extensions
    {
        #region HttpContext.GetTraceHeaders
        public static IDictionary<string, string> GetTraceHeaders(this HttpContext context, string prefix = "x-b3-")
        {
            if (context?.Request?.Headers?.Any() ?? false)
            {
                var headers = context.Request.Headers.Where(xx => xx.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
                if (headers.Any())
                {
                    var result = new Dictionary<string, string>();
                    foreach (var pair in headers)
                    {
                        result.Add(pair.Key, pair.Value);
                    }
                    return result;
                }
            }
            return null;
        }
        #endregion
    }
}
