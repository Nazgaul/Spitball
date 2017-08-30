using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxWriteService
    {
        Task<CreateUserCommandResult> CreateUserAsync(CreateUserCommand command);
        CreateJaredUserCommandResult CreateUserJared(CreateJaredUserCommand command);
        Task<UpdateUserCommandResult> UpdateUserPasswordAsync(UpdateUserPasswordCommand command);

        Task UpdateUserEmailAsync(UpdateUserEmailCommand command);
        void UpdateUserProfile(UpdateUserProfileCommand command);
        void UpdateUserImage(UpdateUserProfileImageCommand command);
        void UpdateUserLanguage(UpdateUserLanguageCommand command);
        void UpdateUserUniversity(UpdateUserUniversityCommand command);
        void UpdateUserEmailSettings(UpdateUserEmailSubscribeCommand command);
        Task<CreateBoxCommandResult> CreateBoxAsync(CreateBoxCommand command);
        void ChangeBoxInfo(ChangeBoxInfoCommand command);
        Task UnFollowBoxAsync(UnFollowBoxCommand command);
        
        
        

        Task DeleteItemAsync(DeleteItemCommand command);
        Task RateItemAsync(RateItemCommand command);


        void DeleteUserFromBox(DeleteUserFromBoxCommand command);
        void ChangeNotificationSettings(ChangeNotificationSettingsCommand command);

        Task ShareBoxAsync(ShareBoxCommand command);
        Task InviteSystemAsync(InviteToSystemCommand command);

        Task SubscribeToSharedBoxAsync(SubscribeToSharedBoxCommand command);
        ChangeFileNameCommandResult ChangeFileName(ChangeFileNameCommand command);



        void UpdateNodeSettings(UpdateNodeSettingsCommand command);
        Task DeleteNodeLibraryAsync(DeleteNodeFromLibraryCommand command);
        Task RequestAccessToDepartmentAsync(RequestAccessLibraryNodeCommand command);
        Task RequestAccessToDepartmentApprovedAsync(LibraryNodeApproveAccessCommand command);




        Task CreateBoxItemTabAsync(CreateItemTabCommand command);


        Task AssignBoxItemToTabAsync(AssignItemToTabCommand command);
        void RenameBoxItemTab(ChangeItemTabNameCommand command);
        void DeleteBoxItemTab(DeleteItemTabCommand command);





        Task<AddItemToBoxCommandResult> AddItemToBoxAsync(AddItemToBoxCommand command);

        #region annotation
        Task AddAnnotationAsync(AddItemCommentCommand command);
        Task AddReplyAnnotationAsync(AddItemReplyToCommentCommand command);
        Task DeleteAnnotationAsync(DeleteItemCommentCommand command);
        Task DeleteItemCommentReplyAsync(DeleteItemCommentReplyCommand command);
        #endregion

        #region QnA
        Task<AddCommentCommandResult> AddCommentAsync(AddCommentCommand command);
        Task AddReplyAsync(AddReplyToCommentCommand command);
        Task DeleteCommentAsync(DeleteCommentCommand command);
        Task DeleteReplyAsync(DeleteReplyCommand command);
        Task<LikeCommentCommandResult> LikeCommentAsync(LikeCommentCommand command);
        Task<LikeReplyCommandResult> LikeReplyAsync(LikeReplyCommand command);
        #endregion

        //Task AddReputationAsync(AddReputationCommand command);
        void DeleteUpdates(DeleteUpdatesCommand command);


        #region Quiz
        Task CreateQuizAsync(CreateQuizCommand command);
        void UpdateQuiz(UpdateQuizCommand command);
        Task DeleteQuizAsync(DeleteQuizCommand command);
        void CreateQuestion(CreateQuestionCommand command);
        void DeleteQuestion(DeleteQuestionCommand command);
        Task<SaveQuizCommandResult> SaveQuizAsync(SaveQuizCommand command);
        Task SaveUserAnswersAsync(SaveUserQuizCommand command);
        Task CreateItemInDiscussionAsync(CreateDiscussionCommand command);
        void DeleteItemInDiscussion(DeleteDiscussionCommand command);

        void AddStudent(AddStudentCommand command);
        Task AddQuizLikeAsync(AddQuizLikeCommand command);
        Task DeleteQuizLikeAsync(DeleteQuizLikeCommand command);

        #endregion

        void UpdatePreviewFailed(PreviewFailedCommand command);




        Task CreateDepartmentAsync(AddNodeToLibraryCommand command);


        Task CreateUniversityAsync(CreateUniversityCommand command);

        void ChangeOnlineStatus(ChangeUserOnlineStatusCommand command);
        //mobile
        void RegisterMobileDevice(RegisterMobileDeviceCommand command);


        //chat
        Task AddChatMessageAsync(ChatAddMessageCommand command);
        void MarkChatAsRead(ChatMarkAsReadCommand command);

        void ManageConnections(ManageConnectionsCommand command);
        void RemoveOldConnections(RemoveOldConnectionCommand command);


        Task AddUserLocationActivityAsync(AddUserLocationActivityCommand command);


        #region flashcard
        Task AddFlashcardAsync(AddFlashcardCommand command);
        Task UpdateFlashcardAsync(UpdateFlashcardCommand command);
        Task PublishFlashcardAsync(PublishFlashcardCommand command);
        Task DeleteFlashcardAsync(DeleteFlashcardCommand command);

        void AddPinFlashcard(AddFlashcardPinCommand command);
        void DeletePinFlashcard(DeleteFlashcardPinCommand command);
        Task AddFlashcardLikeAsync(AddFlashcardLikeCommand command);
        Task DeleteFlashcardLikeAsync(DeleteFlashcardLikeCommand command);
        void SolveFlashcard(SaveUserFlashcardCommand command);

        #endregion


        #region Jared

        //void AddCourseTag(CreateCourseTagCommand command);
        //void UpdateItemCourseTag<T>(T command) where T : UpdateItemCourseTagCommand;
        Task AddItemTagAsync<T>(T command) where T : AssignTagsToItemCommand;
        void RemoveItemTag<T>(T command) where T : RemoveTagsFromItemCommand;
        void SetReviewed<T>(T command) where T : SetReviewedCommand;
        void ChangeItemDocType(ChangeItemDocTypeCommand command);
        void AddItemLanguage<T>(T command) where T : AddLanguageToItemCommand;
        GetGeneralDepartmentCommandResult GetGeneralDepartmentForUniversity(GetGeneralDepartmentCommand command);
        //void DoWork(params ICommand[] commands);

        //void AddItemExtraData(
        //    AssignTagsToItemCommand command1,
        //    UpdateItemCourseTagCommand command2);

        #endregion
    }
}
