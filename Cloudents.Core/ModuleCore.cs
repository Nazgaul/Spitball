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
    [ModuleRegistration(Enum.System.Console)]
    [ModuleRegistration(Enum.System.Function)]
    [ModuleRegistration(Enum.System.Web)]
    [UsedImplicitly]
    public class ModuleCore : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            //builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ICommandHandler<,>)).AsImplementedInterfaces();
            //builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ICommandHandlerAsync<>));//.As(typeof(ICommandHandlerAsync<>));//.AsImplementedInterfaces();
            builder.RegisterType<CommandBus>().As<ICommandBus>();

            //builder.RegisterAssemblyTypes(assembly).As(o => o.GetInterfaces()
            //    .Where(i => i.IsClosedTypeOf(typeof(ICommandHandler<,>)))
            //    .Select(i => new KeyedService("handler", i)));

            builder.RegisterAssemblyTypes(assembly).As(o => o.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(ICommandHandler<>)))
                .Select(i => new KeyedService("handler", i)));

            builder.RegisterGenericDecorator(
                typeof(CommitUnitOfWorkCommandHandlerDecorator<>),
                typeof(ICommandHandler<>),
                fromKey: "handler");
            //builder.RegisterGenericDecorator(
            //    typeof(CommitUnitOfWorkCommandHandlerDecorator<,>),
            //    typeof(ICommandHandler<,>),
            //    fromKey: "handler");

            builder.RegisterType<UpdateMailGunCommandHandler>()
                .Named<ICommandHandler<UpdateMailGunCommand>>("mailGun");

            //TODO: fix that
            //builder.RegisterDecorator<ICommandHandler<UpdateMailGunCommand>>(
            //    (c, inner) =>
            //    {
            //        var t = c.ResolveKeyed<IUnitOfWork>(Database.MailGun);
            //        return new CommitUnitOfWorkCommandHandlerDecorator<UpdateMailGunCommand>(t, inner);
            //    },
            //    fromKey: "mailGun");

            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            //builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IQueryHandlerAsync<>));
            builder.RegisterType<QueryBus>().As<IQueryBus>();

            builder.RegisterType<UrlConst>().As<IUrlBuilder>().SingleInstance();

            builder.RegisterType<UrlRedirectBuilder>().As<IUrlRedirectBuilder>();
            builder.RegisterType<Shuffle>().As<IShuffle>();

            //builder.RegisterType<WebSearch>();

            builder.RegisterType<WebSearch>().As<IWebDocumentSearch>().WithParameter("api", CustomApiKey.Documents);
            builder.RegisterType<WebSearch>().As<IWebFlashcardSearch>().WithParameter("api", CustomApiKey.Flashcard);
        }
    }
}
