using Autofac;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Blockchain
{
    public class ModuleBlockChain : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Erc20Service>().As<IBlockChainErc20Service>();
            builder.RegisterType<QAndAService>().As<IBlockChainQAndAContract>();
        }
    }
}
