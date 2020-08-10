using Microsoft.AspNetCore.Mvc;
using Services.Controllers;
using OpenTracing;


namespace APIGateway.Controllers
{
    // E-Commerce Service Controller - Main Controller on API Gateway

    [Route("api/[controller]")]
    [ApiController]
    public class ECServiceController : BaseController
    {
        public ECServiceController(ITracer tracer) : base(tracer)
        {

        }
    }
}
