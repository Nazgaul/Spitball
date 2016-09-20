using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.ViewModel.Dto.ActivityDtos;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Dashboard;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Dto.Qna;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Dashboard;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using BoxDto = Zbang.Zbox.ViewModel.Dto.BoxDto;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadService : IBaseReadService
    {
        // home page
        Task<HomePageDataDto> GetHomePageDataAsync(GetHomePageQuery query);
        Task<IEnumerable<RecommendBoxDto>> GetUniversityBoxesAsync(GetHomeBoxesUniversityQuery query);
        long? GetUniversityIdByUrl(string url);



        Task<IEnumerable<RecommendBoxDto>> GetCoursesPageDataAsync();
        Task<IEnumerable<BoxDto>> GetUserBoxesAsync(GetBoxesQuery query);

        Task<IEnumerable<LeaderBoardDto>> GetDashboardLeaderBoardAsync(LeaderBoardQuery query);

        Task<UniversityDashboardInfoDto> GetUniversityInfoAsync(UniversityQuery query);

        Task<NodeBoxesDto> GetLibraryNodeAsync(GetLibraryNodeQuery query);
        Task<IEnumerable<ClosedNodeDto>> GetUserClosedDepartmentAsync(QueryBase query);
        Task<IEnumerable<ClosedNodeUsersDto>> GetMembersClosedDepartmendAsync(GetClosedNodeMembersQuery query);

        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query);
        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetWebServiceBoxItemsPagedAsync(GetBoxItemsPagedQuery query);
        Task<IEnumerable<QuizDto>> GetBoxQuizesAsync(GetBoxQuizesPagedQuery query);

        Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoardAsync(GetLeaderBoardQuery query);

        Task<IEnumerable<RecommendBoxDto>> GetBoxRecommendedCoursesAsync(GetBoxSideBarQuery query,
            CancellationToken token);

        Task<ItemDetailDto> GetItem2Async(GetItemQuery query);
        Task<IEnumerable<AnnotationDto>> GetItemCommentsAsync(ItemCommentQuery query);
        Task<ItemMobileDto> GetItemDetailApiAsync(GetItemQuery query);
        Task<FileSeo> GetItemSeoAsync(GetFileSeoQuery query);
        Task<BoxSeoDto> GetBoxSeoAsync(GetBoxSeoQuery query);


        Task<BoxDto2> GetBox2Async(GetBoxQuery query);

        Task<BoxDtoWithMembers> GetBoxMetaWithMembersAsync(GetBoxQuery query, int numberOfMembers);
        Task<IEnumerable<TabDto>> GetBoxTabsAsync(GetBoxQuery query);


        Task<IEnumerable<UserDto>> GetUserFriendsAsync(GetUserFriendsQuery query);

        Task<UserDetailDto> GetUserDataAsync(GetUserDetailsQuery query);
        //Theme GetUserTheme(GetUserDetailsQuery query);

        Task<UserAccountDto> GetUserAccountDetailsAsync(GetUserDetailsQuery query);
        Task<UserNotification> GetUserBoxesNotificationAsync(GetUserDetailsQuery query);
        Task<NotificationSettings> GetUserBoxNotificationSettingsAsync(GetBoxQuery query, long userId);



        // BoxSettingsDto GetBoxSetting(GetBoxQuery query, long userId);

        Task<IEnumerable<UserMemberDto>> GetBoxMembersAsync(GetBoxQuery query);
        //Task<IEnumerable<long>> GetBoxUsersIdAsync(GetBoxWithUserQuery query);

        Task<UserMinProfile> GetUserMinProfileAsync(GetUserMinProfileQuery query);

        Task<string> GetLocationByIpAsync(GetCountryByIpQuery query);
        Task<IEnumerable<CommentDto>> GetCommentsAsync(GetBoxQuestionsQuery query);
        Task<IEnumerable<ReplyDto>> GetRepliesAsync(GetCommentRepliesQuery query);

        Task<IEnumerable<LikeDto>> GetCommentLikesAsync(GetFeedLikesQuery query);
        Task<IEnumerable<LikeDto>> GetReplyLikesAsync(GetFeedLikesQuery query);
        Task<CommentDto> GetCommentAsync(GetQuestionQuery query);
        Task<bool> GetInviteAsync(GetInviteDetailQuery query);



        //user page
        Task<IEnumerable<BoxDto>> GetUserBoxesActivityAsync(GetUserWithFriendQuery query);

        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetUserItemsActivityAsync(GetUserWithFriendQuery query);
        Task<IEnumerable<ActivityDto>> GetUserCommentActivityAsync(GetUserWithFriendQuery query);
        Task<IEnumerable<QuizDto>> GetUserQuizActivityAsync(GetUserWithFriendQuery query);
        Task<UserWithStats> GetUserProfileWithStatsAsync(GetUserWithFriendQuery query);

        Task<IEnumerable<SeoDto>> GetSeoItemsAsync(int page);
        Task<int> GetSeoItemCountAsync();




        Task<UniversityWithCodeDto> GetUniversityNeedIdAsync(long universityId);



        Task<IEnumerable<UpdatesDto>> GetUpdatesAsync(QueryBase query);

        //Quiz
        Task<QuizWithDetailSolvedDto> GetQuizAsync(GetQuizQuery query);
        Task<QuizSeo> GetQuizSeoAsync(GetQuizSeoQuery query);
        Task<QuizWithDetailDto> GetDraftQuizAsync(GetQuizDraftQuery query);
        Task<IEnumerable<DiscussionDto>> GetDiscussionAsync(GetDisscussionQuery query);
        Task<int> GetNumberOfSolversAsync(long quizId);

        //Quiz api stuff
        Task<QuizSolversWithCountDto> GetQuizSolversAsync(GetQuizBestSolvers query);
        Task<QuizQuestionWithSolvedAnswersDto> GetQuizQuestionWithAnswersAsync(GetQuizQuery query);


        Task<IEnumerable<RecommendBoxDto>> GetRecommendedCoursesAsync(RecommendedCoursesQuery query);

        Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddressAsync(UniversityByIpQuery query);
        Task<IEnumerable<UserWithImageNameDto>> GetUsersInBoxByTermAsync(UserInBoxSearchQuery query);
        //Task<IEnumerable<ChatUserDto>> GetUsersByTermAsync(UserSearchQuery query);

        //chat
        Task<IEnumerable<ChatUserDto>> GetUsersConversationAndFriendsAsync(GetUserConversationAndFriends query);
        Task<IEnumerable<ChatDto>> GetUserConversationAsync(GetChatRoomMessagesQuery query);
        Task<int> GetChatUnreadMessagesAsync(GetUserDetailsQuery query);
        //Task<int> GetUnreadChatMessagesAsync(QueryBase query);


    }
}
