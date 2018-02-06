using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
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
            var keys = new ConfigurationKeys()
            {
                Db = GetEnvironmentVariable("sql")
            };
            builder.Register(c => keys).As<IConfigurationKeys>();
            builder.RegisterModule< ModuleDb>();

            var container = builder.Build();

           // Instance = container;
            CreateServiceLocator(container);
            // Create service locator.


            // Set the service locator created.


        }

        private void CreateServiceLocator(IContainer container)
        {
            //var csl = new AutofacServiceLocator(container);
            CommonServiceLocator.ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(null));
            Instance = CommonServiceLocator.ServiceLocator.Current;
        }

        private static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                   System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
