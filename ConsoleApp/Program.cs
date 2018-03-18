using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Read;
using Cloudents.Core.Request;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Search.Tutor;

namespace ConsoleApp
{
    static class Program
    {
        static async Task Main()
        {
            Uri address1 = new Uri("http://api.www.contoso.com/index.htm#search");
            Console.WriteLine("address 1 {0} a valid scheme name",
                Uri.CheckSchemeName(address1.Scheme) ? " has" : " does not have");

            if (address1.Scheme == Uri.UriSchemeHttp)
                Console.WriteLine("Uri is HTTP type");

            Console.WriteLine(address1.HostNameType);

            var builder = new ContainerBuilder();


            //var infrastructureModule = new InfrastructureModule(
            //    ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
            //    ConfigurationManager.AppSettings["AzureSearchServiceName"],
            //    ConfigurationManager.AppSettings["AzureSearchKey"],
            //    ConfigurationManager.AppSettings["Redis"],
            //    ConfigurationManager.AppSettings["StorageConnectionString"]);

            //  builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();

            var keys = new ConfigurationKeys
            {
                Db = ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"]),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"]
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterSystemModules(
                Cloudents.Core.Enum.System.Console,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"));
            //builder.RegisterType<TutorMeSearch>().AsSelf();
            var container = builder.Build();

            var resolve1 = container.Resolve<WebSearch.Factory>();
            var t = resolve1.Invoke(CustomApiKey.Documents);
            var result = await t.SearchAsync(SearchQuery.Ask(new[] {"war"}, 0, null), HighlightTextFormat.None, default);

            var resolve2 = container
                .Resolve<IReadRepositoryAsync<(IEnumerable<CourseSearchWriteDto> update, IEnumerable<SearchWriteBaseDto>
                    delete, long version), SyncAzureQuery>>();

            var t2 = await resolve2.GetAsync(new SyncAzureQuery(0, 0), default);

            Console.WriteLine("Finish");
            Console.ReadLine();
        }
    }
}
