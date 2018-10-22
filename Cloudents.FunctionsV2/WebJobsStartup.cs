using Cloudents.FunctionsV2;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(WebJobsStartup.Startup))]
namespace Cloudents.FunctionsV2
{
    public class WebJobsStartup
    {
        internal class Startup : IWebJobsStartup
        {
            public void Configure(IWebJobsBuilder builder) =>

                builder.AddDependencyInjection<AutofacServiceProviderBuilder>();

            private void ConfigureServices(IServiceCollection services)
            {
                //services.AddTransient<ITransientGreeter, Greeter>();
                //services.AddScoped<IScopedGreeter, Greeter>();
                //services.AddSingleton<ISingletonGreeter, Greeter>();
            }
        }
    }
}