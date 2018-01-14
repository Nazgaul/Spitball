using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.Metadata;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.DocumentDb;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;

namespace ConsoleApp
{
    class Program
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
            builder.RegisterModule<IocModule>();
            var container = builder.Build();
            //210ec431-2d6d-45cb-bc01-04e3f687f0ed.docx
            var meta = container.Resolve<IEnumerable<Meta<IPreviewProvider>>>();


            var repository = container.Resolve<IReadRepositoryAsync<DocumentDto, long>>();

            var result = await repository.GetAsync(566636, default);
            
            var process = meta.FirstOrDefault(f =>
            {
                if (f.Metadata[IocModule.ProcessorMeta] is string[] p)
                {
                    return p.Contains(".docx");
                }

                return false;
            });
            // var model = SearchQuery.Document(new [] {"microsoft"}, null, null, null, 0, SearchRequestSort.None, null);
        }
    }
}
