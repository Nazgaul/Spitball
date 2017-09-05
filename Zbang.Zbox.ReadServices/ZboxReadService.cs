using System.Reflection;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Query;
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.ViewModel.Dto.Dashboard;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Dashboard;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using Activity = Zbang.Zbox.ViewModel.Dto.ActivityDtos;
using Box = Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Item = Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Qna = Zbang.Zbox.ViewModel.Dto.Qna;
using User = Zbang.Zbox.ViewModel.Dto.UserDtos;
using Sql = Zbang.Zbox.ViewModel.SqlQueries;
using Zbang.Zbox.ViewModel.Dto.JaredDtos;
using Zbang.Zbox.ViewModel.Queries.Jared;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadService : BaseReadService, IZboxReadService, IUniversityWithCode
    {
        public async Task<HomePageDataDto> GetHomePageDataAsync(GetHomePageQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var sql = Sql.HomePage.Stats;
                if (query.UniversityId.HasValue)
                {
                    sql += Sql.HomePage.UniversityColors;
                }
                using (var grid = await conn.QueryMultipleAsync(sql
                    , query).ConfigureAwait(false))
                {
                    var retVal = new HomePageDataDto
                    {
                        HomePageStats = await grid.ReadFirstAsync<HomePageStats>().ConfigureAwait(false)
                    };
                    if (grid.IsConsumed) return retVal;
                    retVal.HomePageUniversityData = await grid.ReadFirstAsync<HomePageUniversityData>().ConfigureAwait(false);
                    return retVal;
                }
            }
        }

        public long? GetUniversityIdByUrl(string url)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.QuerySingleOrDefault<long?>("select id from zbox.university where url=@url", new { url });
            }
        }

        public async Task<string> GetUniversitySynonymAsync(long id)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QuerySingleOrDefaultAsync<string>("select coalesce( url, UniversityName) from zbox.university where id=@id", new { id }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Box.RecommendBoxDto>> GetUniversityBoxesAsync(GetHomeBoxesUniversityQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return
                    await
                        conn.QueryAsync<Box.RecommendBoxDto>(Sql.HomePage.UniversityBoxes,
                            query).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Box.RecommendBoxDto>> GetCoursesPageDataAsync()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var fieldInfo = Array.Find(typeof(Sql.Sql).GetFields(BindingFlags.Public | BindingFlags.Static
                                            | BindingFlags.FlattenHierarchy), fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name == "GetCoursesPageBoxes_" + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower());

                var coursesQuery = Sql.Sql.GetCoursesPageBoxes_en;
                if (fieldInfo != null)
                {
                    coursesQuery = fieldInfo.GetValue(null).ToString();
                }
                return await conn.QueryAsync<Box.RecommendBoxDto>(coursesQuery).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// used to get the dashboard and the activity and wall in dashboard
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BoxDto>> GetUserBoxesAsync(GetBoxesQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<BoxDto>(Sql.Dashboard.UserBoxes,
                    query).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// used in dashboard to get user lib in top left
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<UniversityDashboardInfoDto> GetUniversityInfoAsync(UniversityQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryFirstAsync<UniversityDashboardInfoDto>(Sql.Sql.UniversityInfo,
                    query).ConfigureAwait(false);
                return retVal;
            }
        }

        /// <summary>
        /// University page - sub nodes or boxes in the current node
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<NodeBoxesDto> GetLibraryNodeAsync(GetLibraryNodeQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var sql = query.ParentNode.HasValue ?
                    Sql.Library.GetLibraryNodeWithParent +
                    Sql.Library.GetAcademicBoxesByNode + Sql.Library.GetLibraryNodeDetails : Sql.Library.GetLibraryNode;

                using (var grid = await conn.QueryMultipleAsync(sql, new
                {
                    ParentId = query.ParentNode,
                    query.UniversityId,
                    query.UserId,
                    LibraryId = query.ParentNode
                }).ConfigureAwait(false))
                {
                    var retVal = new NodeBoxesDto
                    {
                        Nodes = await grid.ReadAsync<NodeDto>().ConfigureAwait(false)
                    };

                    if (grid.IsConsumed) return retVal;
                    retVal.Boxes = await grid.ReadAsync<BoxDto>().ConfigureAwait(false);
                    retVal.Details = await grid.ReadFirstAsync<NodeDetails>().ConfigureAwait(false);
                    if (retVal.Details.State == LibraryNodeSetting.Closed)
                    {
                        if (retVal.Details.UserType == UserLibraryRelationType.Pending
                            || retVal.Details.UserType == UserLibraryRelationType.None)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }
                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<ClosedNodeDto>> GetUserClosedDepartmentAsync(QueryBase query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal =
                    await conn.QueryAsync<ClosedNodeDto>(Sql.Library.GetClosedLibraryByUser, query).ConfigureAwait(false);

                return retVal;
            }
        }

        public async Task<IEnumerable<ClosedNodeUsersDto>> GetMembersClosedDepartmentAsync(GetClosedNodeMembersQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal =
                   await conn.QueryAsync<ClosedNodeUsersDto>(Sql.Library.GetClosedLibraryUsers,
                   query).ConfigureAwait(false);

                return retVal;
            }
        }

        public async Task<Box.BoxDto2> GetBox2Async(GetBoxQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<Box.BoxDto2>(Sql.Box.BoxData, query).ConfigureAwait(false);
                var box = retVal;
                if (box == null)
                {
                    throw new BoxDoesntExistException();
                }
                return box;
            }
        }

        public async Task<Box.BoxDtoWithMembers> GetBoxMetaWithMembersAsync(GetBoxQuery query, int numberOfMembers)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(
                    $"{Sql.Box.BoxData} {Sql.Box.BoxMembersWithoutInvited}", new
                    {
                        query.BoxId,
                        top = numberOfMembers
                    }).ConfigureAwait(false))
                {
                    var box = await grid.ReadFirstOrDefaultAsync<Box.BoxDtoWithMembers>().ConfigureAwait(false);
                    if (box == null)
                    {
                        throw new BoxDoesntExistException();
                    }
                    box.People = await grid.ReadAsync<User.UserWithImageDto>().ConfigureAwait(false);
                    return box;
                }
            }
        }

        public async Task<IEnumerable<TabDto>> GetBoxTabsAsync(GetBoxQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<TabDto>(Sql.Box.BoxTabs, query).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get user notification to the box in question - notification pop up
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<NotificationSetting> GetUserBoxNotificationSettingsAsync(GetBoxQuery query, long userId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string getBoxNotification =
                    "select NotificationSettings from zbox.UserBoxRel where userId = @UserId and boxId = @BoxId";
                var val = await conn.QueryFirstOrDefaultAsync<NotificationSetting>(getBoxNotification, new
                {
                    UserId = userId,
                    query.BoxId
                }).ConfigureAwait(false);
                return val;
            }
        }

        /// <summary>
        /// Used in box page - get the item related to that page
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Item.ItemDto>> GetWebServiceBoxItemsPagedAsync(GetBoxItemsPagedQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<Item.ItemDto>(Sql.Box.Items, new
                {
                    query.BoxId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage,
                    query.TabId
                }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Item.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                if (!query.TabId.HasValue)
                {
                    return await conn.QueryAsync<Item.ItemDto>(Sql.Box.ItemsWithoutTabs, new
                    {
                        query.BoxId,
                        pageNumber = query.PageNumber,
                        rowsperpage = query.RowsPerPage,
                    }).ConfigureAwait(false);
                }
                return await conn.QueryAsync<Item.ItemDto>(Sql.Box.Items, new
                {
                    query.BoxId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage,
                    query.TabId
                }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Item.QuizDto>> GetBoxQuizzesAsync(GetBoxQuizesPagedQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<Item.QuizDto>(Sql.Quiz.GetBoxQuiz, query).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Item.FlashcardDto>> GetBoxFlashcardsAsync(GetFlashCardsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<Item.FlashcardDto>(Sql.Box.Flashcards, new
                {
                    query.BoxId,
                }).ConfigureAwait(false);
            }
        }

        public async Task<Item.ItemMobileDto> GetItemDetailApiAsync(long itemId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryFirstOrDefaultAsync<Item.ItemMobileDto>(Sql.Item.ItemDetailApi, new { ItemId = itemId }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Activity.AnnotationDto>> GetItemCommentsAsync(ItemCommentQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync($"{Sql.Item.ItemComments} {Sql.Item.ItemCommentReply}",
                    query).ConfigureAwait(false))
                {
                    var retVal = (await grid.ReadAsync<Activity.AnnotationDto>().ConfigureAwait(false)).ToList();

                    IEnumerable<Activity.AnnotationReplyDto> replies =
                        (await grid.ReadAsync<Activity.AnnotationReplyDto>().ConfigureAwait(false)).ToList();

                    foreach (var comment in retVal)
                    {
                        comment.Replies.AddRange(replies.Where(w => w.ParentId == comment.Id));
                    }
                    return retVal;
                }
            }
        }

        public async Task<Item.ItemDetailDto> GetItem2Async(GetItemQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (
                    var grid =
                        await
                            conn.QueryMultipleAsync(
                                $"{Sql.Item.ItemDetail} {Sql.Item.Navigation}  {Sql.Item.UserItemRate}",
                                query).ConfigureAwait(false))
                {
                    var retVal = await grid.ReadFirstOrDefaultAsync<Item.ItemDetailDto>().ConfigureAwait(false);
                    if (retVal == null)
                    {
                        throw new ItemNotFoundException();
                    }
                    retVal.Navigation = await grid.ReadFirstOrDefaultAsync<Item.ItemNavigationDto>().ConfigureAwait(false);

                    retVal.Like = await grid.ReadFirstOrDefaultAsync<int>().ConfigureAwait(false);
                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoardAsync(GetBoxLeaderboardQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await con.QueryAsync<LeaderBoardDto>(Sql.Box.LeaderBoard,
                    query).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get the comment in box
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>. result
        /// </returns>
        public async Task<IEnumerable<Qna.CommentDto>> GetCommentsAsync(GetBoxQuestionsQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await con.QueryMultipleAsync(
                    $"{Sql.Feed.Comments} {Sql.Feed.RepliesInComments} {Sql.Feed.GetItemsInCommentsAndReplies} {Sql.Feed.GetQuizzesForComments} {Sql.Feed.GetFlashcardFromComments}",
                    new { query.BoxId, query.Top, query.Skip, rtop = GetBoxQuestionsQuery.TopOfReplies }).ConfigureAwait(false))
                {
                    var comments = await grid.ReadAsync<Qna.CommentDto>().ConfigureAwait(false);//.ToList();
                    var replies = await grid.ReadAsync<Qna.ReplyDto>().ConfigureAwait(false);//.ToDictionary(x => x.QuestionId);
                    var items = (await grid.ReadAsync<Qna.ItemDto>().ConfigureAwait(false))
                        .Union(await grid.ReadAsync<Qna.ItemDto>().ConfigureAwait(false))
                        .Union(await grid.ReadAsync<Qna.ItemDto>().ConfigureAwait(false));//.ToLookup(c => c.QuestionId ?? c.AnswerId);

                    replies = replies.Select(s =>
                    {
                        s.Files = items.Where(w => w.AnswerId == s.Id);
                        return s;
                    });
                    comments = comments.Select(s =>
                    {
                        s.Files = items.Where(w => w.QuestionId == s.Id);
                        s.Replies = replies.Where(w => w.QuestionId == s.Id);
                        return s;
                    });
                    return comments;
                }
            }
        }

        public async Task<Qna.CommentDto> GetCommentAsync(GetQuestionQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await con.QueryMultipleAsync(
                    $"{Sql.Box.GetCommentForMobile} {Sql.Box.GetCommentFileForMobile}",
                    query).ConfigureAwait(false))
                {
                    var comment = await grid.ReadFirstAsync<Qna.CommentDto>().ConfigureAwait(false);
                    comment.Files = await grid.ReadAsync<Qna.ItemDto>().ConfigureAwait(false);
                    return comment;
                }
            }
        }

        public async Task<IEnumerable<Qna.LikeDto>> GetReplyLikesAsync(GetFeedLikesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = "select UserImageLarge as Image ,UserName as Name,Url from zbox.ReplyLike join zbox.Users on OwnerId = userId where replyId = @Id";
                return await con.QueryAsync<Qna.LikeDto>(sql, query).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Qna.LikeDto>> GetCommentLikesAsync(GetFeedLikesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = "select UserImageLarge as Image ,UserName as Name,Url from zbox.CommentLike join zbox.Users on OwnerId = userId where CommentId = @Id";
                return await con.QueryAsync<Qna.LikeDto>(sql, query).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Guid>> GetUserFeedLikesAsync(UserLikesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string commentSql = @"select commentId from zbox.CommentLike
where ownerId = @UserId and boxId = @BoxId;";

                const string replySql = @"select ReplyId from zbox.ReplyLike
where ownerId = @UserId and boxId = @BoxId;";

                using (var grid = await con.QueryMultipleAsync(commentSql + replySql, query).ConfigureAwait(false))
                {
                    var retVal = new List<Guid>();
                    retVal.AddRange(await grid.ReadAsync<Guid>().ConfigureAwait(false));
                    retVal.AddRange(await grid.ReadAsync<Guid>().ConfigureAwait(false));
                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<Qna.ReplyDto>> GetRepliesAsync(GetCommentRepliesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await con.QueryMultipleAsync(
                    $"{Sql.Feed.GetReplies} {Sql.Feed.GetItemsInReply}",
                    new { query.BoxId, query.PageNumber, query.RowsPerPage, query.CommentId, AnswerId = query.BelowReplyId }).ConfigureAwait(false))
                {
                    var replies = (await grid.ReadAsync<Qna.ReplyDto>().ConfigureAwait(false)).ToList();
                    var items = (await grid.ReadAsync<Qna.ItemDto>().ConfigureAwait(false)).ToLookup(c => c.AnswerId);

                    foreach (var reply in replies)
                    {
                        reply.Files = items[reply.Id];
                    }

                    return replies;
                }
            }
        }

        public async Task<IEnumerable<User.UserDto>> GetUserFriendsAsync(GetUserFriendsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<User.UserDto>(Sql.Sql.FriendList, new
                {
                    query.UserId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                }).ConfigureAwait(false);
            }
        }

        public async Task<User.UserMinProfile> GetUserMinProfileAsync(GetUserMinProfileQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = @"select u.userId as Id, u.username as name, u.UserImageLarge as image,
                            u.score as score, uu.universityName as universityName, u.url as Url
                            from zbox.users u left join zbox.university uu on u.UniversityId = uu.id
                            where u.userId =@UserId";
                var retVal = await conn.QueryFirstAsync<User.UserMinProfile>(sql, query).ConfigureAwait(false);
                return retVal;
            }
        }

        /// <summary>
        /// used in user page to get initial data
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<User.UserWithStats> GetUserProfileWithStatsAsync(GetUserStatsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(Sql.User.UserProfileWithStats, new
                {
                    Myfriend = query.FriendId,
                }).ConfigureAwait(false))
                {
                    var retVal = await grid.ReadFirstOrDefaultAsync<User.UserWithStats>().ConfigureAwait(false);
                    if (retVal == null) return null;
                    retVal.NumItem = await grid.ReadFirstOrDefaultAsync<int>().ConfigureAwait(false);
                    retVal.NumFeed = await grid.ReadFirstOrDefaultAsync<int>().ConfigureAwait(false) + await grid.ReadFirstOrDefaultAsync<int>().ConfigureAwait(false);
                    retVal.NumQuiz = await grid.ReadFirstOrDefaultAsync<int>().ConfigureAwait(false);
                    retVal.NumFriend = await grid.ReadFirstOrDefaultAsync<int>().ConfigureAwait(false);
                    retVal.NumFlashcard = await grid.ReadFirstOrDefaultAsync<int>().ConfigureAwait(false);
                    return retVal;
                }
            }
        }

        public string GetCountryByIp(long ip)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.QueryFirstOrDefault<string>(Sql.Sql.LocationByIp, new { IP = ip });
            }
        }

        public async Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddressAsync(UniversityByIpQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<UniversityByPrefixDto>(Sql.LibraryChoose.GetInitialValueOfUniversitiesBaseOnIpAddress,
                     new
                     {
                         IP = query.IpAddress,
                         query.PageNumber,
                         query.RowsPerPage

                     }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Used in api to private to box / invite to box
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User.UserImageNameDto>> GetUsersInBoxByTermAsync(UserInBoxSearchQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var sql = string.IsNullOrEmpty(query.Term) ? Sql.Search.GetUsersInBox : Sql.Search.GetUsersInBoxByTerm;
                return await conn.QueryAsync<User.UserImageNameDto>(sql,
                     new
                     {
                         query.Term,
                         query.PageNumber,
                         query.RowsPerPage,
                         query.UniversityId,
                         query.BoxId

                     }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<User.ChatUserDto>> GetUsersConversationAndFriendsAsync(GetUserConversationAndFriends query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var sqlQuery = string.IsNullOrEmpty(query.Term)
                    ? Sql.Chat.GetUsersConversationAndFriendsWithoutTerm
                    : Sql.Chat.GetUsersConversationAndFriends;
                var result = await conn.QueryAsync<User.ChatUserDto>(sqlQuery,
                    new
                    {
                        query.UserId,
                        query.UniversityId,
                        query.Term,
                        query.PageNumber,
                        query.RowsPerPage

                    }).ConfigureAwait(false);
                return result.Distinct(new User.ChatUserDtoComparer());
            }
        }

        public async Task<IEnumerable<ChatDto>> GetUserConversationAsync(GetChatRoomMessagesQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                if (query.Id.HasValue)
                {
                    return await conn.QueryAsync<ChatDto>(Sql.Chat.GetChat,
                        new
                        {
                            ChatRoom = query.Id,
                            creationTime = query.FromTime,
                            query.Skip,
                            query.Top

                        }).ConfigureAwait(false);
                }
                return await conn.QueryAsync<ChatDto>(Sql.Chat.GetChatByUserIds,
                        new
                        {
                            query.UserIds,
                            length = query.UserIds.Count(),
                            creationTime = query.FromTime,
                            query.Skip,
                            query.Top

                        }).ConfigureAwait(false);
            }
        }

        public async Task<UniversityWithCodeDto> GetUniversityNeedIdAsync(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<UniversityWithCodeDto>(Sql.LibraryChoose.GetNeedId, new
                {
                    universityId
                }).ConfigureAwait(false);
                return retVal;
            }
        }

        public async Task<int> GetChatUnreadMessagesAsync(QueryBaseUserId query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var result = await conn.QueryFirstOrDefaultAsync<int?>(Sql.Chat.GetUnreadMessages, query).ConfigureAwait(false);
                return result.GetValueOrDefault();
            }
        }

        public async Task<User.UserDetailDto> GetUserDataAsync(QueryBaseUserId query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var user = await conn.QueryFirstOrDefaultAsync<User.UserDetailDto>(Sql.Sql.UserAuthenticationDetail, query).ConfigureAwait(false);
                if (user == null)
                {
                    throw new UserNotFoundException("user is null");
                }
                return user;
            }
        }

        #region Account Settings
        public async Task<User.UserAccountDto> GetUserAccountDetailsAsync(QueryBaseUserId query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryFirstAsync<User.UserAccountDto>(Sql.Sql.GetUserAccountData, query).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Used in account settings notification section
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<User.UserNotification> GetUserBoxesNotificationAsync(QueryBaseUserId query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string boxNotificationSql =
                    @"select b.BoxId as Id, b.BoxName as Name,  ub.NotificationSettings as Notifications, b.Url as url, u.UserName
                    from zbox.Box b 
	                inner join zbox.UserBoxRel ub on b.BoxId = ub.BoxId
					inner join zbox.Users u on b.OwnerId = u.UserId
                    where ub.UserId = @UserId";

                const string userEmailSettings = "select EmailSendSettings from zbox.Users where userId = @UserId";
                using (var grid = await conn.QueryMultipleAsync($"{boxNotificationSql} {userEmailSettings}", query).ConfigureAwait(false))
                {
                    var retVal = new User.UserNotification
                    {
                        BoxNotifications = await grid.ReadAsync<Box.BoxNotificationDto>().ConfigureAwait(false),
                        EmailNotification = await grid.ReadFirstOrDefaultAsync<EmailSend>().ConfigureAwait(false)
                    };
                    return retVal;
                }
            }
        }

        /// <summary>
        /// Used in box page to get the members pop up
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// 
        public async Task<IEnumerable<User.UserMemberDto>> GetBoxMembersAsync(GetBoxQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<User.UserMemberDto>(Sql.Box.BoxMembers, query).ConfigureAwait(false);
            }
        }

        #endregion

        public async Task<bool> GetInviteAsync(GetInviteDetailQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sqlQuery = @"select count(*) from   zbox.Message 
                                where TypeOfMsg = 2
                                and MessageId = @MessageId
                                and isActive = 1";
                var count = await conn.QueryFirstOrDefaultAsync<int>(sqlQuery, query).ConfigureAwait(false);

                return count > 0;
            }
        }

        public async Task<IEnumerable<UpdatesDto>> GetUpdatesAsync(QueryBase query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<UpdatesDto>(Sql.Updates.GetUserUpdates, query).ConfigureAwait(false);
            }
        }

        #region UserPage
        public async Task<IEnumerable<BoxDto>> GetUserBoxesActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryAsync<BoxDto>(Sql.User.UserWithFriendBoxes,
                    new
                    {
                        //Me = query.UserId,
                        Myfriend = query.FriendId,
                        pageNumber = query.PageNumber,
                        rowsperpage = query.RowsPerPage
                    }).ConfigureAwait(false);
                return retVal;
            }
        }

        public async Task<IEnumerable<Item.QuizDto>> GetUserQuizActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<Item.QuizDto>(Sql.User.UserWithFriendQuizzes, new
                {
                    //Me = query.UserId,
                    Myfriend = query.FriendId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Item.ItemDto>> GetUserItemsActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<Item.ItemDto>(Sql.User.UserWithFriendFiles, new
                {
                    Myfriend = query.FriendId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Item.FlashcardDto>> GetUserFlashcardActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<Item.FlashcardDto>(Sql.User.UserWithFriendFlashcards, new
                {
                    Myfriend = query.FriendId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Used in mobile service api
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User.ActivityDto>> GetUserCommentActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<User.ActivityDto>(Sql.Sql.UserQuestionAndAnswersActivityMobileApi, new
                {
                    //Me = query.UserId,
                    Myfriend = query.FriendId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                }).ConfigureAwait(false);
            }
        }

        #endregion

        #region Seo
        public async Task<IEnumerable<SeoDto>> GetSeoItemsAsync(SeoType type, int page, CancellationToken token)
        {
            const int pageSize = 49950;
            string sql;
            switch (type)
            {
                case SeoType.Static:
                    return null;
                case SeoType.Course:
                    sql = Sql.Seo.GetBoxSeoByPage;
                    break;
                case SeoType.Item:
                    sql = Sql.Seo.GetItemSeoByPage;
                    break;
                case SeoType.Quiz:
                    sql = Sql.Seo.GetQuizSeoByPage;
                    break;
                case SeoType.Flashcard:
                    sql = Sql.Seo.GetFlashcardSeoByPage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            //if (page < 1)
            //{
            //    return null;
            //}
            using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
            {
                return await conn.QueryAsync<SeoDto>(new CommandDefinition(sql,
                    new { rowsperpage = pageSize, pageNumber = page },
                    commandTimeout: 360, cancellationToken: token)).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<SitemapDto>> GetSeoItemCountAsync()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var data = await conn.QueryFirstOrDefaultAsync(new CommandDefinition(Sql.Seo.GetSeoItemsCount)).ConfigureAwait(false);
                var list = new List<SitemapDto>
                {
                    new SitemapDto(SeoType.Course, data.boxcount), new SitemapDto(SeoType.Item, data.itemCount), new SitemapDto(SeoType.Quiz, data.quizCount), new SitemapDto(SeoType.Flashcard, data.flashcardCount),
                };
                return list;

                //return (retVal / pageSize) + 1;
            }
        }

        #endregion

        #region Quiz

        public async Task<Item.QuizSeo> GetQuizSeoAsync(GetQuizSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryFirstAsync<Item.QuizSeo>(Sql.Quiz.QuizSeoQuery, query).ConfigureAwait(false);
                return retVal;
            }
        }

        public async Task<Item.FileSeo> GetItemSeoAsync(GetFileSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<Item.FileSeo>(Sql.Seo.FileSeo, query).ConfigureAwait(false);
                return retVal;
            }
        }

        public async Task<Item.FlashcardSeoDto> GetFlashcardUrlAsync(GetFlashcardSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<Item.FlashcardSeoDto>(Sql.Seo.FlashcardSeo, query).ConfigureAwait(false);
                return retVal;
            }
        }

        public async Task<Box.BoxSeoDto> GetBoxSeoAsync(GetBoxIdQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<Box.BoxSeoDto>(Sql.Seo.BoxSeo, query).ConfigureAwait(false);
                var box = retVal;
                if (box == null)
                {
                    throw new BoxDoesntExistException();
                }
                return box;
            }
        }

        public async Task<Item.QuizSolversWithCountDto> GetQuizSolversAsync(GetQuizBestSolvers query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var sql = $"{Sql.Quiz.TopUsers} {Sql.Quiz.NumberOfQuizSolved}";
                using (var grid = await conn.QueryMultipleAsync(sql, new { query.QuizId, topusers = query.NumberOfUsers }).ConfigureAwait(false))
                {
                    var retVal = new Item.QuizSolversWithCountDto
                    {
                        Users = await grid.ReadAsync<Item.QuizBestUser>().ConfigureAwait(false),
                        SolversCount = await grid.ReadFirstOrDefaultAsync<int>().ConfigureAwait(false)
                    };

                    return retVal;
                }
            }
        }

        public async Task<Item.QuizQuestionWithSolvedAnswersDto> GetQuizQuestionWithAnswersAsync(GetQuizQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var sql = $"{Sql.Quiz.Question} {Sql.Quiz.Answer} {Sql.Quiz.UserAnswer} {Sql.Quiz.UserQuiz}";
                using (var grid = await conn.QueryMultipleAsync(sql, query).ConfigureAwait(false))
                {
                    var retVal = new Item.QuizQuestionWithSolvedAnswersDto
                    {
                        Questions = await grid.ReadAsync<Item.QuestionWithDetailDto>().ConfigureAwait(false)
                    };
                    var answers = (await grid.ReadAsync<Item.AnswerWithDetailDto>().ConfigureAwait(false)).ToList();
                    var solvedQuestion = await grid.ReadAsync<Item.SolveQuestion>().ConfigureAwait(false);

                    foreach (var question in retVal.Questions)
                    {
                        question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                    }
                    retVal.UserAnswers = solvedQuestion;
                    retVal.Sheet = await grid.ReadFirstOrDefaultAsync<Item.SolveSheet>().ConfigureAwait(false);
                    return retVal;
                }
            }
        }

        public async Task<Item.QuizWithDetailSolvedDto> GetQuizAsync(GetQuizQuery query)
        {
            var retVal = new Item.QuizWithDetailSolvedDto();
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = Sql.Quiz.QuizQuery + Sql.Quiz.Question + Sql.Quiz.Answer + Sql.Quiz.UserQuiz
                                   + Sql.Quiz.UserLike +
                                   Sql.Quiz.UserAnswer + Sql.Quiz.TopUsers;
                using (var grid = await conn.QueryMultipleAsync(sql, new { query.QuizId, query.UserId, topusers = 3 }).ConfigureAwait(false))
                {
                    retVal.Quiz = await grid.ReadFirstAsync<Item.QuizWithDetailDto>().ConfigureAwait(false);
                    retVal.Quiz.Questions = await grid.ReadAsync<Item.QuestionWithDetailDto>().ConfigureAwait(false);
                    var answers = (await grid.ReadAsync<Item.AnswerWithDetailDto>().ConfigureAwait(false)).ToLookup(c => c.QuestionId);

                    foreach (var question in retVal.Quiz.Questions)
                    {
                        question.Answers.AddRange(answers[question.Id]);
                        //question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                    }
                    retVal.Sheet = await grid.ReadFirstOrDefaultAsync<Item.SolveSheet>().ConfigureAwait(false);
                    retVal.Like = await grid.ReadFirstOrDefaultAsync<Guid?>().ConfigureAwait(false);
                    var solvedQuestion = await grid.ReadAsync<Item.SolveQuestion>().ConfigureAwait(false);
                    if (retVal.Sheet != null)
                    {
                        retVal.Sheet.Questions = solvedQuestion;
                    }
                    retVal.Quiz.TopUsers = await grid.ReadAsync<Item.QuizBestUser>().ConfigureAwait(false);
                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<Item.DiscussionDto>> GetDiscussionAsync(GetDisscussionQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<Item.DiscussionDto>(Sql.Quiz.Discussion, query).ConfigureAwait(false);
            }
        }

        public async Task<Item.QuizWithDetailDto> GetDraftQuizAsync(GetQuizDraftQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync($"{Sql.Quiz.QuizQuery} {Sql.Quiz.Question} {Sql.Quiz.Answer}", query).ConfigureAwait(false))
                {
                    var retVal = await grid.ReadFirstAsync<Item.QuizWithDetailDto>().ConfigureAwait(false);
                    retVal.Questions = await grid.ReadAsync<Item.QuestionWithDetailDto>().ConfigureAwait(false);

                    var answers = (await grid.ReadAsync<Item.AnswerWithDetailDto>().ConfigureAwait(false)).ToList();

                    foreach (var question in retVal.Questions)
                    {
                        question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                    }
                    return retVal;
                }
            }
        }

        public async Task<int> GetNumberOfSolversAsync(long quizId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<int>(Sql.Quiz.NumberOfQuizSolved, quizId).ConfigureAwait(false);
                return retVal;
            }
        }

        #endregion

        public async Task<IEnumerable<long>> GetUniversityWithCodeAsync()
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = @"select id from zbox.university  where needCode = 1
                                    union 
	                select distinct universityId2 from zbox.student where universityId2 is not null
					union 
					select distinct universityId from zbox.department";
                return await conn.QueryAsync<long>(sql).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<SmallNodeDto>> GetUniversityNodesAsync(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = @"select l.Name,l.LibraryId as id, l.Settings as type ,
b.boxId as Id,b.BoxName as Name, b.CourseCode as CourseId,b.ProfessorName as ProfessorName , b.quizCount + b.itemCount + b.FlashcardCount as Items,
                                b.MembersCount as Members
from zbox.library l join zbox.box b on l.libraryId = b.libraryId where university = @UniversityId and isDeleted = 0 and Discriminator = 2;";
                const string sql2 = "select l.Name,l.LibraryId as id,l.Settings as type from zbox.library l where id = @UniversityId and settings = 1 and ParentId is null";
                using (var grid = await conn.QueryMultipleAsync(sql + sql2, new { UniversityId = universityId }).ConfigureAwait(false))
                {
                    var dic = new Dictionary<Guid, SmallNodeDto>();
                    grid.Read<SmallNodeDto, Box.SmallBoxDto, Guid>((node, box) =>
                    {
                        if (dic.ContainsKey(node.Id))
                        {
                            dic[node.Id].Boxes.Add(box);
                        }
                        else
                        {
                            node.Boxes = new List<Box.SmallBoxDto>
                            {
                                box

                            };
                            dic.Add(node.Id, node);
                        }
                        return node.Id;
                    });
                    var nodes = dic.Select(s => s.Value).Union(await grid.ReadAsync<SmallNodeDto>().ConfigureAwait(false)).OrderByDescending(a => a.Boxes?.Count);
                    return nodes;
                }
            }
        }

        public async Task<FlashcardUserDto> GetUserFlashcardAsync(GetUserFlashcardQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = "SELECT position FROM [Zbox].[FlashcardPin] where flashcardId = @FlashcardId and userId = @UserId;";
                const string sqlLike = "SELECT  [Id] FROM[Zbox].[FlashcardLike] where flashcardId = @FlashcardId and userId = @UserId;";
                const string sqlOwnerName = "select username as OwnerName, f.publish from zbox.users u join zbox.Flashcard f on f.UserId = u.UserId where id = @FlashcardId;";

                using (var grid = await conn.QueryMultipleAsync(sqlOwnerName + sql + sqlLike, query).ConfigureAwait(false))
                {
                    var result = await grid.ReadFirstAsync<FlashcardUserDto>().ConfigureAwait(false);
                    result.Pins = await grid.ReadAsync<int>().ConfigureAwait(false);
                    result.Like = await grid.ReadFirstOrDefaultAsync<Guid?>().ConfigureAwait(false);
                    return result;
                }
            }
        }

        #region Gamification

        public async Task<User.GamificationBoardDto> GamificationBoardAsync(QueryBaseUserId query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryFirstAsync<User.GamificationBoardDto>(Sql.User.GamificationBoard, query).ConfigureAwait(false);
            }
        }

        public async Task<User.LevelDto> UserLevelsAsync(QueryBaseUserId query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryFirstAsync<User.LevelDto>(Sql.User.Level, query).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<User.BadgeDto>> UserBadgesAsync(QueryBaseUserId query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<User.BadgeDto>(Sql.User.Badge, query).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<LeaderBoardDto>> UserLeaderBoardAsync(LeaderboardQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<LeaderBoardDto>(Sql.User.LeaderBoardAll, query).ConfigureAwait(false);
            }
        }

        #endregion

        #region Jared

        //public async Task<JaredDto> GetJaredStartupValuesAsync(CancellationToken token)
        //{
        //    using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
        //    {
        //        const string actionsText = "select action,text from zbox.jaredText;";
        //        var command = new CommandDefinition(actionsText,
        //               cancellationToken: token);
        //        using (var grid = await conn.QueryMultipleAsync(command).ConfigureAwait(false))
        //        {
        //            var groupResult = (await grid.ReadAsync<JaredTextDto>().ConfigureAwait(false)).GroupBy(x => x.Action, x => x.Text);
        //            return new JaredDto
        //            {
        //                ActionsText = groupResult.ToDictionary(x => x.Key, x => x.AsEnumerable())
        //            };
        //        }
        //    }
        //}

        public async Task<Tuple<User.UserDetailDto, IEnumerable<BoxDto>>> GetJaredUserDataAsync(
            QueryBaseUserId query, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(
                    new CommandDefinition(Sql.Sql.UserAuthenticationDetail + Sql.Dashboard.UserBoxes
                    , new
                    {
                        query.UserId,
                        pageNumber = 0,
                        rowsperpage = int.MaxValue
                    }, cancellationToken: token)).ConfigureAwait(false))
                {
                    var user = await grid.ReadFirstAsync<User.UserDetailDto>().ConfigureAwait(false);
                    var boxes = await grid.ReadAsync<BoxDto>().ConfigureAwait(false);
                    return new Tuple<User.UserDetailDto, IEnumerable<BoxDto>>(user, boxes);
                }
            }
        }

        public async Task<JaredFavoriteDto> JaredFavoritesAsync(JaredFavoritesQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(Sql.Jared.DocumentFavorites
                    + Sql.Jared.QuizFavorites + Sql.Jared.FlashcardFavorite + Sql.Jared.CommentFavorite,
                    new { query.DocumentIds, query.FlashcardIds, query.QuizIds, query.CommentIds }).ConfigureAwait(false))
                {
                    var retVal = new JaredFavoriteDto
                    {
                        Documents = await grid.ReadAsync<JaredFavoriteDocumentDto>().ConfigureAwait(false),
                        Quizzes = await grid.ReadAsync<JaredFavoriteQuiz>().ConfigureAwait(false),
                        Flashcards = await grid.ReadAsync<JaredFavoriteFlashcardDto>().ConfigureAwait(false),
                        Comments = await grid.ReadAsync<JaredFavoriteCommentDto>().ConfigureAwait(false)
                    };
                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<User.ChatUserDto>> OnlineUsersByClassAsync(GetBoxIdQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql =
                                    @"select u.userId as id,u.userImageLarge as image, u.username as name, online, LastAccessTime as LastSeen
                from zbox.users u join zbox.userBoxRel ub on u.userId = ub.userId
               where ub.boxId = @BoxId";
                return await conn.QueryAsync<User.ChatUserDto>(
                    new CommandDefinition(sql
                       , new { query.BoxId })).ConfigureAwait(false);
            }
        }

        #endregion

        #region JaredSearch
        public async Task<IEnumerable<ItemTagsDto>> GetItemsWithTagsAsync(JaredSearchQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(Sql.Jared.ItemInfo + Sql.Jared.ItemTags, query).ConfigureAwait(false))
                {
                    var retVal = (await grid.ReadAsync<ItemTagsDto>().ConfigureAwait(false)).ToList();
                    var tags = (await grid.ReadAsync<ItemTagDto>().ConfigureAwait(false)).ToList();
                    foreach (var item in retVal)
                    {
                        item.Tags = tags.Where(w => w.ItemId == item.ItemId).Select(s => s.Tag);
                    }

                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<string>> GetUniAsync(SearchTermQuery term)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<string>(Sql.Jared.AutoUni, term).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<string>> GetDepartmentAsync(SearchTermQuery term)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<string>(Sql.Jared.AutoDepartment, term).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<string>> GetTagAsync(SearchTermQuery term)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<string>(Sql.Jared.AutoTag, term).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
