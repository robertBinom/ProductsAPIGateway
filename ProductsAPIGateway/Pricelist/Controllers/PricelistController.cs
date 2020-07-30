using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pricelist.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricelistController : ControllerBase
    {
        private static readonly string[] Pricelist = new[]
        {
            "1001", "1002", "1003", "1004", "1005", "1006", "1007", "1008", "1009", "1010"
        };

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Pricelist;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return Pricelist[id] ?? "There is no such price";
        }
    }
}
