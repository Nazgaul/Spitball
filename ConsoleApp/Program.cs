// ReSharper disable All
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
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

            var t = container.Resolve<IWebDocumentSearch>();
            var result = await t.SearchWithUniversityAndCoursesAsync(SearchQuery.Ask(new[] {"war"}, 0, null), HighlightTextFormat.None, default);

            //var resolve2 = container
            //    .Resolve<IReadRepositoryAsync<(IEnumerable<CourseSearchWriteDto> update, IEnumerable<SearchWriteBaseDto>
            //        delete, long version), SyncAzureQuery>>();

            //var t2 = await resolve2.GetAsync(new SyncAzureQuery(0, 0), default);


            var subjectTopicList = GoogleSheets.GetData("1G5mztkX5w9_JcbR0tQCY9_OvlszsTzh2FXuZFecAosw", "Subjects!B2:C");
            var autocompleteWrite = container.Resolve<ISearchServiceWrite<AutoComplete>>();

            var keyGenerator = container.Resolve<IKeyGenerator>();

            await autocompleteWrite.CreateOrUpdateAsync(default);

            foreach (var batch in subjectTopicList.Where(w => !string.IsNullOrEmpty(w.Value)).Batch(100))
            {
                var autoCompleteBatch = batch.Select(s => new AutoComplete()
                {
                    Key = s.Value,
                    Id = keyGenerator.GenerateKey(s.Value.ToLowerInvariant()),
                    Vertical = Vertical.Job,
                    Value = s.Key,
                    Prefix = s.Value
                });
                await autocompleteWrite.UpdateDataAsync(autoCompleteBatch, default);
            }

            Console.WriteLine("Finish");
            Console.ReadLine();
        }
    }
}
