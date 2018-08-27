using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Cloudents.Ico
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(p => p.AddServerHeader = false)
                .UseStartup<Startup>()
                .Build();
    }
}
