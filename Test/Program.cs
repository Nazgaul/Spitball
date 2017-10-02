using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            var infrastructureModule = new InfrastructureModule(
                ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                ConfigurationManager.AppSettings["AzureSearchServiceName"],
                ConfigurationManager.AppSettings["AzureSearchKey"]);


            builder.RegisterModule(infrastructureModule);
            var container = builder.Build();


            var service = container.Resolve<ITutorSearch>();

            var result = await service.SearchAsync("math",Cloudents.Core.Enum.SearchRequestFilter.None, Cloudents.Core.Enum.SearchRequestSort.None,null, default).ConfigureAwait(false);

            var t = result.ToList();
            Console.WriteLine();
            
            Console.ReadLine();
        }
    }
}
