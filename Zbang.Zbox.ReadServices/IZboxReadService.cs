﻿using System.Collections.Generic;
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
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.Queries.User;
using BoxDto = Zbang.Zbox.ViewModel.Dto.BoxDto;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadService : IBaseReadService
    {
        
        Task<IEnumerable<BoxDto>> GetUserBoxesOld(GetBoxesQuery query);
        Task<IEnumerable<BoxDto>> GetUserBoxes(GetBoxesQuery query);

        Task<DashboardDto> GetDashboardSideBar(GetDashboardQuery query);

        Task<UniversityDashboardInfoDto> GetMyData(GetDashboardQuery query);

        Task<NodeBoxesDto> GetLibraryNode(GetLibraryNodeQuery query);
        Task<UniversityInfoDto> GetUniversityDetail(GetUniversityDetailQuery query);
        Task<IEnumerable<InviteDto>> GetInvites(GetInvitesQuery query);

        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query);
        Task<IEnumerable<QuizDto>> GetBoxQuizes(GetBoxQuizesPagedQuery query);

        Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoard(GetLeaderBoardQuery query);
        Task<IEnumerable<RecommendBoxDto>> GetBoxRecommendedCourses(GetBoxSideBarQuery query);
        ItemWithDetailDto GetItem(GetItemQuery query);

        Task<ItemDetailDto> GetItem2(GetItemQuery query);
        Task<ItemMobileDto> GetItemDetailApi(GetItemQuery query);
        Task<FileSeo> GetItemSeo(GetFileSeoQuery query);
        Task<BoxSeoDto> GetBoxSeo(GetBoxSeoQuery query);


        Task<BoxDto2> GetBox2(GetBoxQuery query);

        Task<BoxDtoWithMembers> GetBoxMetaWithMembersAsync(GetBoxQuery query, int numberOfMembers);
        Task<IEnumerable<TabDto>> GetBoxTabs(GetBoxQuery query);


        Task<IEnumerable<UserDto>> GetUserFriends(GetUserFriendsQuery query);

        UserDetailDto GetUserData(GetUserDetailsQuery query);
        Task<UserDetailDto> GetUserDataAsync(GetUserDetailsQuery query);

        UserAccountDto GetUserAccountDetails(GetUserDetailsQuery query);
        IEnumerable<BoxNotificationDto> GetUserBoxesNotification(GetUserDetailsQuery query);
        NotificationSettings GetUserBoxNotificationSettings(GetBoxQuery query, long userId);

        

        BoxSettingsDto GetBoxSetting(GetBoxQuery query, long userId);

        Task<IEnumerable<UserMemberDto>> GetBoxMembersAsync(GetBoxQuery query);
        Task<IEnumerable<long>> GetBoxUsersId(GetBoxWithUserQuery query);

        Task<UserMinProfile> GetUserMinProfile(GetUserMinProfileQuery query);

        string GetLocationByIp(long ipNumber);
        Task<IEnumerable<UniversityByFriendDto>> GetUniversityListByFriendsIds(IEnumerable<long> friendsIds);


        Task<IEnumerable<QuestionDto>> GetQuestionsWithAnswers(GetBoxQuestionsQuery query);
        Task<IEnumerable<QuestionDto>> GetQuestionsWithLastAnswer(GetBoxQuestionsQuery query);
        Task<IEnumerable<AnswerDto>> GetReplies(GetCommentRepliesQuery query);
        Task<QuestionDto> GetQuestionAsync(GetQuestionQuery query);
        Task<bool> GetInvite(GetInviteDetailQuery query);



        //user page
        Task<IEnumerable<BoxDto>> GetUserWithFriendBoxes(GetUserWithFriendQuery query);
        Task<IEnumerable<QuestionToFriendDto>> GetUserWithFriendQuestion(GetUserWithFriendQuery query);
        Task<IEnumerable<AnswerToFriendDto>> GetUserWithFriendAnswer(GetUserWithFriendQuery query);
        Task<IEnumerable<UserInviteDto>> GetUserPersonalInvites(GetInvitesQuery query);


        Task<UserToFriendActivity> GetUserWithFriendActivity(GetUserWithFriendQuery query);
        Task<IEnumerable<ItemToFriendDto>> GetUserWithFriendItemsAsync(GetUserWithFriendQuery query);
        Task<IEnumerable<ActivityDto>> GetUserWithFriendActivityAsync(GetUserWithFriendQuery query);

        Task<IEnumerable<string>> GetSeoItems(int page);
        Task<int> GetSeoItemCount();

        Task<IEnumerable<AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query);

        Task<IEnumerable<RussianDepartmentDto>> GetRussianDepartmentList(long universityId);



        Task<UniversityWithCodeDto> GetUniversityNeedId(long universityId);
        Task<bool> GetUniversityNeedCode(long universityId);


        Task<IEnumerable<UpdatesDto>> GetUpdates(QueryBase query);

        //Quiz
        Task<QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query);
        Task<QuizSeo> GetQuizSeo(GetQuizSeoQuery query);
        Task<QuizWithDetailDto> GetDraftQuiz(GetQuizDraftQuery query);
        Task<IEnumerable<DiscussionDto>> GetDiscussion(GetDisscussionQuery query);
        Task<int> GetNumberOfSolvers(long quizId);

        Task<IEnumerable<RecommendBoxDto>> GetRecommendedCourses(RecommendedCoursesQuery query);

        Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddress(UniversityByIpQuery query);
        Task<IEnumerable<UserWithImageNameDto>> GetUsersByTerm(UniversitySearchQuery query);

    }
}
