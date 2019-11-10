using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;


[assembly: AssemblyVersion("1.0.9.0")]
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
