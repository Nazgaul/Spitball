using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxWriteService
    {
        CreateUserCommandResult CreateUser(CreateUserCommand command);
        UpdateUserCommandResult UpdateUserPassword(UpdateUserPasswordCommand command);
        void UpdateUserEmail(UpdateUserEmailCommand command);
        void UpdateUserProfile(UpdateUserProfileCommand command);
        void UpdateUserLanguage(UpdateUserLanguageCommand command);
        void UpdateUserUniversity(UpdateUserUniversityCommand command);
        CreateBoxCommandResult CreateBox(CreateBoxCommand command);
        void ChangeBoxInfo(ChangeBoxInfoCommand command);
        ChangeBoxPrivacySettingsCommandResult ChangeBoxPrivacySettings(ChangeBoxPrivacySettingsCommand command);
        void DeleteBox(DeleteBoxCommand command);
        void UnfollowBox(UnfollowBoxCommand command);

        AddFileToBoxCommandResult AddFileToBox(AddFileToBoxCommand command);
        AddLinkToBoxCommandResult AddLinkToBox(AddLinkToBoxCommand command);
        AddBoxCommentCommandResult AddBoxComment(AddBoxCommentCommand command);
        AddReplyToCommentCommandResult AddReplyToComment(AddReplyToCommentCommand command);

        void DeleteItem(DeleteItemCommand command);
        void RateItem(RateItemCommand command);
        

        void DeleteUserFromBox(DeleteUserFromBoxCommand command);
        void ChangeNotificationSettings(ChangeNotificationSettingsCommand command);

        void ShareBox(ShareBoxCommand command);
        void InviteSystem(InviteToSystemCommand command);
        void ShareBoxFacebbok(ShareBoxFacebookCommand command);
        void InviteSystemFromFacebook(InviteToSystemFacebookCommand inviteCommand);

        void SendMessage(SendMessageCommand command);
        void SubscribeToSharedBox(SubscribeToSharedBoxCommand command);
        void DeleteComment(DeleteCommentCommand command);
        ChangeFileNameCommandResult ChangeFileName(ChangeFileNameCommand command);

        

        void AddNodeToLibrary(AddNodeToLibraryCommand command);
        void RenameNodeLibrary(RenameNodeCommand command);
        void DeleteNodeLibrary(DeleteNodeFromLibraryCommand command);

        #region ZboxWorkerRoleService
        void UpdateThumbnailPicture(UpdateThumbnailCommand command);
        void AddNewUpdate(AddNewUpdatesCommand command);
        #endregion


        void CreateBoxItemTab(CreateItemTabCommand command);


        void AssignBoxItemToTab(AssignItemToTabCommand command);
        void RenameBoxItemTab(ChangeItemTabNameCommand command);
        void DeleteBoxItemTab(DeleteItemTabCommand command);


        void UpdateUserFirstTimeStatus(UpdateUserFirstTimeStatusCommand command);

        void Dbi();
        void Statistics(UpdateStatisticsCommand command);

        #region annotation
        void AddAnnotation(AddAnnotationCommand command);
        void AddReplyAnnotation(AddReplyToAnnotationCommand command);
        void DeleteAnnotation(DeleteAnnotationCommand command);
        #endregion

        #region QnA
        void AddQuestion(AddQuestionCommand command);
        Task AddAnswer(AddAnswerToQuestionCommand command);
        void MarkCorrectAnswer(MarkAsAnswerCommand command);
        void RateAnswer(RateAnswerCommand command);
        void DeleteFileFromQnA(DeleteFileFromQnACommand command);
        void DeleteQuestion(DeleteQuestionCommand command);
        void DeleteAnswer(DeleteAnswerCommand command);
        #endregion

        void AddReputation(AddReputationCommand command);
        void DeleteUpdates(DeleteUpdatesCommand command);

        void MarkMessageAsRead(MarkMessagesAsReadCommand command);
        void MarkMessagesAsOld(MarkMessagesAsOldCommand command);
       
    }
}
