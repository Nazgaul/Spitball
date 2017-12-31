using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Request;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Test
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            var infrastructureModule = new InfrastructureModule(
                ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                ConfigurationManager.AppSettings["AzureSearchServiceName"],
                ConfigurationManager.AppSettings["AzureSearchKey"],
                ConfigurationManager.AppSettings["Redis"],
                ConfigurationManager.AppSettings["StorageConnectionString"]);

          //  builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
        builder.RegisterModule(infrastructureModule);
            var container = builder.Build();

            var repository = container.Resolve<IEngineProcess>();

            var result = await repository.ProcessRequestAsync("suburbs", default);
            //var result  = await repository.FetchBlobMetaDataAsync(new Uri(
            //    "https://zboxstorage.blob.core.windows.net/zboxfiles/b6a4938b-8dd8-4df7-bcdf-4454a80e31d1.pdf"),default);
           


            var location = new GeoPoint()
            {
                Latitude = 31.889692,
                Longitude = 34.812241
            };

            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConfigurationManager.ConnectionStrings["Zbox"].ConnectionString).Options;





            using (var db = new AppDbContext(options))
            {
                var services = new EfRepository<Course>(db);
                var t = await services.AddAsync(new Course("ram2", 170460), default);

                
            }
           
            Console.ReadLine();
        }
    }
}
