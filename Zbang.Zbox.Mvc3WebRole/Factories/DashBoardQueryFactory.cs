using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.DashBoard;

namespace Zbang.Zbox.Mvc3WebRole.Factories
{
    public static class DashBoardQueryFactory
    {
        static IShortCodesCache m_ShortCodesCache;
        static IZboxReadService m_ZboxReadService;
        readonly static Dictionary<Query, Func<long, string, string[], string, GetDashBoardQueryBase>> m_Queries = new Dictionary<Query, Func<long, string, string[], string, GetDashBoardQueryBase>>()
        {
            {Query.UserBoxes,CreateUserBoxesQuery},
            {Query.FriendBoxes,CreateFriendBoxesQuery},
            {Query.UserTags, CreateUserBoxesByTagQuery},
            {Query.FriendTags, CreateFriendBoxByTagQuery},
            {Query.UserBoxesSearch, CreateUserBoxesWithSearchQuery},
            {Query.UserBoxesTagsSearch,CreateUserBoxesWithTagsAndSearchQuery},
            {Query.FriendBoxesSearch,CreateFriendBoxesWithSearchQuery},
            {Query.FriendBoxesTagsSearch,CreateFriendBoxesWithTagsAndSearchQuery}
        };
        [Flags]
        enum Query
        {
            None = 0,
            Search = 1,
            Tags = 2,
            Friend = 4,

            UserBoxes = None,
            FriendBoxes = Friend,
            UserTags = None | Tags,
            FriendTags = Friend | Tags,
            UserBoxesSearch = None | Search,
            UserBoxesTagsSearch = None | Tags | Search,

            FriendBoxesSearch = Friend | Search,
            FriendBoxesTagsSearch = Friend | Tags | Search
        }
        public static void Init(IShortCodesCache shortToLongCode, IZboxReadService zboxReadService)
        {
            m_ShortCodesCache = shortToLongCode;
            m_ZboxReadService = zboxReadService;
        }

        public static GetDashBoardQueryBase Create(long userId, string friendId, string[] tags, string search)
        {
            if (m_ShortCodesCache == null)
                throw new InvalidOperationException("m_ShortCodesCache is null. Did you forget to call Init() before?");
            if (m_ZboxReadService == null)
                throw new InvalidOperationException("m_ZboxReadService is null. Did you forget to call Init() before?");
            var decision = string.IsNullOrWhiteSpace(friendId) ? Query.None : Query.Friend;
            decision |= (tags == null || tags.Length == 0) ? Query.None : Query.Tags;
            decision |= string.IsNullOrWhiteSpace(search) ? Query.None : Query.Search;
            return m_Queries[decision](userId, friendId, tags, search);
        }

        private static GetDashBoardQuery CreateUserBoxesQuery(long userId, string friendId, string[] tags, string search)
        {
            return new GetDashBoardQuery(userId);
        }
        private static GetFriendDashBoardQuery CreateFriendBoxesQuery(long userId, string friendUid, string[] tags, string search)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendDashBoardQuery(friendId, userId,userType);
        }

        private static GetDashBoardByTagQuery CreateUserBoxesByTagQuery(long userId, string friendUid, string[] tags, string search)
        {
            return new GetDashBoardByTagQuery(tags.Distinct().ToArray(), userId);
        }
        private static GetFriendDashBoardByTagQuery CreateFriendBoxByTagQuery(long userId, string friendUid, string[] tags, string search)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendDashBoardByTagQuery(tags.Distinct().ToArray(), friendId, userId, userType);
        }

        private static GetDashBoardBySearchQuery CreateUserBoxesWithSearchQuery(long userId, string friendUid, string[] tags, string search)
        {
            return new GetDashBoardBySearchQuery(userId, search);
        }
        private static GetDashBoardByTagAndSearchQuery CreateUserBoxesWithTagsAndSearchQuery(long userId, string friendUid, string[] tags, string search)
        {
            return new GetDashBoardByTagAndSearchQuery(tags.Distinct().ToArray(), search, userId);

        }

        private static GetFriendDashBoardBySearchQuery CreateFriendBoxesWithSearchQuery(long userId, string friendUid, string[] tags, string search)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendDashBoardBySearchQuery(friendId, userId, search, userType);
        }

        private static GetFriendDashBoardByTagAndSearchQuery CreateFriendBoxesWithTagsAndSearchQuery(long userId, string friendUid, string[] tags, string search)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendDashBoardByTagAndSearchQuery(tags.Distinct().ToArray(), search, friendId, userId, userType);
        }
    }
}