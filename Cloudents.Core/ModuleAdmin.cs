using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Cloudents.Core
{
    [ModuleRegistration(Enum.System.Admin, Order = 2)]
    [ModuleRegistration(Enum.System.Function, Order = 2)]
    //[ModuleRegistration(Enum.System.Console, Order = 2)]
    [UsedImplicitly]
    public class ModuleAdmin : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).As(o => o.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(ICommandHandler<>)) && i.GetCustomAttribute<AdminCommandHandler>() != null)
                .Select(i => new KeyedService("handler", i)));

            builder.RegisterType<CommandHandler.MarkAnswerAsCorrectCommandHandler>().As<ICommandHandler<Command.MarkAnswerAsCorrectCommand>>();
        }
    }
}