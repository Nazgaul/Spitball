using System;
using System.Configuration;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.WebJob
{
    public class MyActivator : IJobActivator
    {
        private readonly IContainer _container;

        public MyActivator()
        {
            var builder = new ContainerBuilder();
            var keys = new ConfigurationKeys
            {
                Db = ConfigurationManager.ConnectionStrings["Zbox"].ConnectionString
            };
            builder.RegisterType<Functions>()
                .InstancePerDependency();

            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterModule<ModuleDb>();

            _container = builder.Build();
        }

        public T CreateInstance<T>()
        {
            return _container.Resolve<T>();
        }
    }
}