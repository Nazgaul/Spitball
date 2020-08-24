using Cloudents.FunctionsV2;
using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.Di;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Cloudents.FunctionsV2
{


    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            //builder.AddAzureStorage();
            //builder.AddDurableTask();
            //builder.AddSendGrid();
            //builder.AddTwilioSms();
            //builder.AddServiceBus();
            //builder.AddTimers();
            //builder.AddSignalR();

            builder.AddExtension<HttpClientFactoryExtensionConfigProvider>();

            builder.Services.AddHttpClient();
            builder.Services.Configure<HttpClientFactoryOptions>(options => options.SuppressHandlerScope = true);
            builder.AddExtension<AzureSearchSyncProvider>();
            builder.AddExtension<TwilioExtensionConfigProvider>();
            builder.AddDependencyInjection<AutofacServiceProviderBuilder>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //services.AddTransient<ITransientGreeter, Greeter>();
            //services.AddScoped<IScopedGreeter, Greeter>();
            //services.AddSingleton<ISingletonGreeter, Greeter>();
        }
    }

}