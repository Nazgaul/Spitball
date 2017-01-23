using Autofac;
using Autofac.Features.Variance;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CommandsModule : Module
    {
        
        protected override void Load(ContainerBuilder builder)
        {
            #region Jared
            /*builder.RegisterAssemblyTypes(myAssembly)
    .AsClosedTypesOf(typeof(ICommandHandler<>));*/
            //builder.RegisterSource(new ContravariantRegistrationSource());
           // builder.RegisterType<CreateCourseTagCommandHandler>().As<ICommandHandler<CreateCourseTagCommand>>();
            //builder.RegisterType<UpdateItemCourseTagCommandHandler>().As<ICommandHandler<UpdateItemCourseTagCommand>>();
            //builder.RegisterType<AssignTagsToItemCommandHandler>().As<ICommandHandler<AssignTagsToItemCommand>>();
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(ICommandHandler<>));

            #endregion
        }
    }
}
