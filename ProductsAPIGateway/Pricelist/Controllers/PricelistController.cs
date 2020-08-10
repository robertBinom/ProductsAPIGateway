using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using Services.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace Pricelist.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricelistController : BaseController
    {
        private readonly ITracer tracer;

        public PricelistController(ITracer tracer) : base(tracer) { }

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
            return Pricelist.ElementAtOrDefault(id) ?? "There is no such price";
        }
    }
}
