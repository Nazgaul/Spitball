using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Zbox.Mvc3WebRole.Factories
{
    public static class BoxesQueryFactory
    {
        static IShortCodesCache m_ShortCodesCache;
        static IZboxReadService m_ZboxReadService;

        readonly static Dictionary<Query, Func<long, string, string[], string, int, GetBoxesQueryBase>> m_Queries = new Dictionary<Query, Func<long, string, string[], string, int, GetBoxesQueryBase>>()
        {
            {Query.UserBoxes,CreateUserBoxesQuery},
            {Query.FriendBoxes,CreateFriendBoxesQuery},
            {Query.UserTags, CreateUserBoxesByTagQuery},
            {Query.FriendTags, CreateFriendBoxByTagQuery},
            {Query.UserBoxesSearch, CreateUserBoxesQuery},
            {Query.UserBoxesTagsSearch,CreateUserBoxesByTagQuery},
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

        public static GetBoxesQueryBase Create(long userId, string friendId, string[] tags, string search, int pageNumber)
        {
            if (m_ShortCodesCache == null)
                throw new InvalidOperationException("m_ShortCodesCache is null. Did you forget to call Init() before?");

            var decision = string.IsNullOrWhiteSpace(friendId) ? Query.None : Query.Friend;
            decision |= (tags == null || tags.Length == 0) ? Query.None : Query.Tags;
            decision |= string.IsNullOrWhiteSpace(search) ? Query.None : Query.Search;
            return m_Queries[decision](userId, friendId, tags, search, pageNumber);
        }

        private static GetBoxesQuery CreateUserBoxesQuery(long userId, string friendId, string[] tags, string search, int pageNumber)
        {
            return new GetBoxesQuery(userId, pageNumber);
        }
        private static GetFriendBoxesQuery CreateFriendBoxesQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendBoxesQuery(friendId, userId, userType, pageNumber);
        }

        private static GetBoxesByTagQuery CreateUserBoxesByTagQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            return new GetBoxesByTagQuery(tags.Distinct().ToArray(), userId, pageNumber);
        }
        private static GetFriendBoxesByTagQuery CreateFriendBoxByTagQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendBoxesByTagQuery(tags.Distinct().ToArray(), friendId, userId, userType, pageNumber);
        }

        private static GetBoxesBySearchQuery CreateUserBoxesWithSearchQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            return new GetBoxesBySearchQuery(userId, search, pageNumber);
        }
        private static GetBoxesByTagAndSearchQuery CreateUserBoxesWithTagsAndSearchQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            return new GetBoxesByTagAndSearchQuery(tags.Distinct().ToArray(), search, userId, pageNumber);

        }

        private static GetFriendBoxesBySearchQuery CreateFriendBoxesWithSearchQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendBoxesBySearchQuery(friendId, userId, search, userType, pageNumber);
        }

        private static GetFriendBoxesByTagAndSearchQuery CreateFriendBoxesWithTagsAndSearchQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendBoxesByTagAndSearchQuery(tags.Distinct().ToArray(), search, friendId, userId, userType, pageNumber);
        }

    }
}