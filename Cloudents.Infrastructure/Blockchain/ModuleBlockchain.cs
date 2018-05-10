using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Infrastructure.Blockchain
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    public class ModuleBlockchain : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlockchainProvider>().As<IBlockchainProvider>();
        }
    }
}
