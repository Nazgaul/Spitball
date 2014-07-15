using Zbang.Zbox.Domain.CommandHandlers.Quiz;
using Zbang.Zbox.Domain.CommandHandlers.Store;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Domain.CommandHandlers.Ioc
{
    public class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;

            ioc.RegisterType(typeof(ICommandHandler<CreateUserCommand, CreateUserCommandResult>), typeof(CreateMembershipUserCommandHandler), "Membership");
            ioc.RegisterType(typeof(ICommandHandler<CreateUserCommand, CreateUserCommandResult>), typeof(CreateFacebookUserCommandHandler), "Facebook");

            ioc.RegisterType(typeof(ICommandHandler<UpdateUserPasswordCommand, UpdateUserCommandResult>), typeof(UpdateUserPasswordCommandHandler))
            .RegisterType(typeof(ICommandHandler<UpdateUserEmailCommand>), typeof(UpdateUserEmailCommandHandler))
            .RegisterType(typeof(ICommandHandler<UpdateUserProfileCommand>), typeof(UpdateUserProfileCommandHandler))
            .RegisterType(typeof(ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>), typeof(CreateBoxCommandHandler), "CreateBoxCommand")
            .RegisterType(typeof(ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>), typeof(CreateAcademicBoxCommandHandler), "CreateAcademicBoxCommand")
            .RegisterType(typeof(ICommandHandler<ChangeBoxInfoCommand>), typeof(ChangeBoxInfoCommandHandler))
            .RegisterType(typeof(ICommandHandler<ChangeBoxPrivacySettingsCommand, ChangeBoxPrivacySettingsCommandResult>), typeof(ChangeBoxPrivacySettingsCommandHandler))
            .RegisterType(typeof(ICommandHandler<ChangeNotificationSettingsCommand>), typeof(ChangeNotificationSettingsCommandHandler));

            ioc.RegisterType(typeof(ICommandHandler<DeleteBoxCommand>), typeof(DeleteBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<UnfollowBoxCommand>), typeof(UnfollowBoxCommandHandler));
            

            ioc.RegisterType(typeof(ICommandHandler<AddFileToBoxCommand, AddFileToBoxCommandResult>), typeof(AddFileToBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<AddLinkToBoxCommand, AddLinkToBoxCommandResult>), typeof(AddLinkToBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<ChangeFileNameCommand, ChangeFileNameCommandResult>), typeof(ChangeFileNameCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<UpdateThumbnailCommand>), typeof(UpdateThumbnailCommandHandler));

            ioc.RegisterType(typeof(ICommandHandler<DeleteItemCommand>), typeof(DeleteItemCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<SubscribeToSharedBoxCommand>), typeof(SubscribeToSharedBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteUserFromBoxCommand>), typeof(DeleteUserFromBoxCommandHandler));
            //ioc.RegisterType(typeof(ICommandHandler<AddBoxCommentCommand, AddBoxCommentCommandResult>), typeof(AddBoxCommentCommandHandler));
            //ioc.RegisterType(typeof(ICommandHandler<AddReplyToCommentCommand, AddReplyToCommentCommandResult>), typeof(AddReplyToCommentCommandHandler));
            //ioc.RegisterType(typeof(ICommandHandler<DeleteCommentCommand>), typeof(DeleteCommentCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<UpdateUserLanguageCommand>), typeof(UpdateUserLanguageCommandHandler));

            ioc.RegisterType(typeof(ICommandHandler<AddNodeToLibraryCommand>), typeof(AddNodeToLibraryCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<RenameNodeCommand>), typeof(RenameNodeCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteNodeFromLibraryCommand>), typeof(DeleteNodeFromLibraryCommandHandler));

            ioc.RegisterType(typeof(ICommandHandler<UpdateUserUniversityCommand>), typeof(UpdateUserUniversityCommandHandler));


            //anotation
            ioc.RegisterType(typeof(ICommandHandler<AddAnnotationCommand>), typeof(AddAnnotationCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<AddReplyToAnnotationCommand>), typeof(AddReplyToAnnotationCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteAnnotationCommand>), typeof(DeleteAnnotationCommandHandler));


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

            //create university app
            ioc.RegisterType(typeof(ICommandHandler<CreateUniversityCommand>), typeof(CreateUniversityCommandHandler));

            //QnA
            ioc.RegisterType(typeof(ICommandHandler<AddCommentCommand>), typeof(AddQuestionCommandHandler));
            ioc.RegisterType(typeof(ICommandHandlerAsync<AddAnswerToQuestionCommand>), typeof(AddAnswerToQuestionCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<MarkAsAnswerCommand>), typeof(MarkAsAnswerCommandHandler));
            //ioc.RegisterType(typeof(ICommandHandler<RateAnswerCommand>), typeof(RateAnswerCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteFileFromQnACommand>), typeof(DeleteFileFromQnACommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteCommentCommand>), typeof(DeleteCommentCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<DeleteReplyCommand>), typeof(DeleteReplyCommandHandler));

            //message
            ioc.RegisterType(typeof(ICommandHandler<SendMessageCommand>), typeof(SendMessageCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<ShareBoxCommand>), typeof(ShareBoxCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<ShareBoxFacebookCommand>), typeof(ShareBoxFacebookCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<InviteToSystemCommand>), typeof(InviteToSystemCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<InviteToSystemFacebookCommand>), typeof(InviteToSystemFacebookCommandHandler));
            ioc.RegisterType(typeof(ICommandHandler<MarkMessagesAsReadCommand>), typeof(MarkMessagesAsReadCommandHanlder));
            ioc.RegisterType(typeof(ICommandHandler<MarkMessagesAsOldCommand>), typeof(MarkMessagesAsOldCommandHanlder));



            ioc.RegisterType(typeof(ICommandHandler<AddReputationCommand>), typeof(AddReputationCommandHandler));

            //updates
            ioc.RegisterType(typeof(ICommandHandler<AddNewUpdatesCommand>), typeof(AddNewUpdatesCommandHandler));
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
            .RegisterType(typeof(ICommandHandler<SaveQuizCommand, SaveQuizCommandResult>), typeof(SaveQuizCommandHandler))
            .RegisterType(typeof(ICommandHandler<SaveUserQuizCommand>), typeof(SaveUserQuizCommandHandler))
            .RegisterType(typeof(ICommandHandler<CreateDiscussionCommand>), typeof(CreateDiscussionCommandHandler))
            .RegisterType(typeof(ICommandHandler<DeleteDiscussionCommand>), typeof(DeleteDiscussionCommandHandler))
            .RegisterType(typeof(ICommandHandler<CreateAnswerCommand>), typeof(CreateAnswerCommandHandler))
            .RegisterType(typeof(ICommandHandler<MarkAnswerCorrectCommand>), typeof(MarkAnswerCorrectCommandHandler));

            ioc.RegisterType(typeof (ICommandHandler<AddStudentCommand>), typeof (AddStudentCommandHandler));

            //product

            ioc.RegisterType(typeof (ICommandHandler<AddProductsToStoreCommand>),
                typeof (AddProductsToStoreCommandHandler));

        }
    }
}
