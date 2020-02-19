using Microsoft.AspNetCore.Hosting;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Cloudents.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(x => x.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                });

        //.ConfigureKestrel(f => f.AddServerHeader = false)
        // .UseStartup<Startup>();

        //.ConfigureLogging((hostingContext, logging) =>
        //{
        //    //if (hostingContext.HostingEnvironment.IsDevelopment())
        //    //{
        //    //    logging.AddLog4Net();
        //    //}
        //});
    }
}
