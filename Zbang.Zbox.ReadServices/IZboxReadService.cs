using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto;
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
        
        Task<IEnumerable<BoxDto>> GetUserBoxesOld(GetBoxesQuery query);
        Task<IEnumerable<BoxDto>> GetUserBoxesAsync(GetBoxesQuery query);

        //Task<DashboardDto> GetDashboardSideBarAsync(GetDashboardQuery query);
        Task<IEnumerable<LeaderBoardDto>> GetDashboardLeaderBoardAsync(LeaderBoardQuery query);

        Task<UniversityDashboardInfoDto> GetUniversityInfoAsync(UniversityQuery query);

        Task<NodeBoxesDto> GetLibraryNode(GetLibraryNodeQuery query);
        Task<UniversityInfoDto> GetUniversityDetail(GetUniversityDetailQuery query);
        Task<IEnumerable<InviteDto>> GetInvites(GetInvitesQuery query);

        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query);
        Task<IEnumerable<QuizDto>> GetBoxQuizesAsync(GetBoxQuizesPagedQuery query);

        Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoardAsync(GetLeaderBoardQuery query);
        Task<IEnumerable<RecommendBoxDto>> GetBoxRecommendedCourses(GetBoxSideBarQuery query);
        ItemWithDetailDto GetItem(GetItemQuery query);

        Task<ItemDetailDto> GetItem2(GetItemQuery query);
        Task<ItemMobileDto> GetItemDetailApi(GetItemQuery query);
        Task<FileSeo> GetItemSeo(GetFileSeoQuery query);
        Task<BoxSeoDto> GetBoxSeo(GetBoxSeoQuery query);


        Task<BoxDto2> GetBox2(GetBoxQuery query);

        Task<BoxDtoWithMembers> GetBoxMetaWithMembersAsync(GetBoxQuery query, int numberOfMembers);
        Task<IEnumerable<TabDto>> GetBoxTabs(GetBoxQuery query);


        Task<IEnumerable<UserDto>> GetUserFriendsAsync(GetUserFriendsQuery query);

        UserDetailDto GetUserData(GetUserDetailsQuery query);
        Task<UserDetailDto> GetUserDataAsync(GetUserDetailsQuery query);

        Task<UserAccountDto> GetUserAccountDetailsAsync(GetUserDetailsQuery query);
        Task<IEnumerable<BoxNotificationDto>> GetUserBoxesNotificationAsync(GetUserDetailsQuery query);
        NotificationSettings GetUserBoxNotificationSettings(GetBoxQuery query, long userId);

        

        BoxSettingsDto GetBoxSetting(GetBoxQuery query, long userId);

        Task<IEnumerable<UserMemberDto>> GetBoxMembersAsync(GetBoxQuery query);
        Task<IEnumerable<long>> GetBoxUsersId(GetBoxWithUserQuery query);

        Task<UserMinProfile> GetUserMinProfile(GetUserMinProfileQuery query);

        Task<string> GetLocationByIpAsync(GetCountryByIpQuery query);
        Task<IEnumerable<UniversityByFriendDto>> GetUniversityListByFriendsIds(IEnumerable<long> friendsIds);


        Task<IEnumerable<QuestionDto>> GetQuestionsWithAnswersAsync(GetBoxQuestionsQuery query);
        Task<IEnumerable<QuestionDto>> GetQuestionsWithLastAnswerAsync(GetBoxQuestionsQuery query);
        Task<IEnumerable<AnswerDto>> GetReplies(GetCommentRepliesQuery query);
        Task<QuestionDto> GetQuestionAsync(GetQuestionQuery query);
        Task<bool> GetInvite(GetInviteDetailQuery query);



        //user page
        Task<IEnumerable<BoxDto>> GetUserBoxesActivityAsync(GetUserWithFriendQuery query);

        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetUserItemsActivityAsync(GetUserWithFriendQuery query);
        Task<IEnumerable<ActivityDto>> GetUserCommentActivityAsync(GetUserWithFriendQuery query);
        Task<IEnumerable<QuizDto>> GetUserQuizActivityAsync(GetUserWithFriendQuery query);
        Task<UserWithStats> GetUserProfileWithStatsAsync(GetUserWithFriendQuery query);

        Task<IEnumerable<string>> GetSeoItems(int page);
        Task<int> GetSeoItemCount();

        //Task<IEnumerable<AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query);

        Task<IEnumerable<RussianDepartmentDto>> GetRussianDepartmentList(long universityId);



        Task<UniversityWithCodeDto> GetUniversityNeedId(long universityId);
       


        Task<IEnumerable<UpdatesDto>> GetUpdates(QueryBase query);

        //Quiz
        Task<QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query);
        Task<QuizSeo> GetQuizSeo(GetQuizSeoQuery query);
        Task<QuizWithDetailDto> GetDraftQuiz(GetQuizDraftQuery query);
        Task<IEnumerable<DiscussionDto>> GetDiscussion(GetDisscussionQuery query);
        Task<int> GetNumberOfSolvers(long quizId);

        //Quiz api stuff
        Task<QuizSolversWithCountDto> GetQuizSolversAsync(GetQuizBestSolvers query);
        Task<QuizQuestionWithSolvedAnswersDto> GetQuizQuestionWithAnswersAsync(GetQuizQuery query);


        Task<IEnumerable<RecommendBoxDto>> GetRecommendedCourses(RecommendedCoursesQuery query);

        Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddress(UniversityByIpQuery query);
        Task<IEnumerable<UserWithImageNameDto>> GetUsersByTermAsync(UserSearchQuery query);

    }
}
