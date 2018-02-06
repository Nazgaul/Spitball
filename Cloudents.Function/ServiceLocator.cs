using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Cloudents.Infrastructure.Framework;
using CommonServiceLocator;

namespace Cloudents.Function
{
    public class ServiceLocator
    {
        public ServiceLocator()
        {
            this.BuildContainer();
        }

        public IServiceLocator Instance { get; private set; }

        private void BuildContainer()
        {
            var builder = new ContainerBuilder();
            var sql = GetEnvironmentVariable("sql");
            //builder.RegisterModule(new ModuleDb(ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString));

            var container = builder.Build();

            // Create service locator.
           // var csl = new AutofacServiceLocator(container);


            // Set the service locator created.

           // CommonServiceLocator.ServiceLocator.SetLocatorProvider(() => csl);
           // this.Instance = CommonServiceLocator.ServiceLocator.Current;
        }

        private static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                   System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
