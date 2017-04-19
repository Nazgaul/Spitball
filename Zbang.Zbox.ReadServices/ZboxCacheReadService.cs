using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.ViewModel.Dto.ActivityDtos;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Dashboard;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.JaredDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Dto.Qna;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Dashboard;
using Zbang.Zbox.ViewModel.Queries.Jared;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using UserDto = Zbang.Zbox.ViewModel.Dto.UserDtos.UserDto;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxCacheReadService : IZboxCacheReadService
    {
        private readonly IZboxReadService m_ReadService;
        private readonly IWithCache m_Cache;
        public ZboxCacheReadService(IZboxReadService readService, IWithCache cache)
        {
            m_ReadService = readService;
            m_Cache = cache;
        }



        public Task<IEnumerable<BoxDto>> GetUserBoxesAsync(GetBoxesQuery query)
        {
            return m_ReadService.GetUserBoxesAsync(query);
        }

        public Task<UniversityDashboardInfoDto> GetUniversityInfoAsync(UniversityQuery query)
        {
            return m_Cache.QueryAsync(m_ReadService.GetUniversityInfoAsync, query);
        }

        public Task<NodeBoxesDto> GetLibraryNodeAsync(GetLibraryNodeQuery query)
        {
            //cant do cache due to userid pass.
            //return m_Cache.QueryAsync(m_ReadService.GetLibraryNodeAsync, query);
            return m_ReadService.GetLibraryNodeAsync(query);
        }

        public Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query)
        {
            return m_ReadService.GetBoxItemsPagedAsync(query);
        }
        public Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetWebServiceBoxItemsPagedAsync(GetBoxItemsPagedQuery query)
        {
            return m_ReadService.GetWebServiceBoxItemsPagedAsync(query);
        }
        public Task<IEnumerable<QuizDto>> GetBoxQuizesAsync(GetBoxQuizesPagedQuery query)
        {
            return m_ReadService.GetBoxQuizesAsync(query);
        }

        public Task<IEnumerable<FlashcardDto>> GetBoxFlashcardsAsync(GetFlashCardsQuery query)
        {
            return m_ReadService.GetBoxFlashcardsAsync(query);
        }


        public Task<ItemDetailDto> GetItem2Async(GetItemQuery query)
        {
            return m_ReadService.GetItem2Async(query);
        }

        public Task<IEnumerable<AnnotationDto>> GetItemCommentsAsync(ItemCommentQuery query)
        {
            return m_Cache.QueryAsync(m_ReadService.GetItemCommentsAsync, query);
        }

        public Task<BoxDto2> GetBox2Async(GetBoxQuery query)
        {
            return m_ReadService.GetBox2Async(query);
        }
        public Task<BoxDtoWithMembers> GetBoxMetaWithMembersAsync(GetBoxQuery query, int numberOfMembers)
        {
            return m_ReadService.GetBoxMetaWithMembersAsync(query, numberOfMembers);
        }

        public Task<IEnumerable<TabDto>> GetBoxTabsAsync(GetBoxQuery query)
        {
            return m_ReadService.GetBoxTabsAsync(query);
        }



        public Task<IEnumerable<UserDto>> GetUserFriendsAsync(GetUserFriendsQuery query)
        {
            return m_ReadService.GetUserFriendsAsync(query);
        }



        public Task<UserAccountDto> GetUserAccountDetailsAsync(QueryBaseUserId query)
        {
            return m_ReadService.GetUserAccountDetailsAsync(query);
        }

        public Task<UserNotification> GetUserBoxesNotificationAsync(QueryBaseUserId query)
        {
            return m_ReadService.GetUserBoxesNotificationAsync(query);
        }

        public Task<NotificationSetting> GetUserBoxNotificationSettingsAsync(GetBoxQuery query, long userId)
        {
            return m_ReadService.GetUserBoxNotificationSettingsAsync(query, userId);
        }



        public Task<IEnumerable<UserMemberDto>> GetBoxMembersAsync(GetBoxQuery query)
        {
            return m_ReadService.GetBoxMembersAsync(query);
        }

        public Task<UserMinProfile> GetUserMinProfileAsync(GetUserMinProfileQuery query)
        {
            return m_ReadService.GetUserMinProfileAsync(query);
        }


        public Task<IEnumerable<CommentDto>> GetCommentsAsync(GetBoxQuestionsQuery query)
        {
            return m_Cache.QueryAsync(m_ReadService.GetCommentsAsync, query);
        }
        public Task<IEnumerable<ReplyDto>> GetRepliesAsync(GetCommentRepliesQuery query)
        {
            return m_Cache.QueryAsync(m_ReadService.GetRepliesAsync, query);
            //return m_ReadService.GetRepliesAsync(query);
        }

        public Task<IEnumerable<LikeDto>> GetCommentLikesAsync(GetFeedLikesQuery query)
        {
            return m_ReadService.GetCommentLikesAsync(query);
        }

        public Task<IEnumerable<LikeDto>> GetReplyLikesAsync(GetFeedLikesQuery query)
        {
            return m_ReadService.GetReplyLikesAsync(query);
        }

        public Task<IEnumerable<Guid>> GetUserFeedLikesAsync(UserLikesQuery query)
        {
            return m_ReadService.GetUserFeedLikesAsync(query);
        }

        public Task<CommentDto> GetCommentAsync(GetQuestionQuery query)
        {
            return m_ReadService.GetCommentAsync(query);
        }

        public Task<bool> GetInviteAsync(GetInviteDetailQuery query)
        {
            return m_ReadService.GetInviteAsync(query);
        }



        public Task<LogInUserDto> GetUserDetailsByMembershipId(GetUserByMembershipQuery query)
        {
            return m_ReadService.GetUserDetailsByMembershipId(query);
        }

        public Task<LogInUserDto> GetUserDetailsByFacebookId(GetUserByFacebookQuery query)
        {
            return m_ReadService.GetUserDetailsByFacebookId(query);
        }
        public Task<LogInUserDto> GetUserDetailsByGoogleIdAsync(GetUserByGoogleQuery query, CancellationToken cancellationToken)
        {
            return m_ReadService.GetUserDetailsByGoogleIdAsync(query, cancellationToken);
        }

        public Task<LogInUserDto> GetUserDetailsByEmail(GetUserByEmailQuery query)
        {
            return m_ReadService.GetUserDetailsByEmail(query);
        }

        public Task<IEnumerable<BoxDto>> GetUserBoxesActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserBoxesActivityAsync(query);
        }

        public Task<IEnumerable<UpdatesDto>> GetUpdatesAsync(QueryBase query)
        {
            return m_ReadService.GetUpdatesAsync(query);
        }


        public Task<QuizWithDetailSolvedDto> GetQuizAsync(GetQuizQuery query)
        {
            return m_ReadService.GetQuizAsync(query);
        }

        public Task<QuizSeo> GetQuizSeoAsync(GetQuizSeoQuery query)
        {
            return m_ReadService.GetQuizSeoAsync(query);
        }


        public Task<QuizWithDetailDto> GetDraftQuizAsync(GetQuizDraftQuery query)
        {
            return m_ReadService.GetDraftQuizAsync(query);
        }


        public Task<IEnumerable<DiscussionDto>> GetDiscussionAsync(GetDisscussionQuery query)
        {
            return m_ReadService.GetDiscussionAsync(query);
        }


        public Task<UniversityWithCodeDto> GetUniversityNeedIdAsync(long universityId)
        {
            return m_ReadService.GetUniversityNeedIdAsync(universityId);
        }


        public Task<IEnumerable<ActivityDto>> GetUserCommentActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserCommentActivityAsync(query);
        }

        public Task<IEnumerable<SeoDto>> GetSeoItemsAsync(SeoType type, int page, CancellationToken token)
        {
            return m_ReadService.GetSeoItemsAsync(type, page, token);
        }

        public Task<IEnumerable<SitemapDto>> GetSeoItemCountAsync()
        {
            return m_ReadService.GetSeoItemCountAsync();
        }

        public Task<int> GetNumberOfSolversAsync(long quizId)
        {
            return m_ReadService.GetNumberOfSolversAsync(quizId);
        }





        public Task<FileSeo> GetItemSeoAsync(GetFileSeoQuery query)
        {
            return m_ReadService.GetItemSeoAsync(query);
        }

        public Task<FlashcardSeoDto> GetFlashcardUrlAsync(GetFlashcardSeoQuery query)
        {
            return m_ReadService.GetFlashcardUrlAsync(query);
        }


        public Task<BoxSeoDto> GetBoxSeoAsync(GetBoxIdQuery query)
        {
            return m_ReadService.GetBoxSeoAsync(query);
        }


        public Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoardAsync(GetBoxLeaderboardQuery query)
        {
            return m_ReadService.GetBoxLeaderBoardAsync(query);
        }


        public Task<UserDetailDto> GetUserDataAsync(QueryBaseUserId query)
        {
            return m_ReadService.GetUserDataAsync(query);
        }


        public Task<ItemMobileDto> GetItemDetailApiAsync(GetItemQuery query)
        {
            return m_ReadService.GetItemDetailApiAsync(query);
        }


        public Task<LogInUserDto> GetUserDetailsById(GetUserByIdQuery query)
        {
            return m_ReadService.GetUserDetailsById(query);
        }


        public Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddressAsync(UniversityByIpQuery query)
        {
            return m_ReadService.GetUniversityByIpAddressAsync(query);
        }


        public Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetUserItemsActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserItemsActivityAsync(query);
        }
        public Task<IEnumerable<QuizDto>> GetUserQuizActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserQuizActivityAsync(query);
        }

        public Task<IEnumerable<FlashcardDto>> GetUserFlashcardActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserFlashcardActivityAsync(query);
        }


        public Task<IEnumerable<UserImageNameDto>> GetUsersInBoxByTermAsync(UserInBoxSearchQuery query)
        {
            return m_ReadService.GetUsersInBoxByTermAsync(query);
        }

        public Task<IEnumerable<ChatUserDto>> GetUsersConversationAndFriendsAsync(GetUserConversationAndFriends query)
        {
            return m_ReadService.GetUsersConversationAndFriendsAsync(query);
        }

        public Task<IEnumerable<ChatDto>> GetUserConversationAsync(GetChatRoomMessagesQuery query)
        {
            return m_ReadService.GetUserConversationAsync(query);
        }

        public Task<int> GetChatUnreadMessagesAsync(QueryBaseUserId query)
        {
            return m_ReadService.GetChatUnreadMessagesAsync(query);
        }

        public Task<IEnumerable<SmallNodeDto>> GetUniversityNodesAsync(long universityId)
        {
            return m_ReadService.GetUniversityNodesAsync(universityId);
        }

        public Task<FlashcardUserDto> GetUserFlashcardAsync(GetUserFlashcardQuery query)
        {
            return m_ReadService.GetUserFlashcardAsync(query);
        }

        public Task<GamificationBoardDto> GamificationBoardAsync(QueryBaseUserId query)
        {
            return m_ReadService.GamificationBoardAsync(query);
        }
        public Task<LevelDto> UserLevelsAsync(QueryBaseUserId query)
        {
            return m_ReadService.UserLevelsAsync(query);
        }

        public Task<IEnumerable<BadgeDto>> UserBadgesAsync(QueryBaseUserId query)
        {
            return m_ReadService.UserBadgesAsync(query);
        }

        public Task<IEnumerable<LeaderBoardDto>> UserLeaderboardAsync(LeaderboardQuery query)
        {
            return m_ReadService.UserLeaderboardAsync(query);
        }

        public Task<QuizSolversWithCountDto> GetQuizSolversAsync(GetQuizBestSolvers query)
        {
            return m_ReadService.GetQuizSolversAsync(query);
        }


        public Task<QuizQuestionWithSolvedAnswersDto> GetQuizQuestionWithAnswersAsync(GetQuizQuery query)
        {
            return m_ReadService.GetQuizQuestionWithAnswersAsync(query);
        }

        public Task<UserWithStats> GetUserProfileWithStatsAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserProfileWithStatsAsync(query);
        }

        public Task<ForgotPasswordDto> GetForgotPasswordByEmailAsync(GetUserByEmailQuery query, CancellationToken token)
        {
            return m_ReadService.GetForgotPasswordByEmailAsync(query, token);
        }

        public Task<HomePageDataDto> GetHomePageDataAsync(GetHomePageQuery query)
        {
            return m_ReadService.GetHomePageDataAsync(query);
        }

        public Task<IEnumerable<RecommendBoxDto>> GetUniversityBoxesAsync(GetHomeBoxesUniversityQuery query)
        {
            return m_Cache.QueryAsync(m_ReadService.GetUniversityBoxesAsync, query);
        }

        public long? GetUniversityIdByUrl(string url)
        {
            return m_ReadService.GetUniversityIdByUrl(url);
        }

        public Task<IEnumerable<RecommendBoxDto>> GetCoursesPageDataAsync()
        {
            return m_ReadService.GetCoursesPageDataAsync();
        }

        public Task<IEnumerable<ClosedNodeDto>> GetUserClosedDepartmentAsync(QueryBase query)
        {
            return m_ReadService.GetUserClosedDepartmentAsync(query);
        }

        public Task<IEnumerable<ClosedNodeUsersDto>> GetMembersClosedDepartmendAsync(GetClosedNodeMembersQuery query)
        {
            return m_ReadService.GetMembersClosedDepartmendAsync(query);
        }

        public Task<JaredDto> GetJaredStartupValuesAsync(CancellationToken token)
        {
            return m_ReadService.GetJaredStartupValuesAsync(token);
        }

        public Task<IEnumerable<UserImageNameDto>> OnlineUsersByClassAsync(GetBoxIdQuery query)
        {
            return m_ReadService.OnlineUsersByClassAsync(query);
        }
        public Task<IEnumerable<ItemTagsDto>> GetItemsWithTagsAsync(JaredSearchQuery query)
        {
            return m_ReadService.GetItemsWithTagsAsync(query);
        }
        public Task<IEnumerable<JaredTextDto>> GetJaredText()
        {
            return m_ReadService.GetJaredText();
        }
        public Task<IEnumerable<string>> GetUniAsync(SearchTermQuery term) {
            return m_ReadService.GetUniAsync(term);
        }
        public Task<IEnumerable<string>> GetDepartmentAsync(SearchTermQuery term) {
            return m_ReadService.GetDepartmentAsync(term);
        }
        public Task<IEnumerable<string>> GetTagAsync(SearchTermQuery term)
        {
            return m_ReadService.GetTagAsync(term);
        }
    }
}
