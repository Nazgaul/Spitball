using System.Linq;
using Autofac;
using Autofac.Core;
using Cloudents.Command;
using Cloudents.Command.CommandHandler;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure.Stuff
{
    [UsedImplicitly]
    public class ModuleCore : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterType<CommandBus>().As<ICommandBus>();//.InstancePerLifetimeScope();


            builder.RegisterAssemblyTypes(typeof(IQueryHandler<,>).Assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterType<QueryBus>().As<IQueryBus>();//.InstancePerLifetimeScope();

          //  builder.RegisterType<UrlRedirectBuilder>().As<IUrlRedirectBuilder>();
            builder.RegisterType<UrlConst>().As<IUrlBuilder>().SingleInstance();
            //builder.RegisterType<Shuffle>().As<IShuffle>();

            //builder.RegisterType<WebSearch>().As<IWebDocumentSearch>().WithParameter("api", CustomApiKey.Documents);
            //builder.RegisterType<WebSearch>().As<IWebFlashcardSearch>().WithParameter("api", CustomApiKey.Flashcard);

            //builder.RegisterType<DbConnectionStringProvider>().AsSelf();

            //builder.RegisterAssemblyTypes(typeof(ICommandHandler<>).Assembly).AsClosedTypesOf(typeof(ICommandHandler<>));

            builder.RegisterAssemblyTypes(typeof(ICommandHandler<>).Assembly).As(o => o.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(ICommandHandler<>)))
                .Select(i => new KeyedService("handler", i)));


            builder.RegisterGenericDecorator(
                typeof(CommitUnitOfWorkCommandHandlerDecorator<>),
                typeof(ICommandHandler<>),
                fromKey: "handler");

            builder.RegisterAssemblyTypes(typeof(IEventHandler<>).Assembly).AsClosedTypesOf(typeof(IEventHandler<>));

            builder.RegisterType<EventPublisher>().As<IEventPublisher>();
           // builder.RegisterType<EventStore>().As<IEventStore>().InstancePerLifetimeScope();
            builder.RegisterType<Logger>().As<ILogger>();

        }
    }
}
