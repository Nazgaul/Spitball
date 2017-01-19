using Autofac;
using Zbang.Zbox.Domain.CommandHandlers.Ioc;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CommandsModule : Module
    {
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateCourseTagCommandHandler>().As<ICommandHandler<CreateCourseTagCommand>>();
        }
    }
}
