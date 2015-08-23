﻿using Zbang.Zbox.Domain.CommandHandlers.Quiz;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Domain.CommandHandlers.Ioc
{
    public static class RegisterIoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;

            ioc
                .RegisterType
                <ICommandHandlerAsync<CreateUserCommand, CreateUserCommandResult>, CreateMembershipUserCommandHandler>(
                    CreateMembershipUserCommand.ResolveName);
            ioc
                .RegisterType
                <ICommandHandlerAsync<CreateUserCommand, CreateUserCommandResult>, CreateFacebookUserCommandHandler>(
                    CreateFacebookUserCommand.ResolveName);

            ioc
               .RegisterType
               <ICommandHandlerAsync<CreateUserCommand, CreateUserCommandResult>, CreateGoogleUserCommandHandler>(
                   CreateGoogleUserCommand.ResolveName);

            ioc.RegisterType(typeof(ICommandHandlerAsync<UpdateUserPasswordCommand, UpdateUserCommandResult>), typeof(UpdateUserPasswordCommandHandler))
            .RegisterType(typeof(ICommandHandlerAsync<UpdateUserEmailCommand>), typeof(UpdateUserEmailCommandHandler))
            .RegisterType(typeof(ICommandHandler<UpdateUserProfileCommand>), typeof(UpdateUserProfileCommandHandler))
            .RegisterType<ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>, CreateBoxCommandHandler>("CreateBoxCommand");
            ioc.RegisterType<ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>, CreateAcademicBoxCommandHandler>("CreateAcademicBoxCommand");
            ioc.RegisterType(typeof(ICommandHandler<ChangeBoxInfoCommand>), typeof(ChangeBoxInfoCommandHandler))
            .RegisterType(typeof(ICommandHandler<ChangeNotificationSettingsCommand>), typeof(ChangeNotificationSettingsCommandHandler))
            .RegisterType(typeof(ICommandHandler<RegisterMobileDeviceCommand>), typeof(UpdateUserMobileSettings));

            ioc.RegisterType(typeof(ICommandHandlerAsync<UnFollowBoxCommand>), typeof(UnFollowBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandlerAsync<DeleteBoxCommand>), typeof(DeleteBoxCommandHandler));

            ioc
                .RegisterType
                <ICommandHandlerAsync<AddItemToBoxCommand, AddItemToBoxCommandResult>, AddFileToBoxCommandHandler>(
                    AddItemToBoxCommand.FileResolver);
            ioc
                .RegisterType
                <ICommandHandlerAsync<AddItemToBoxCommand, AddItemToBoxCommandResult>, AddLinkToBoxCommandHandler>(
                    AddItemToBoxCommand.LinkResolver);
            ioc.RegisterType(typeof(ICommandHandler<ChangeFileNameCommand, ChangeFileNameCommandResult>), typeof(ChangeFileNameCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<UpdateThumbnailCommand>), typeof(UpdateThumbnailCommandHandler));

            ioc.RegisterType(typeof(ICommandHandlerAsync<DeleteItemCommand>), typeof(DeleteItemCommandHandler));
            ioc.RegisterType(typeof(ICommandHandlerAsync<SubscribeToSharedBoxCommand>), typeof(SubscribeToSharedBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteUserFromBoxCommand>), typeof(DeleteUserFromBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<UpdateUserLanguageCommand>), typeof(UpdateUserLanguageCommandHandler));

            ioc.RegisterType(typeof(ICommandHandler<RenameNodeCommand>), typeof(RenameNodeCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteNodeFromLibraryCommand>), typeof(DeleteNodeFromLibraryCommandHandler));

            ioc.RegisterType(typeof(ICommandHandler<UpdateUserUniversityCommand>), typeof(UpdateUserUniversityCommandHandler));


            //annotation
            ioc.RegisterType(typeof(ICommandHandlerAsync<AddAnnotationCommand>), typeof(AddAnnotationCommandHandler));
            ioc.RegisterType(typeof(ICommandHandlerAsync<AddReplyToAnnotationCommand>), typeof(AddReplyToAnnotationCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteItemCommentCommand>), typeof(DeleteItemCommentCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteItemCommentReplyCommand>), typeof(DeleteItemCommentReplyCommandHandler));


            ioc.RegisterType(typeof(ICommandHandler<CreateItemTabCommand>), typeof(CreateItemTabCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<AssignItemToTabCommand>), typeof(AssignItemToTabCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<ChangeItemTabNameCommand>), typeof(ChangeItemTabNameCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteItemTabCommand>), typeof(DeleteItemTabCommandHandler));

            //first time
            ioc.RegisterType(typeof(ICommandHandler<UpdateUserFirstTimeStatusCommand>), typeof(UpdateUserFirstTimeStatusCommandHandler));

            //item command
            ioc.RegisterType(typeof(ICommandHandler<RateItemCommand>), typeof(RateItemCommandHandler));


            //statistics
            ioc.RegisterType(typeof(ICommandHandler<UpdateStatisticsCommand>), typeof(UpdateStatisticsCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<UpdateReputationCommand>), typeof(UpdateReputationCommandHandler));

            //create university app
            ioc.RegisterType(typeof(ICommandHandler<CreateUniversityCommand>), typeof(CreateUniversityCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<UpdateUniversityStatsCommand>), typeof(UpdateUniversityStatsCommandHandler));


            //QnA
            ioc.RegisterType(typeof(ICommandHandlerAsync<AddCommentCommand, AddCommentCommandResult>), typeof(AddQuestionCommandHandler));
            ioc.RegisterType(typeof(ICommandHandlerAsync<AddAnswerToQuestionCommand>), typeof(AddAnswerToQuestionCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteCommentCommand>), typeof(DeleteCommentCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteReplyCommand>), typeof(DeleteReplyCommandHandler));

            //message
            ioc.RegisterType(typeof(ICommandHandlerAsync<SendMessageCommand>), typeof(SendMessageCommandHandler));
            ioc.RegisterType(typeof(ICommandHandlerAsync<ShareBoxCommand>), typeof(ShareBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<ShareBoxFacebookCommand>), typeof(ShareBoxFacebookCommandHandler));
            ioc.RegisterType(typeof(ICommandHandlerAsync<InviteToSystemCommand>), typeof(InviteToSystemCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<InviteToSystemFacebookCommand>), typeof(InviteToSystemFacebookCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<MarkMessagesAsReadCommand>), typeof(MarkMessagesAsReadCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteNotificationCommand>), typeof(DeleteNotificationCommandHandler));


            ioc.RegisterType(typeof(ICommandHandlerAsync<AddReputationCommand>), typeof(AddReputationCommandHandler));

            //updates
            ioc.RegisterType(typeof(ICommandHandlerAsync<AddNewUpdatesCommand>), typeof(AddNewUpdatesCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteUpdatesCommand>), typeof(DeleteUpdatesCommandHandler));

            //quiz
            ioc.RegisterType(typeof(ICommandHandler<CreateQuizCommand>), typeof(CreateQuizCommandHandler))
            .RegisterType(typeof(ICommandHandler<UpdateQuizCommand>), typeof(UpdateQuizCommandHandler))
            .RegisterType(typeof(ICommandHandler<DeleteQuizCommand>), typeof(DeleteQuizCommandHandler))
            .RegisterType(typeof(ICommandHandler<UpdateQuestionCommand>), typeof(UpdateQuestionCommandHandler))
            .RegisterType(typeof(ICommandHandler<DeleteQuestionCommand>), typeof(DeleteQuestionCommandHandler))
            .RegisterType(typeof(ICommandHandler<CreateQuestionCommand>), typeof(CreateQuestionCommandHandler))
            .RegisterType(typeof(ICommandHandler<DeleteAnswerCommand>), typeof(DeleteAnswerCommandHandler))
            .RegisterType(typeof(ICommandHandler<UpdateAnswerCommand>), typeof(UpdateAnswerCommandHandler))
            .RegisterType(typeof(ICommandHandlerAsync<SaveQuizCommand, SaveQuizCommandResult>), typeof(SaveQuizCommandHandler))
            .RegisterType(typeof(ICommandHandler<SaveUserQuizCommand>), typeof(SaveUserQuizCommandHandler))
            .RegisterType(typeof(ICommandHandler<CreateDiscussionCommand>), typeof(CreateDiscussionCommandHandler))
            .RegisterType(typeof(ICommandHandler<DeleteDiscussionCommand>), typeof(DeleteDiscussionCommandHandler))
            .RegisterType(typeof(ICommandHandler<CreateAnswerCommand>), typeof(CreateAnswerCommandHandler))
            .RegisterType(typeof(ICommandHandler<MarkAnswerCorrectCommand>), typeof(MarkAnswerCorrectCommandHandler));

            ioc.RegisterType(typeof(ICommandHandler<AddStudentCommand>), typeof(AddStudentCommandHandler));

   
            //library
            ioc.RegisterType(typeof(ICommandHandler<AddNodeToLibraryCommand>),
                typeof(AddNodeToLibraryCommandHandler));


        }
    }
}
