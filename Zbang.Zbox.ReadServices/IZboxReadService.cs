﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
using BoxDto = Zbang.Zbox.ViewModel.Dto.BoxDto;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadService : IBaseReadService
    {
        // home page
        Task<HomePageDataDto> GetHomePageDataAsync(GetHomePageQuery query);
        Task<IEnumerable<RecommendBoxDto>> GetUniversityBoxesAsync(GetHomeBoxesUniversityQuery query);
        long? GetUniversityIdByUrl(string url);

        Task<string> GetUniversitySynonymAsync(long id);

        Task<IEnumerable<RecommendBoxDto>> GetCoursesPageDataAsync();
        Task<IEnumerable<BoxDto>> GetUserBoxesAsync(GetBoxesQuery query);
        Task<IEnumerable<string>> GetUniAsync(SearchTermQuery term);
        Task<IEnumerable<string>> GetDepartmentAsync(SearchTermQuery term);
        Task<IEnumerable<string>> GetTagAsync(SearchTermQuery term);

        Task<UniversityDashboardInfoDto> GetUniversityInfoAsync(UniversityQuery query);

        Task<NodeBoxesDto> GetLibraryNodeAsync(GetLibraryNodeQuery query);
        Task<IEnumerable<ClosedNodeDto>> GetUserClosedDepartmentAsync(QueryBase query);
        Task<IEnumerable<ClosedNodeUsersDto>> GetMembersClosedDepartmentAsync(GetClosedNodeMembersQuery query);

        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query);
        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetWebServiceBoxItemsPagedAsync(GetBoxItemsPagedQuery query);
        Task<IEnumerable<QuizDto>> GetBoxQuizzesAsync(GetBoxQuizesPagedQuery query);

        Task<IEnumerable<FlashcardDto>> GetBoxFlashcardsAsync(GetFlashCardsQuery query);
        Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoardAsync(GetBoxLeaderboardQuery query);

        Task<ItemDetailDto> GetItem2Async(GetItemQuery query);
        Task<IEnumerable<AnnotationDto>> GetItemCommentsAsync(ItemCommentQuery query);
        Task<ItemMobileDto> GetItemDetailApiAsync(long itemId);
        Task<FileSeo> GetItemSeoAsync(GetFileSeoQuery query);
        Task<FlashcardSeoDto> GetFlashcardUrlAsync(GetFlashcardSeoQuery query);
        Task<BoxSeoDto> GetBoxSeoAsync(GetBoxIdQuery query);

        Task<BoxDto2> GetBox2Async(GetBoxQuery query);

        Task<BoxDtoWithMembers> GetBoxMetaWithMembersAsync(GetBoxQuery query, int numberOfMembers);
        Task<IEnumerable<TabDto>> GetBoxTabsAsync(GetBoxQuery query);

        Task<IEnumerable<UserDto>> GetUserFriendsAsync(GetUserFriendsQuery query);

        Task<UserDetailDto> GetUserDataAsync(QueryBaseUserId query);

        Task<UserAccountDto> GetUserAccountDetailsAsync(QueryBaseUserId query);
        Task<UserNotification> GetUserBoxesNotificationAsync(QueryBaseUserId query);
        Task<NotificationSetting> GetUserBoxNotificationSettingsAsync(GetBoxQuery query, long userId);

        Task<IEnumerable<UserMemberDto>> GetBoxMembersAsync(GetBoxQuery query);

        Task<UserMinProfile> GetUserMinProfileAsync(GetUserMinProfileQuery query);

        Task<IEnumerable<CommentDto>> GetCommentsAsync(GetBoxQuestionsQuery query);
        Task<IEnumerable<ReplyDto>> GetRepliesAsync(GetCommentRepliesQuery query);

        Task<IEnumerable<LikeDto>> GetCommentLikesAsync(GetFeedLikesQuery query);
        Task<IEnumerable<LikeDto>> GetReplyLikesAsync(GetFeedLikesQuery query);
        Task<IEnumerable<Guid>> GetUserFeedLikesAsync(UserLikesQuery query);
        Task<CommentDto> GetCommentAsync(GetQuestionQuery query);
        Task<bool> GetInviteAsync(GetInviteDetailQuery query);

        //user page
        Task<IEnumerable<BoxDto>> GetUserBoxesActivityAsync(GetUserWithFriendQuery query);

        Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetUserItemsActivityAsync(GetUserWithFriendQuery query);
        Task<IEnumerable<ActivityDto>> GetUserCommentActivityAsync(GetUserWithFriendQuery query);
        Task<IEnumerable<QuizDto>> GetUserQuizActivityAsync(GetUserWithFriendQuery query);
        Task<IEnumerable<FlashcardDto>> GetUserFlashcardActivityAsync(GetUserWithFriendQuery query);
        Task<UserWithStats> GetUserProfileWithStatsAsync(GetUserStatsQuery query);

        Task<IEnumerable<SeoDto>> GetSeoItemsAsync(SeoType type, int page, CancellationToken token);
        Task<IEnumerable<SitemapDto>> GetSeoItemCountAsync();

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

        string GetCountryByIp(long ip);
        Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddressAsync(UniversityByIpQuery query);
        Task<IEnumerable<UserImageNameDto>> GetUsersInBoxByTermAsync(UserInBoxSearchQuery query);

        #region chat
        Task<IEnumerable<ChatUserDto>> GetUsersConversationAndFriendsAsync(GetUserConversationAndFriends query);
        Task<IEnumerable<ChatDto>> GetUserConversationAsync(GetChatRoomMessagesQuery query);
        Task<int> GetChatUnreadMessagesAsync(QueryBaseUserId query);
        #endregion
        Task<IEnumerable<SmallNodeDto>> GetUniversityNodesAsync(long universityId);

        #region flashcard
        Task<FlashcardUserDto> GetUserFlashcardAsync(GetUserFlashcardQuery query);
        #endregion

        #region Gamification

        Task<GamificationBoardDto> GamificationBoardAsync(QueryBaseUserId query);
        Task<LevelDto> UserLevelsAsync(QueryBaseUserId query);

        Task<IEnumerable<BadgeDto>> UserBadgesAsync(QueryBaseUserId query);
        Task<IEnumerable<LeaderBoardDto>> UserLeaderBoardAsync(LeaderboardQuery query);

        #endregion

        #region Jared

        //Task<JaredDto> GetJaredStartupValuesAsync(CancellationToken token,QueryBaseUserId query);
        Task<JaredDto> GetJaredStartupValuesAsync(CancellationToken token);

        Task<Tuple<UserDetailDto, IEnumerable<BoxDto>>> GetJaredUserDataAsync(
            QueryBaseUserId query, CancellationToken token);
        Task<IEnumerable<ChatUserDto>> OnlineUsersByClassAsync(GetBoxIdQuery query);
        Task<IEnumerable<ItemTagsDto>> GetItemsWithTagsAsync(JaredSearchQuery query);

        Task<JaredFavoriteDto> JaredFavoritesAsync(JaredFavoritesQuery query);

        #endregion

    }
}
