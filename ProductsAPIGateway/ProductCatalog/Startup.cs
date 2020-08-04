using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Contrib.NetCore.CoreFx;
using OpenTracing.Util;
using Serilog;
using Services;

namespace ProductCatalog
{
    public class Startup
    {
        private IConfiguration configuration { get; }
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();


            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = Assembly.GetEntryAssembly().GetName().Name;

                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                ISampler sampler = new ConstSampler(sample: true);

                ITracer tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .Build();

                GlobalTracer.Register(tracer);

                

                return tracer;
            });

            services.AddOpenTracing();

            //// Adds the Jaeger Tracer.
            //services.AddSingleton(serviceProvider =>
            //{
            //    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //    Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", "ProductCat");
            //    //Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", "localhost");
            //    //Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", "6831"); 
            //    //Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "const");
            //    //Environment.SetEnvironmentVariable("JAEGER_TRACEID_128BIT", "1");
            //    //Environment.SetEnvironmentVariable("JAEGER_PROPAGATION", "b3");


            //    var config = Jaeger.Configuration.FromEnv(loggerFactory);
            //    var tracer = config.GetTracer();

            //    OpenTracing.Util.GlobalTracer.Register(tracer);

            //    return tracer;
            //});

            ////services.AddOpenTracing();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();


            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
