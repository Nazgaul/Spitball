using Autofac;
using Autofac.Features.ResolveAnything;
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

           
            builder.RegisterType(typeof(AddLanguageToItemCommandHandler<Item>))
                .As<ICommandHandler<AddLanguageToDocumentCommand>>();
            builder.RegisterType(typeof(AddLanguageToItemCommandHandler<FlashcardMeta>))
                .As<ICommandHandler<AddLanguageToFlashcardCommand>>();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(ICommandHandler<>));

            #endregion
        }
    }
}
