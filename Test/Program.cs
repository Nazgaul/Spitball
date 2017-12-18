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
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Environment = Cloudents.Infrastructure.Environment;

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
                ConfigurationManager.AppSettings["Redis"], Environment.Console);

          //  builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
        builder.RegisterModule(infrastructureModule);
            var container = builder.Build();

            var repository = container.Resolve<IDocumentCseSearch>();

            var result = await repository.SearchAsync(
                new SearchQuery(new[] {"calculus", "class notes"}, 
                new[] { "University of Michigan","umich" },
                new[] {"ECON 101"}, null, 0,
                    SearchCseRequestSort.None)
                , default);


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
