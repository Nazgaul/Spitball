using Autofac;
using Autofac.Core;
using Cloudents.Command;
using Cloudents.Command.CommandHandler;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using System.Linq;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure.Stuff
{
    public class ModuleCore : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CommandBus>().As<ICommandBus>();


            // builder.RegisterAssemblyTypes(typeof(IQueryHandler<,>).Assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterType<QueryBus>().As<IQueryBus>();

            builder.RegisterType<UrlConst>().As<IUrlBuilder>().SingleInstance();

            builder.RegisterAssemblyTypes(typeof(ICommandHandler<>).Assembly).As(o => o.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(ICommandHandler<>)))
                .Select(i => new KeyedService("handler", i)));


            builder.RegisterAssemblyTypes(typeof(IQueryHandler<,>).Assembly).As(o => o.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(IQueryHandler<,>)))
                .Select(i => new KeyedService("handler", i)));

            builder.RegisterGenericDecorator(
                typeof(CacheQueryHandlerDecorator<,>),
                typeof(IQueryHandler<,>),
                fromKey: "handler");

            builder.RegisterGenericDecorator(
                typeof(CommitUnitOfWorkCommandHandlerDecorator<>),
                typeof(ICommandHandler<>),
                fromKey: "handler");

            builder.RegisterAssemblyTypes(typeof(IEventHandler<>).Assembly).AsClosedTypesOf(typeof(IEventHandler<>));

            builder.RegisterType<EventPublisher>().As<IEventPublisher>();
            builder.RegisterType<Logger>().As<ILogger>();

        }
    }
}
