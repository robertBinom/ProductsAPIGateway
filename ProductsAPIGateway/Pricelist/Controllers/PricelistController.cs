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

        [HttpGet] // TODO: Prebaciti ovo u neki base controller tako da svi controlleri imaju po defaultu echo
        [Route("echo/{attributeString}")]
        public string Echo(string attributeString)
        {
            return $"This is working echo message from {this.GetType().Name}: {attributeString}";
        }


        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Pricelist;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return Pricelist.ElementAtOrDefault(id) ?? "There is no such price";
        }
    }
}
