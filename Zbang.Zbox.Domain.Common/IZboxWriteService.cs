﻿using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxWriteService
    {
        Task<CreateUserCommandResult> CreateUserAsync(CreateUserCommand command);
        Task<UpdateUserCommandResult> UpdateUserPasswordAsync(UpdateUserPasswordCommand command);

        Task UpdateUserEmailAsync(UpdateUserEmailCommand command);
        void UpdateUserProfile(UpdateUserProfileCommand command);
        void UpdateUserLanguage(UpdateUserLanguageCommand command);
        void UpdateUserUniversity(UpdateUserUniversityCommand command);
        CreateBoxCommandResult CreateBox(CreateBoxCommand command);
        void ChangeBoxInfo(ChangeBoxInfoCommand command);
        //void DeleteBox(DeleteBoxCommand command);
        Task UnFollowBoxAsync(UnFollowBoxCommand command);
        
        
        

        Task DeleteItemAsync(DeleteItemCommand command);
        Task RateItemAsync(RateItemCommand command);


        void DeleteUserFromBox(DeleteUserFromBoxCommand command);
        void ChangeNotificationSettings(ChangeNotificationSettingsCommand command);

        Task ShareBoxAsync(ShareBoxCommand command);
        Task InviteSystemAsync(InviteToSystemCommand command);
        void ShareBoxFacebook(ShareBoxFacebookCommand command);
        void InviteSystemFromFacebook(InviteToSystemFacebookCommand inviteCommand);

        Task SendMessageAsync(SendMessageCommand command);
        Task SubscribeToSharedBoxAsync(SubscribeToSharedBoxCommand command);
        //void DeleteComment(DeleteCommentCommand command);
        ChangeFileNameCommandResult ChangeFileName(ChangeFileNameCommand command);



        //void AddNodeToLibrary(AddNodeToLibraryCommand command);
        void RenameNodeLibrary(RenameNodeCommand command);
        void DeleteNodeLibrary(DeleteNodeFromLibraryCommand command);

     


        void CreateBoxItemTab(CreateItemTabCommand command);


        void AssignBoxItemToTab(AssignItemToTabCommand command);
        void RenameBoxItemTab(ChangeItemTabNameCommand command);
        void DeleteBoxItemTab(DeleteItemTabCommand command);


        void UpdateUserFirstTimeStatus(UpdateUserFirstTimeStatusCommand command);



        Task<AddItemToBoxCommandResult> AddItemToBoxAsync(AddItemToBoxCommand command);

        #region annotation
        Task AddAnnotationAsync(AddAnnotationCommand command);
        Task AddReplyAnnotationAsync(AddReplyToAnnotationCommand command);
        void DeleteAnnotation(DeleteItemCommentCommand command);
        void DeleteItemCommentReply(DeleteItemCommentReplyCommand command);
        #endregion

        #region QnA
        Task<AddCommentCommandResult> AddQuestionAsync(AddCommentCommand command);
        Task AddAnswerAsync(AddAnswerToQuestionCommand command);
        void DeleteComment(DeleteCommentCommand command);
        void DeleteAnswer(DeleteReplyCommand command);
        #endregion

        Task AddReputationAsync(AddReputationCommand command);
        void DeleteUpdates(DeleteUpdatesCommand command);

        void MarkMessageAsRead(MarkMessagesAsReadCommand command);
        void DeleteNotification(DeleteNotificationCommand command);

        #region Quiz
        Task CreateQuizAsync(CreateQuizCommand command);
        void UpdateQuiz(UpdateQuizCommand command);
        void DeleteQuiz(DeleteQuizCommand command);
        void CreateQuestion(CreateQuestionCommand command);
        void UpdateQuestion(UpdateQuestionCommand command);
        void DeleteQuestion(DeleteQuestionCommand command);
        void CreateAnswer(CreateAnswerCommand command);
        void DeleteAnswer(DeleteAnswerCommand command);
        void UpdateAnswer(UpdateAnswerCommand command);
        Task<SaveQuizCommandResult> SaveQuizAsync(SaveQuizCommand command);
        Task SaveUserAnswersAsync(SaveUserQuizCommand command);
        void CreateItemInDiscussion(CreateDiscussionCommand command);
        void DeleteItemInDiscussion(DeleteDiscussionCommand command);

        void MarkAnswerAsCorrect(MarkAnswerCorrectCommand command);
        void AddStudent(AddStudentCommand command);

        #endregion

       



        void CreateDepartment(AddNodeToLibraryCommand command);


        void CreateUniversity(CreateUniversityCommand command);


        //mobile
        void RegisterMobileDevice(RegisterMobileDeviceCommand command);

    }
}
