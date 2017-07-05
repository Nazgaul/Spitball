using Autofac;
using Zbang.Zbox.Domain.CommandHandlers.Quiz;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CommandsModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(AddLanguageToItemCommandHandler<Item>))
                .As<ICommandHandler<AddLanguageToDocumentCommand>>();
            builder.RegisterType(typeof(AddLanguageToItemCommandHandler<FlashcardMeta>))
                .As<ICommandHandler<AddLanguageToFlashcardCommand>>();
            builder.RegisterType(typeof(AddLanguageToItemCommandHandler<Domain.Quiz>))
                .As<ICommandHandler<AddLanguageToQuizCommand>>();

            builder.RegisterType(typeof(AssignTagsToItemCommandHandler<Item>))
               .As<ICommandHandlerAsync<AssignTagsToDocumentCommand>>();
            builder.RegisterType(typeof(AssignTagsToItemCommandHandler<FlashcardMeta>))
                .As<ICommandHandlerAsync<AssignTagsToFlashcardCommand>>();
            builder.RegisterType(typeof(AssignTagsToItemCommandHandler<Domain.Quiz>))
               .As<ICommandHandlerAsync<AssignTagsToQuizCommand>>();
            builder.RegisterType(typeof(AssignTagsToItemCommandHandler<Comment>))
               .As<ICommandHandlerAsync<AssignTagsToFeedCommand>>();

            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<Item>))
            .As<ICommandHandler<RemoveTagsFromDocumentCommand>>();
            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<FlashcardMeta>))
                .As<ICommandHandler<RemoveTagsFromFlashcardCommand>>();
            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<Domain.Quiz>))
               .As<ICommandHandler<RemoveTagsFromQuizCommand>>();



            builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();

            builder.RegisterType(typeof(AddFileToBoxCommandHandler)).Named <
                ICommandHandlerAsync<AddItemToBoxCommand, AddItemToBoxCommandResult>>(AddItemToBoxCommand.FileResolver);

            builder.RegisterType(typeof(CreateAcademicBoxCommandHandler) ).Named<
                ICommandHandlerAsync<CreateBoxCommand, CreateBoxCommandResult>>("CreateAcademicBoxCommand");



            builder
                .RegisterType
                <ICommandHandlerAsync<CreateUserCommand, CreateUserCommandResult>, CreateMembershipUserCommandHandler>(
                    CreateMembershipUserCommand.ResolveName);
            builder
                .RegisterType
                <ICommandHandlerAsync<CreateUserCommand, CreateUserCommandResult>, CreateFacebookUserCommandHandler>(
                    CreateFacebookUserCommand.ResolveName);

            builder
                .RegisterType
                <ICommandHandlerAsync<CreateUserCommand, CreateUserCommandResult>, CreateGoogleUserCommandHandler>(
                    CreateGoogleUserCommand.ResolveName);

            builder.RegisterType(typeof(ICommandHandlerAsync<UpdateUserPasswordCommand, UpdateUserCommandResult>),
                typeof(UpdateUserPasswordCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<UpdateUserEmailCommand>),
                typeof(UpdateUserEmailCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateUserProfileCommand>),
                typeof(UpdateUserProfileCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateUserProfileImageCommand>),
                typeof(UpdateUserProfileImageCommandHandler));
                builder.RegisterType<ICommandHandlerAsync<CreateBoxCommand, CreateBoxCommandResult>, CreateBoxCommandHandler>("CreateBoxCommand");
            builder.RegisterType(typeof(ICommandHandler<ChangeBoxInfoCommand>), typeof(ChangeBoxInfoCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<ChangeNotificationSettingsCommand>),
                typeof(ChangeNotificationSettingsCommandHandler));
                builder.RegisterType(typeof(ICommandHandler<RegisterMobileDeviceCommand>), typeof(UpdateUserMobileSettings));

            builder.RegisterType(typeof(ICommandHandlerAsync<UnFollowBoxCommand>), typeof(UnFollowBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteBoxCommand>), typeof(DeleteBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<ChangeBoxLibraryCommand>), typeof(ChangeBoxLibraryCommandHandler));

            builder
                .RegisterType
                <ICommandHandlerAsync<AddItemToBoxCommand, AddItemToBoxCommandResult>, AddLinkToBoxCommandHandler>(
                    AddItemToBoxCommand.LinkResolver);
            builder.RegisterType(typeof(ICommandHandler<ChangeFileNameCommand, ChangeFileNameCommandResult>), typeof(ChangeFileNameCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateThumbnailCommand>), typeof(UpdateThumbnailCommandHandler));

            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteItemCommand>), typeof(DeleteItemCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<SubscribeToSharedBoxCommand>), typeof(SubscribeToSharedBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteUserFromBoxCommand>), typeof(DeleteUserFromBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateUserLanguageCommand>), typeof(UpdateUserLanguageCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateUserEmailSubscribeCommand>), typeof(UpdateUserEmailSubscribeCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<UpdateNodeSettingsCommand>), typeof(UpdateNodeSettingsCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteNodeFromLibraryCommand>), typeof(DeleteNodeFromLibraryCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<UpdateUserUniversityCommand>), typeof(UpdateUserUniversityCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UnsubscribeUsersFromEmailCommand>), typeof(UnsubscribeUsersFromEmailCommandHandler));


            //annotation
            builder.RegisterType(typeof(ICommandHandlerAsync<AddItemCommentCommand>), typeof(AddItemCommentCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<AddItemReplyToCommentCommand>), typeof(AddItemReplyToCommentCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteItemCommentCommand>), typeof(DeleteItemCommentCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteItemCommentReplyCommand>), typeof(DeleteItemCommentReplyCommandHandler));


            builder.RegisterType(typeof(ICommandHandler<CreateItemTabCommand>), typeof(CreateItemTabCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<AssignItemToTabCommand>), typeof(AssignItemToTabCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<ChangeItemTabNameCommand>), typeof(ChangeItemTabNameCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteItemTabCommand>), typeof(DeleteItemTabCommandHandler));


            //item command
            builder.RegisterType(typeof(ICommandHandlerAsync<RateItemCommand>), typeof(RateItemCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<PreviewFailedCommand>), typeof(PreviewFailedCommandHandler));


            //statistics
            builder.RegisterType(typeof(ICommandHandlerAsync<UpdateStatisticsCommand>), typeof(UpdateStatisticsCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateBadgesCommand>), typeof(UpdateBadgesCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateReputationCommand>), typeof(UpdateReputationCommandHandler));

            //create university app
            builder.RegisterType(typeof(ICommandHandlerAsync<CreateUniversityCommand>), typeof(CreateUniversityCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateUniversityStatsCommand>), typeof(UpdateUniversityStatsCommandHandler));


            //QnA
            builder.RegisterType(typeof(ICommandHandlerAsync<AddCommentCommand, AddCommentCommandResult>), typeof(AddCommentCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<AddReplyToCommentCommand>), typeof(AddReplyToCommentCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteCommentCommand>), typeof(DeleteCommentCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteReplyCommand>), typeof(DeleteReplyCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<LikeCommentCommand, LikeCommentCommandResult>), typeof(LikeCommentCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<LikeReplyCommand, LikeReplyCommandResult>), typeof(LikeReplyCommandHandler));

            //message
            builder.RegisterType(typeof(ICommandHandlerAsync<ShareBoxCommand>), typeof(ShareBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<InviteToSystemCommand>), typeof(InviteToSystemCommandHandler));

            // builder.RegisterType(typeof(ICommandHandlerAsync<AddReputationCommand>), typeof(AddReputationCommandHandler));

            //updates
            builder.RegisterType(typeof(ICommandHandlerAsync<AddNewUpdatesCommand>), typeof(AddNewUpdatesCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteUpdatesCommand>), typeof(DeleteUpdatesCommandHandler));

            //quiz
            builder.RegisterType(typeof(ICommandHandler<CreateQuizCommand>), typeof(CreateQuizCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateQuizCommand>), typeof(UpdateQuizCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteQuizCommand>), typeof(DeleteQuizCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteQuestionCommand>), typeof(DeleteQuestionCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<CreateQuestionCommand>), typeof(CreateQuestionCommandHandler));
                builder.RegisterType(typeof(ICommandHandlerAsync<SaveQuizCommand, SaveQuizCommandResult>),
                    typeof(SaveQuizCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<SaveUserQuizCommand>), typeof(SaveUserQuizCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<CreateDiscussionCommand>),
                typeof(CreateDiscussionCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteDiscussionCommand>), typeof(DeleteDiscussionCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<AddStudentCommand>), typeof(AddStudentCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<AddUserLocationActivityCommand>), typeof(AddUserLocationActivityCommandHandler));

            builder.RegisterType(typeof(ICommandHandlerAsync<AddQuizLikeCommand>),
                typeof(AddQuizLikeCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteQuizLikeCommand>),
                typeof(DeleteQuizLikeCommandHandler));

            //library
            builder.RegisterType(typeof(ICommandHandler<AddNodeToLibraryCommand>),
                typeof(AddNodeToLibraryCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<RequestAccessLibraryNodeCommand>),
                typeof(RequestAccessLibraryNodeCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<LibraryNodeApproveAccessCommand>),
                typeof(LibraryNodeApproveAccessCommandHandler));

            //online + chat
            builder.RegisterType(typeof(ICommandHandler<ChangeUserOnlineStatusCommand>),
                typeof(ChangeUserOnlineStatusCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<ChatAddMessageCommand>),
                typeof(ChatAddMessageCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<ManageConnectionsCommand>),
                typeof(ManageConnectionsCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<RemoveOldConnectionCommand>),
                typeof(RemoveOldConnectionCommandHandler));


            builder.RegisterType(typeof(ICommandHandler<ChatMarkAsReadCommand>),
                typeof(ChatMarkAsReadCommandHandler));

            //flashcard
            builder.RegisterType(typeof(ICommandHandlerAsync<AddFlashcardCommand>),
                typeof(AddFlashcardCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<UpdateFlashcardCommand>),
                typeof(UpdateFlashcardCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<PublishFlashcardCommand>),
                typeof(PublishFlashcardCommandHandler));

            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteFlashcardCommand>),
                typeof(DeleteFlashcardCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<AddFlashcardPinCommand>),
                typeof(AddFlashcardPinCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteFlashcardPinCommand>),
                typeof(DeleteFlashcardPinCommandHandler));

            builder.RegisterType(typeof(ICommandHandlerAsync<AddFlashcardLikeCommand>),
                typeof(AddFlashcardLikeCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteFlashcardLikeCommand>),
                typeof(DeleteFlashcardLikeCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<SaveUserFlashcardCommand>),
                typeof(SaveUserFlashcardCommandHandler));
        }
    }
}
