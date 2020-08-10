using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly ITracer _tracer;

        public BaseController(ITracer tracer)
        {
            _tracer = tracer;
        }

        #region ExecuteAPIGateway

        #endregion

        [HttpGet]
        [Route("echo/{attributeString}")]
        public string Echo(string attributeString)
        {
            return $"This is working echo message: {attributeString}";
        }

        [HttpGet]
        [Route("authecho/{attributeString}")]
        public string AuthEcho(string attributeString)
        {
            return $"This is working AUTHENTICATED echo message: {attributeString}";
        }
    }

}
