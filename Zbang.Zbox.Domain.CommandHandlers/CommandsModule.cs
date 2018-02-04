using Autofac;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CommandsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<Item>))
            .As<ICommandHandler<RemoveTagsFromDocumentCommand>>();
            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<FlashcardMeta>))
                .As<ICommandHandler<RemoveTagsFromFlashcardCommand>>();
            builder.RegisterType(typeof(RemoveTagsFromItemCommandHandler<Domain.Quiz>))
               .As<ICommandHandler<RemoveTagsFromQuizCommand>>();

            builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();


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

            builder.RegisterType(typeof(ICommandHandlerAsync<UnFollowBoxCommand>), typeof(UnFollowBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteBoxCommand>), typeof(DeleteBoxCommandHandler));

       
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
            builder.RegisterType(typeof(ICommandHandler<DeleteItemCommentCommand>), typeof(DeleteItemCommentCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<DeleteItemCommentReplyCommand>), typeof(DeleteItemCommentReplyCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<CreateItemTabCommand>), typeof(CreateItemTabCommandHandler));
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

           
       
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteQuizLikeCommand>),
                typeof(DeleteQuizLikeCommandHandler));

            //library
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

          
            builder.RegisterType(typeof(ICommandHandlerAsync<UpdateFlashcardCommand>),
                typeof(UpdateFlashcardCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<PublishFlashcardCommand>),
                typeof(PublishFlashcardCommandHandler));

            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteFlashcardCommand>),
                typeof(DeleteFlashcardCommandHandler));

           
            builder.RegisterType(typeof(ICommandHandler<DeleteFlashcardPinCommand>),
                typeof(DeleteFlashcardPinCommandHandler));

           
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteFlashcardLikeCommand>),
                typeof(DeleteFlashcardLikeCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<SaveUserFlashcardCommand>),
                typeof(SaveUserFlashcardCommandHandler));
        }
    }
}
