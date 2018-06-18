using System.Reflection;
using Autofac;
using Cloudents.Core.Attributes;
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
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ICommandHandlerAsync<,>)).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ICommandHandlerAsync<>)).AsImplementedInterfaces();
            builder.RegisterType<CommandBus>().As<ICommandBus>();

            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IQueryHandlerAsync<,>));
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IQueryHandlerAsync<>));
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
