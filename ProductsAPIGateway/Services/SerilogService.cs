using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class SerilogService
    {
        public static void SetupSerilog()
        {
            string userName = Environment.UserName; // TODO : Choose logging destination
            string folder = @$"C:\Users\{userName}\Desktop\Serilog-Logging-Test\"; // ----------- Logging to desktop .txt file - FOR TESTING PURPOSES ONLY !!! -----------
            string file = $"Serilog-Logging-Test-{DateTime.Now.ToString("dd-MM-yyyy")}.txt";

            string path = String.Concat(folder, file);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.File(path)
                .CreateLogger();
        }

    }
}
