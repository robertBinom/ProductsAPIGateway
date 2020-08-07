using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Common
{
    public class BaseController : ControllerBase
    {
        [HttpGet]
        [Route("echo/{attributeString}")]
        public string Echo(string attributeString)
        {
            return $"This is working echo message from {this.GetType().Name}: {attributeString}";
        }

        [HttpGet]
        [Route("authecho/{attributeString}")]
        public string AuthEcho(string attributeString) 
        {
            return $"This is working AUTHENTICATED echo message from {this.GetType().Name}: {attributeString}";
        }
    }
}
