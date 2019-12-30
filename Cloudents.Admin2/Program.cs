using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;

namespace Cloudents.Admin2
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
                .UseStartup<Startup>();

        //.Build();
    }
}
