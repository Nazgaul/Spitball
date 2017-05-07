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
            builder.RegisterType(typeof(AssignTagsToItemCommandHandler<Comment>))
               .As<ICommandHandler<AssignTagsToFeedCommand>>();

            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<Item>))
            .As<ICommandHandler<RemoveTagsFromDocumentCommand>>();
            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<FlashcardMeta>))
                .As<ICommandHandler<RemoveTagsFromFlashcardCommand>>();
            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<Domain.Quiz>))
               .As<ICommandHandler<RemoveTagsFromQuizCommand>>();


            //builder.RegisterType(typeof(SetReviewedCommandHandler<Item>))
            //.As<ICommandHandler<SetReviewedDocumentCommand>>();
            //builder.RegisterType(typeof(SetReviewedCommandHandler<FlashcardMeta>))
            //    .As<ICommandHandler<SetReviewedFlashcardCommand>>();
            //builder.RegisterType(typeof(SetReviewedCommandHandler<Domain.Quiz>))
            //   .As<ICommandHandler<SetReviewedQuizCommand>>();

            builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();

            builder.RegisterType(typeof(AddFileToBoxCommandHandler)).Named <
                ICommandHandlerAsync<AddItemToBoxCommand, AddItemToBoxCommandResult>>(AddItemToBoxCommand.FileResolver);

            builder.RegisterType(typeof(CreateAcademicBoxCommandHandler) ).Named<
                ICommandHandlerAsync<CreateBoxCommand, CreateBoxCommandResult>>("CreateAcademicBoxCommand");
            #endregion
        }
    }
}
