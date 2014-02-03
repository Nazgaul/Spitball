using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Tags;

namespace Zbang.Zbox.Mvc3WebRole.Factories
{
    public static class TagsQueryFactory
    {
        static IShortCodesCache m_ShortCodesCache;
        static IZboxReadService m_ZboxReadService;
        readonly static Dictionary<Query, Func<long, string, string[], string, int, GetTagsQueryBase>> m_Queries = new Dictionary<Query, Func<long, string, string[], string, int, GetTagsQueryBase>>()
       {
             {Query.UserTags,CreateUserTagsQuery},
             {Query.FriendTags,CreateFriendTagsQuery},
             {Query.UserRelevantTags, CreateUserRelevantTagQuery},
             {Query.FriendRelevantTags, CreateFriendRelevantTagQuery},
             //{Query.UserTagsSearch,CreateUserTagsWithSearchQuery},
             //{Query.UserRelevantTagsSearch,CreateUserRelevantTagsWithSearchQuery},
             {Query.UserTagsSearch,CreateUserTagsQuery},
             {Query.UserRelevantTagsSearch,CreateUserRelevantTagQuery},
             {Query.FriendTagsSearch,CreateFriendTagsWithSearchQuery},
             {Query.FriendRelevantTagsSearch,CreateFriendRelevantTagsWithSearchQuery}

        };
        [Flags]
        enum Query
        {
            None = 0,
            Search = 1,
            Tags = 2,
            Friend = 4,

            UserTags = None,
            FriendTags = Friend,
            UserRelevantTags = None | Tags,
            FriendRelevantTags = Friend | Tags,

            UserTagsSearch = None | Search,
            UserRelevantTagsSearch = None | Tags | Search,

            FriendTagsSearch = Friend | Search,
            FriendRelevantTagsSearch = Friend | Tags | Search
        }
        public static void Init(IShortCodesCache shortToLongCode, IZboxReadService zboxReadService)
        {
            m_ShortCodesCache = shortToLongCode;
            m_ZboxReadService = zboxReadService;
        }

        public static GetTagsQueryBase Create(long userId, string friendId, string[] tags, string search, int pageNumber)
        {
            if (m_ShortCodesCache == null)
                throw new InvalidOperationException("m_ShortCodesCache is null. Did you forget to call Init() before?");

            var decision = string.IsNullOrEmpty(friendId) ? Query.None : Query.Friend;
            decision |= string.IsNullOrWhiteSpace(search) ? Query.None : Query.Search;
            decision |= (tags == null || tags.Length == 0) ? Query.None : Query.Tags;
            return m_Queries[decision](userId, friendId, tags, search, pageNumber);
        }

        private static GetUserTagsQuery CreateUserTagsQuery(long userId, string friendId, string[] tags, string search, int pageNumber)
        {
            return new GetUserTagsQuery(userId, pageNumber);
        }
        private static GetFriendTagsQuery CreateFriendTagsQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendTagsQuery(friendId, userId, userType, pageNumber);
        }

        private static GetUserRelevantTagsQuery CreateUserRelevantTagQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            return new GetUserRelevantTagsQuery(tags.Distinct().ToArray(), userId, pageNumber);
        }
        private static GetFriendRelevantTagsQuery CreateFriendRelevantTagQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendRelevantTagsQuery(tags.Distinct().ToArray(), friendId, userId, userType, pageNumber);
        }

        private static GetUserTagsWithSearchQuery CreateUserTagsWithSearchQuery(long userId, string friendId, string[] tags, string search, int pageNumber)
        {
            return new GetUserTagsWithSearchQuery(userId, search, pageNumber);
        }

        private static GetUserRelevantTagsWithSearchQuery CreateUserRelevantTagsWithSearchQuery(long userId, string friendUid, string[] tags, string search, int pageNumber)
        {
            return new GetUserRelevantTagsWithSearchQuery(userId, search, tags, pageNumber);
        }
        private static GetFriendTagsWithSearchQuery CreateFriendTagsWithSearchQuery(long userId, string friendUId, string[] tags, string search, int pageNumber)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUId, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendTagsWithSearchQuery(friendId, userId, search, userType, pageNumber);
        }

        private static GetFriendRelevantTagsWithSearchQuery CreateFriendRelevantTagsWithSearchQuery(long userId, string friendUId, string[] tags, string search, int pageNumber)
        {
            var friendId = m_ShortCodesCache.ShortCodeToLong(friendUId, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new GetUserTypeQuery(friendId));
            return new GetFriendRelevantTagsWithSearchQuery(tags.Distinct().ToArray(), friendId, userId, search, userType, pageNumber);
        }
    }
}