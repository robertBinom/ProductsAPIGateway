using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing;
using Services.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace ProductCatalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : BaseController
    {
        private readonly ITracer tracer;

        public ProductsController(ITracer tracer) : base(tracer) { }


        private static readonly string[] Products = new[]
        {
            "Samsung Galaxy A30", "Samsung Galaxy A50", "Samsung Galaxy A70", "Huawei P30", "Huawei P40", 
        };

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Products;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return Products.ElementAtOrDefault(id) ?? "There is no such product";
        }
    }
}
