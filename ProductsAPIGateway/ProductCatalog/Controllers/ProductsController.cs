using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace ProductCatalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ITracer tracer;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(ITracer tracer, ILogger<ProductsController> logger)
        {
            this.tracer = tracer;
            this.logger = logger;
        }


        private static readonly string[] Products = new[]
        {
            "Samsung Galaxy A30", "Samsung Galaxy A50", "Samsung Galaxy A70", "Huawei P30", "Huawei P40", 
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
            return Products;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return Products.ElementAtOrDefault(id) ?? "There is no such product";
        }
    }
}
