using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Cloudents.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureKestrel(f=>f.AddServerHeader = false)
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //if (hostingContext.HostingEnvironment.IsDevelopment())
                    //{
                    //    logging.AddLog4Net();
                    //}
                });
    }
}
