using Autofac;
using Cloudents.Application.Attributes;
using Cloudents.Application.Interfaces;

namespace Cloudents.Infrastructure.Blockchain
{
    [ModuleRegistration(Application.Enum.System.Console)]
    [ModuleRegistration(Application.Enum.System.Web)]
    [ModuleRegistration(Application.Enum.System.Function)]
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
