using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.CommandHandler;
using Cloudents.Core.Attributes;
using JetBrains.Annotations;
using DeleteAnswerCommandHandler = Cloudents.Command.CommandHandler.Admin.DeleteAnswerCommandHandler;
using DeleteQuestionCommandHandler = Cloudents.Command.CommandHandler.Admin.DeleteQuestionCommandHandler;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure.Stuff
{
    [UsedImplicitly]
    public class ModuleAdmin : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).As(o => o.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(ICommandHandler<>)) && i.GetCustomAttribute<AdminCommandHandler>() != null)
                .Select(i => new KeyedService("handler", i)));

            builder.RegisterType<MarkAnswerAsCorrectCommandHandler>()
                .As<ICommandHandler<MarkAnswerAsCorrectCommand>>();


            builder.RegisterType<DeleteAnswerCommandHandler>().AsSelf();
                //.Named<ICommandHandler<Command.Admin.DeleteAnswerCommand>>("withoutDecorator");

            builder.RegisterType<DeleteQuestionCommandHandler>().AsSelf();
            //.Named<ICommandHandler<Command.Admin.DeleteQuestionCommand>>("withoutDecorator");
        }
    }
}