using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Command;
using Cloudents.Core.CommandHandler;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Cloudents.Core
{
    [ModuleRegistration(Enum.System.Console, Order = 1)]
    [ModuleRegistration(Enum.System.Function)]
    [ModuleRegistration(Enum.System.Web)]
    [ModuleRegistration(Enum.System.Admin, Order = 1)]
    [UsedImplicitly]
    public class ModuleCore : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterType<CommandBus>().As<ICommandBus>();



            builder.RegisterType<UpdateMailGunCommandHandler>()
                .Named<ICommandHandler<UpdateMailGunCommand>>("mailGun");

            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterType<QueryBus>().As<IQueryBus>();

            builder.RegisterType<UrlRedirectBuilder>().As<IUrlRedirectBuilder>();
            builder.RegisterType<UrlConst>().As<IUrlBuilder>().SingleInstance();
            builder.RegisterType<Shuffle>().As<IShuffle>();

            builder.RegisterType<WebSearch>().As<IWebDocumentSearch>().WithParameter("api", CustomApiKey.Documents);
            builder.RegisterType<WebSearch>().As<IWebFlashcardSearch>().WithParameter("api", CustomApiKey.Flashcard);

            builder.RegisterType<DbConnectionStringProvider>().AsSelf();

            builder.RegisterAssemblyTypes(assembly).As(o => o.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(ICommandHandler<>)) && i.GetCustomAttribute<AdminCommandHandler>() == null)
                .Select(i => new KeyedService("handler", i)));

            builder.RegisterGenericDecorator(
                typeof(CommitUnitOfWorkCommandHandlerDecorator<>),
                typeof(ICommandHandler<>),
                fromKey: "handler");
        }
    }
}
