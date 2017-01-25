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

           
            builder.RegisterType(typeof(AddLanguageToItemCommandHandler<Item>))
                .As<ICommandHandler<AddLanguageToDocumentCommand>>();
            builder.RegisterType(typeof(AddLanguageToItemCommandHandler<FlashcardMeta>))
                .As<ICommandHandler<AddLanguageToFlashcardCommand>>();
            builder.RegisterType(typeof(AddLanguageToItemCommandHandler<Domain.Quiz>))
                .As<ICommandHandler<AddLanguageToQuizCommand>>();

            builder.RegisterType(typeof(AssignTagsToItemCommandHandler<Item>))
               .As<ICommandHandler<AssignTagsToDocumentCommand>>();
            builder.RegisterType(typeof(AssignTagsToItemCommandHandler<FlashcardMeta>))
                .As<ICommandHandler<AssignTagsToFlashcardCommand>>();
            builder.RegisterType(typeof(AssignTagsToItemCommandHandler<Domain.Quiz>))
               .As<ICommandHandler<AssignTagsToQuizCommand>>();
            //AssignTagsToQuizCommand

            builder.RegisterType(typeof(UpdateItemCourseTagCommandHandler<Item>))
               .As<ICommandHandler<UpdateDocumentCourseTagCommand>>();
            builder.RegisterType(typeof(UpdateItemCourseTagCommandHandler<FlashcardMeta>))
                .As<ICommandHandler<UpdateFlashcardCourseTagCommand>>();
            builder.RegisterType(typeof(UpdateItemCourseTagCommandHandler<Domain.Quiz>))
                .As<ICommandHandler<UpdateQuizCourseTagCommand>>();

            //builder.RegisterType<AssignTagsToDocumentCommandHandler>()
            //    .As<ICommandHandler<AssignTagsToDocumentCommand>>();

            //builder.RegisterType<AssignTagsToFlashcardCommandHandler>()
            //    .As<ICommandHandler<AssignTagsToFlashcardCommand>>();

            //builder.RegisterType<AssignTagsToDocumentCommandHandler>()
            //    .As<ICommandHandler<AssignTagsToQuizCommand>>();

            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(ICommandHandler<>));

            #endregion
        }
    }
}
