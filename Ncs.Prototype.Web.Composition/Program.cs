using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ncs.Prototype.Web.Composition
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddApplicationInsights("9c2eb2db-38fd-4818-ba90-d0e73becec9f"); 
                    logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
                    //logging.AddConsole();
                    logging.AddDebug();
                    //logging.AddEventSourceLogger();
                })
                .UseStartup<Startup>();
    }
}
