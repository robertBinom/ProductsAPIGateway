using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProductCatalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
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
            return Products[id] ?? "There is no such product";
        }
    }
}
