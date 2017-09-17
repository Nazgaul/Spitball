using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.AI;

namespace Cloudents.Infrastructure
{
    public class InfrastructureModule :  Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LuisAI>().As<IAI>();
            builder.RegisterType<AIDecision>().As<IDesicions>();
        }
    }
}
