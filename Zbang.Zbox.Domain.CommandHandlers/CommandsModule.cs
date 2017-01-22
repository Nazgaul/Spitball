using Autofac;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CommandsModule : Module
    {
        
        protected override void Load(ContainerBuilder builder)
        {
            #region Jared
            builder.RegisterType<CreateCourseTagCommandHandler>().As<ICommandHandler<CreateCourseTagCommand>>();
            builder.RegisterType<UpdateItemCourseTagCommandHandler>().As<ICommandHandler<UpdateItemCourseTagCommand>>();
            builder.RegisterType<AssignTagsToItemCommandHandler>().As<ICommandHandler<AssignTagsToItemCommand>>();

            #endregion
        }
    }
}
