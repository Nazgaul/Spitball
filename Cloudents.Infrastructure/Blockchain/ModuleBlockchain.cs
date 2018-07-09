using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Blockchain;

namespace Cloudents.Infrastructure.BlockChain
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [ModuleRegistration(Core.Enum.System.Function)]
    public class ModuleBlockChain : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Erc20Service>().As<IBlockChainErc20Service>();
            builder.RegisterType<QAndAService>().As<IBlockChainQAndAContract>();
            //builder.RegisterType<CrowdSaleService>().As<IBlockChainCrowdSaleService>();
        }
    }
}
