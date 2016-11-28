using System.Reflection;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain;
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

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadService : BaseReadService, IZboxReadService, IUniversityWithCode
    {

        public async Task<HomePageDataDto> GetHomePageDataAsync(GetHomePageQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = Sql.HomePage.Stats;
                if (query.UniversityId.HasValue)
                {
                    sql += Sql.HomePage.UniversityColors;
                }
                using (var grid = await conn.QueryMultipleAsync(sql
                    , new { query.UniversityId }))
                {
                    var retVal = new HomePageDataDto
                    {
                        HomePageStats = await grid.ReadFirstAsync<HomePageStats>()
                    };
                    if (grid.IsConsumed) return retVal;
                    retVal.HomePageUniversityData = await grid.ReadFirstAsync<HomePageUniversityData>();
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

        public async Task<IEnumerable<Box.RecommendBoxDto>> GetUniversityBoxesAsync(GetHomeBoxesUniversityQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return
                    await
                        conn.QueryAsync<Box.RecommendBoxDto>(Sql.HomePage.UniversityBoxes,
                            new { query.UniversityId, query.Country });
            }
        }

        public async Task<IEnumerable<Box.RecommendBoxDto>> GetCoursesPageDataAsync()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var fieldInfo = typeof(Sql.Sql).GetFields(BindingFlags.Public | BindingFlags.Static |
                                            BindingFlags.FlattenHierarchy)
                                            .FirstOrDefault(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name == "GetCoursesPageBoxes_" + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower());

                var coursesQuery = Sql.Sql.GetCoursesPageBoxes_en;
                if (fieldInfo != null)
                {
                    coursesQuery = fieldInfo.GetValue(null).ToString();
                }
                return await conn.QueryAsync<Box.RecommendBoxDto>(coursesQuery);
            }

        }

        /// <summary>
        /// used to get the dashboard and the activity and wall in dashboard
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BoxDto>> GetUserBoxesAsync(GetBoxesQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<BoxDto>(Sql.Dashboard.UserBoxes,
                    new { query.UserId, query.RowsPerPage, query.PageNumber });
            }

        }

        public async Task<IEnumerable<LeaderBoardDto>> GetDashboardLeaderBoardAsync(LeaderBoardQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<LeaderBoardDto>(Sql.Sql.UniversityLeaderBoard, query);
            }
        }

        public async Task<LeaderboardFlashcardDto> GetDashboardFlashcardStatusAsync(FlashcardLeaderboardQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = @"select row,likecount as count from ( 
select ROW_NUMBER() OVER(ORDER BY likecount desc) AS Row, * from (
select  sum(likecount) as likecount, f.userid from zbox.flashcard f 
join zbox.users u on f.userid = u.userid and coalesce (u.usertype,0) = 0
where u.universityid = @UniversityId
group by f.userid 
) t) z
where z.userid = @UserId
";
                return await conn.QueryFirstOrDefaultAsync<LeaderboardFlashcardDto>(sql, query);

            }
        }




        /// <summary>
        /// Used in dashboard
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Box.RecommendBoxDto>> GetRecommendedCoursesAsync(RecommendedCoursesQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Box.RecommendBoxDto>(Sql.Sql.RecommendedCourses,
                    new { query.UniversityId, query.UserId });
            }
        }



        /// <summary>
        /// used in dashboard to get user lib in top left
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<UniversityDashboardInfoDto> GetUniversityInfoAsync(UniversityQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryFirstAsync<UniversityDashboardInfoDto>(Sql.Sql.UniversityInfo,
                    new { query.UniversityId });
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
            using (var conn = await DapperConnection.OpenConnectionAsync())
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
                }))
                {
                    var retVal = new NodeBoxesDto
                    {
                        Nodes = await grid.ReadAsync<NodeDto>()
                    };

                    if (grid.IsConsumed) return retVal;
                    retVal.Boxes = await grid.ReadAsync<BoxDto>();
                    retVal.Details = await grid.ReadFirstAsync<NodeDetails>();
                    if (retVal.Details.State == LibraryNodeSetting.Closed)
                    {
                        if (retVal.Details.UserType == UserLibraryRelationType.Pending ||
                            retVal.Details.UserType == UserLibraryRelationType.None)
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

            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal =
                    await conn.QueryAsync<ClosedNodeDto>(Sql.Library.GetClosedLibraryByUser, new { query.UserId });

                return retVal;
            }
        }

        public async Task<IEnumerable<ClosedNodeUsersDto>> GetMembersClosedDepartmendAsync(GetClosedNodeMembersQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal =
                   await conn.QueryAsync<ClosedNodeUsersDto>(Sql.Library.GetClosedLibraryUsers,
                   new { query.UserId, query.LibraryId });

                return retVal;
            }
        }

        public async Task<Box.BoxDto2> GetBox2Async(GetBoxQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<Box.BoxDto2>(Sql.Box.BoxData, new { query.BoxId });
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
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {

                using (var grid = await conn.QueryMultipleAsync(
                    $"{Sql.Box.BoxData} {Sql.Box.BoxMembersWithoutInvited}", new
                    {
                        query.BoxId,
                        top = numberOfMembers
                    }))
                {
                    var box = await grid.ReadFirstOrDefaultAsync<Box.BoxDtoWithMembers>();
                    if (box == null)
                    {
                        throw new BoxDoesntExistException();
                    }
                    box.People = await grid.ReadAsync<User.UserWithImageDto>();
                    return box;
                }

            }
        }



        public async Task<IEnumerable<TabDto>> GetBoxTabsAsync(GetBoxQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<TabDto>(Sql.Box.BoxTabs, new { query.BoxId });

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
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string getBoxNotification =
                    "select NotificationSettings from zbox.UserBoxRel where userid = @UserId and boxid = @BoxId";
                var val = await conn.QueryFirstOrDefaultAsync<NotificationSetting>(getBoxNotification, new
                {
                    UserId = userId,
                    query.BoxId
                });
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
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Item.ItemDto>(Sql.Box.Items, new
                {
                    query.BoxId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage,
                    query.TabId
                });
            }
        }

        public async Task<IEnumerable<Item.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                if (!query.TabId.HasValue)
                {
                    return await conn.QueryAsync<Item.ItemDto>(Sql.Box.ItemsWithoutTabs, new
                    {
                        query.BoxId,
                        pageNumber = query.PageNumber,
                        rowsperpage = query.RowsPerPage,
                    });
                }
                return await conn.QueryAsync<Item.ItemDto>(Sql.Box.Items, new
                {
                    query.BoxId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage,
                    query.TabId
                });
            }
        }



        public async Task<IEnumerable<Item.QuizDto>> GetBoxQuizesAsync(GetBoxQuizesPagedQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Item.QuizDto>(Sql.Quiz.GetBoxQuiz, new
                {
                    query.BoxId,
                    query.PageNumber,
                    query.RowsPerPage
                });
            }

        }

        public async Task<IEnumerable<Item.FlashcardDto>> GetBoxFlashcardsAsync(GetFlashCardsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Item.FlashcardDto>(Sql.Box.Flashcards, new
                {
                    query.BoxId,
                });
            }

        }




        public async Task<Item.ItemMobileDto> GetItemDetailApiAsync(GetItemQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Item.ItemMobileDto>(Sql.Item.ItemDetailApi, new { query.ItemId, query.UserId });
                return retVal.SingleOrDefault();
            }
        }

        public async Task<IEnumerable<Activity.AnnotationDto>> GetItemCommentsAsync(ItemCommentQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync($"{Sql.Item.ItemComments} {Sql.Item.ItemCommentReply}",
                    new { query.ItemId }))
                {
                    var retVal = await grid.ReadAsync<Activity.AnnotationDto>();

                    IEnumerable<Activity.AnnotationReplyDto> replies =
                        grid.Read<Activity.AnnotationReplyDto>().ToList();

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
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (
                    var grid =
                        await
                            conn.QueryMultipleAsync(
                                $"{Sql.Item.ItemDetail} {Sql.Item.Navigation}  {Sql.Item.UserItemRate}",
                                new { query.ItemId, query.BoxId, query.UserId }))
                {
                    var retVal = await grid.ReadFirstOrDefaultAsync<Item.ItemDetailDto>();
                    if (retVal == null)
                    {
                        throw new ItemNotFoundException();
                    }
                    retVal.Navigation = await grid.ReadFirstOrDefaultAsync<Item.ItemNavigationDto>();

                    retVal.Like = await grid.ReadFirstOrDefaultAsync<int>();
                    return retVal;
                }

            }
        }

        public async Task<IEnumerable<Box.RecommendBoxDto>> GetBoxRecommendedCoursesAsync(GetBoxSideBarQuery query, CancellationToken token)
        {
            using (var con = await DapperConnection.OpenConnectionAsync(token))
            {
                return await con.QueryAsync<Box.RecommendBoxDto>(
                    new CommandDefinition(Sql.Box.RecommendedCourses, new { query.BoxId, query.UserId }, commandTimeout: 5, cancellationToken: token)
                );

            }
        }
        public async Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoardAsync(GetLeaderBoardQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                return await con.QueryAsync<LeaderBoardDto>(Sql.Box.LeaderBoard,
                    new { query.BoxId });
            }
        }





        public async Task<IEnumerable<Qna.CommentDto>> GetCommentsAsync(GetBoxQuestionsQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await con.QueryMultipleAsync(
                    $"{Sql.Feed.Comments} {Sql.Feed.RepliesInComments} {Sql.Feed.GetItemsInCommentsAndReplies} {Sql.Feed.GetQuizzesForComments}",
                    new { query.BoxId, query.Top, query.Skip, rtop = GetBoxQuestionsQuery.TopOfReplies }))
                {
                    var comments = grid.Read<Qna.CommentDto>();//.ToList();
                    var replies = grid.Read<Qna.ReplyDto>();//.ToDictionary(x => x.QuestionId);
                    var items = grid.Read<Qna.ItemDto>().Union(grid.Read<Qna.ItemDto>());//.ToLookup(c => c.QuestionId ?? c.AnswerId);


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
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await con.QueryMultipleAsync(
                    $"{Sql.Box.GetCommentForMobile} {Sql.Box.GetCommentFileForMobile}",
                    new { query.BoxId, query.QuestionId }))
                {
                    var comment = await grid.ReadFirstAsync<Qna.CommentDto>();
                    comment.Files = await grid.ReadAsync<Qna.ItemDto>();
                    return comment;

                }
            }
        }

        public async Task<IEnumerable<Qna.LikeDto>> GetReplyLikesAsync(GetFeedLikesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "select UserImageLarge as Image ,UserName as Name,Url from zbox.ReplyLike join zbox.Users on OwnerId = userid where replyid = @Id";
                return await con.QueryAsync<Qna.LikeDto>(sql, new { query.Id });
            }
        }
        public async Task<IEnumerable<Qna.LikeDto>> GetCommentLikesAsync(GetFeedLikesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "select UserImageLarge as Image ,UserName as Name,Url from zbox.CommentLike join zbox.Users on OwnerId = userid where CommentId = @Id";
                return await con.QueryAsync<Qna.LikeDto>(sql, new { query.Id });
            }
        }

        public async Task<IEnumerable<Guid>> GetUserFeedLikesAsync(UserLikesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                const string commentSql = @"select commentid from zbox.CommentLike
where ownerid = @UserId and boxid = @BoxId;";

                const string replySql = @"select ReplyId from zbox.ReplyLike
where ownerid = @UserId and boxid = @BoxId;";

                using (var grid = await con.QueryMultipleAsync(commentSql + replySql, new { query.UserId, query.BoxId }))
                {
                    var retVal = new List<Guid>();
                    retVal.AddRange(await grid.ReadAsync<Guid>());
                    retVal.AddRange(await grid.ReadAsync<Guid>());
                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<Qna.ReplyDto>> GetRepliesAsync(GetCommentRepliesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await con.QueryMultipleAsync(
                    $"{Sql.Feed.GetReplies} {Sql.Feed.GetItemsInReply}",
                    new { query.BoxId, query.PageNumber, query.RowsPerPage, query.CommentId, AnswerId = query.BelowReplyId }))
                {
                    var replies = grid.Read<Qna.ReplyDto>();
                    var items = grid.Read<Qna.ItemDto>().ToLookup(c => c.AnswerId);

                    foreach (var reply in replies)
                    {
                        reply.Files = items[reply.Id];
                    }

                    return replies;
                }
            }
        }

        /// <summary>
        /// Used for autocomplete in the share box dialog
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User.UserDto>> GetUserFriendsAsync(GetUserFriendsQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<User.UserDto>(Sql.Sql.FriendList, new
                {
                    query.UserId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                });
            }
        }

        public async Task<User.UserMinProfile> GetUserMinProfileAsync(GetUserMinProfileQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = @"select u.userid as Id, u.username as name, u.UserImageLarge as image,
                            u.userReputation as score, uu.universityname as universityName, u.url as Url
                            from zbox.users u left join zbox.university uu on u.UniversityId = uu.id
                            where u.userid =@UserId";
                var retVal = await conn.QueryFirstAsync<User.UserMinProfile>(sql, new { query.UserId });
                return retVal;

            }
        }


        /// <summary>
        /// used in user page to get initial data
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<User.UserWithStats> GetUserProfileWithStatsAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {


                using (var grid = await conn.QueryMultipleAsync(Sql.User.UserProfileWithStats, new
                {
                    //Me = query.UserId,
                    Myfriend = query.FriendId,
                }))
                {
                    var retVal = await grid.ReadFirstOrDefaultAsync<User.UserWithStats>();
                    if (retVal != null)
                    {
                        retVal.NumClass = await grid.ReadFirstOrDefaultAsync<int>();
                        retVal.NumItem = await grid.ReadFirstOrDefaultAsync<int>();
                        retVal.NumFeed = await grid.ReadFirstOrDefaultAsync<int>() + await grid.ReadFirstOrDefaultAsync<int>();
                        retVal.NumQuiz = await grid.ReadFirstOrDefaultAsync<int>();
                        retVal.NumFriend = await grid.ReadFirstOrDefaultAsync<int>();

                    }
                    return retVal;
                }




            }
        }

        /// <summary>
        /// Get The country the user is in based on the ip address
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetLocationByIpAsync(GetCountryByIpQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = @" select country_code2  from zbox.ip_range 
    where ip_from <= @IP and @IP <= ip_to";
                var retVal = await conn.QueryFirstOrDefaultAsync<string>(sql, new { IP = query.IpAddress });
                return retVal;
            }

        }

        public async Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddressAsync(UniversityByIpQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<UniversityByPrefixDto>(Sql.LibraryChoose.GetInitialValueOfUniversitiesBaseOnIpAddress,
                     new
                     {
                         IP = query.IpAddress,
                         query.PageNumber,
                         query.RowsPerPage

                     });
            }
        }

        /// <summary>
        /// Used in api to private to box / invite to box
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User.UserWithImageNameDto>> GetUsersInBoxByTermAsync(UserInBoxSearchQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = string.IsNullOrEmpty(query.Term) ? Sql.Search.GetUsersInBox : Sql.Search.GetUsersInBoxByTerm;
                return await conn.QueryAsync<User.UserWithImageNameDto>(sql,
                     new
                     {
                         query.Term,
                         query.PageNumber,
                         query.RowsPerPage,
                         query.UniversityId,
                         query.BoxId

                     });
            }
        }

        //public async Task<IEnumerable<User.ChatUserDto>> GetUsersByTermAsync(UserSearchQuery query)
        //{
        //    using (var conn = await DapperConnection.OpenConnectionAsync())
        //    {
        //        return await conn.QueryAsync<User.ChatUserDto>(Sql.Search.GetUsersByTerm,
        //             new
        //             {
        //                 query.Term,
        //                 query.PageNumber,
        //                 query.RowsPerPage,
        //                 query.UniversityId,
        //                 query.UserId

        //             });
        //    }
        //}

        public async Task<IEnumerable<User.ChatUserDto>> GetUsersConversationAndFriendsAsync(GetUserConversationAndFriends query)
        {

            using (var conn = await DapperConnection.OpenConnectionAsync())
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

                     });
                return result.Distinct(new User.ChatUserDtoComparer());
            }
        }
        public async Task<IEnumerable<ChatDto>> GetUserConversationAsync(GetChatRoomMessagesQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            using (var conn = await DapperConnection.OpenConnectionAsync())
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

                        });
                }
                return await conn.QueryAsync<ChatDto>(Sql.Chat.GetChatByUserIds,
                        new
                        {
                            query.UserIds,
                            length = query.UserIds.Count(),
                            creationTime = query.FromTime,
                            query.Skip,
                            query.Top

                        });
            }
        }





        public async Task<UniversityWithCodeDto> GetUniversityNeedIdAsync(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<UniversityWithCodeDto>(Sql.LibraryChoose.GetNeedId, new
                {
                    universityId
                });
                return retVal;
            }
        }


        public async Task<int> GetChatUnreadMessagesAsync(GetUserDetailsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var result = await conn.QueryFirstOrDefaultAsync<int?>(Sql.Chat.GetUnreadMessages, new { query.UserId });
                return result.GetValueOrDefault();
            }
        }

        public async Task<User.UserDetailDto> GetUserDataAsync(GetUserDetailsQuery query)
        {

            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                // {Sql.Chat.GetUnreadMessages}
                using (
                    var grid = await conn.QueryMultipleAsync($"{Sql.Sql.UserAuthenticationDetail}",
                        new { query.UserId }))
                {
                    var user = await grid.ReadFirstOrDefaultAsync<User.UserDetailDto>();
                    if (user == null)
                    {
                        throw new UserNotFoundException("user is null");
                    }
                    //var count = await grid.ReadFirstOrDefaultAsync<int?>();
                    //if (count != null)
                    //{
                    //    user.Unread = count.Value;
                    //}
                    return user;
                }

            }
        }



        #region Account Settings
        public async Task<User.UserAccountDto> GetUserAccountDetailsAsync(GetUserDetailsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryFirstAsync<User.UserAccountDto>(Sql.Sql.GetUserAccountData, new { query.UserId });
            }
        }
        /// <summary>
        /// Used in account settings notification section
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<User.UserNotification> GetUserBoxesNotificationAsync(GetUserDetailsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string boxNotificationSql =
                    @"select b.BoxId as Id, b.BoxName as Name,  ub.NotificationSettings as Notifications, b.Url as url, u.UserName
                    from zbox.Box b 
	                inner join zbox.UserBoxRel ub on b.BoxId = ub.BoxId
					inner join zbox.Users u on b.OwnerId = u.UserId
                    where ub.UserId = @UserId";

                const string userEmailSettings = @"select EmailSendSettings from zbox.Users where userid = @UserId";
                using (var grid = await conn.QueryMultipleAsync($"{boxNotificationSql} {userEmailSettings}", new { query.UserId }))
                {
                    var retVal = new User.UserNotification
                    {
                        BoxNotifications = await grid.ReadAsync<Box.BoxNotificationDto>(),
                        EmailNotification = await grid.ReadFirstOrDefaultAsync<EmailSend>()
                    };
                    return retVal;

                }
                //           await conn.QueryAsync<Box.BoxNotificationDto>(@"select b.BoxId as Id, b.BoxName as Name,  ub.NotificationSettings as Notifications, b.Url as url, u.UserName
                //               from zbox.Box b 
                //            inner join zbox.UserBoxRel ub on b.BoxId = ub.BoxId
                //inner join zbox.Users u on b.OwnerId = u.UserId
                //               where ub.UserId = @UserId", new { query.UserId });
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
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<User.UserMemberDto>(Sql.Box.BoxMembers, new { query.BoxId, query.PageNumber, query.RowsPerPage });
            }
        }

        /// <summary>
        /// Used for push notification to send message to users in box
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        //public async Task<IEnumerable<long>> GetBoxUsersIdAsync(GetBoxWithUserQuery query)
        //{
        //    using (var conn = await DapperConnection.OpenConnectionAsync())
        //    {
        //        return await conn.QueryAsync<long>(Sql.Box.BoxUserIds,
        //            new
        //            {
        //                query.BoxId,
        //                pageNumber = query.PageNumber,
        //                rowsperpage = query.RowsPerPage,
        //                query.UserId
        //            });
        //    }
        //}

        #endregion


        public async Task<bool> GetInviteAsync(GetInviteDetailQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sqlQuery = @"select count(*) from   zbox.Message 
                                where TypeOfMsg = 2
                                and MessageId = @MessageId
                                and isActive = 1";
                var count = await conn.QueryFirstOrDefaultAsync<int>(sqlQuery, new { query.MessageId });

                return count > 0;
            }
        }


        public async Task<IEnumerable<UpdatesDto>> GetUpdatesAsync(QueryBase query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<UpdatesDto>(Sql.Updates.GetUserUpdates, new { userid = query.UserId });
            }
        }

        #region UserPage
        public async Task<IEnumerable<BoxDto>> GetUserBoxesActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<BoxDto>(Sql.User.UserWithFriendBoxes,
                    new
                    {
                        //Me = query.UserId,
                        Myfriend = query.FriendId,
                        pageNumber = query.PageNumber,
                        rowsperpage = query.RowsPerPage
                    });
                return retVal;
            }
        }

        public async Task<IEnumerable<Item.QuizDto>> GetUserQuizActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Item.QuizDto>(Sql.User.UserWithFriendQuizzes, new
                {
                    //Me = query.UserId,
                    Myfriend = query.FriendId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                });
            }
        }


        public async Task<IEnumerable<Item.ItemDto>> GetUserItemsActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Item.ItemDto>(Sql.User.UserWithFriendFiles, new
                {
                    // Me = query.UserId,
                    Myfriend = query.FriendId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                });
            }
        }

        /// <summary>
        /// Used in mobile service api
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User.ActivityDto>> GetUserCommentActivityAsync(GetUserWithFriendQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<User.ActivityDto>(Sql.Sql.UserQuestionAndAnswersActivityMobileApi, new
                {
                    //Me = query.UserId,
                    Myfriend = query.FriendId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                });
            }
        }

        #endregion

        #region Seo
        public async Task<IEnumerable<SeoDto>> GetSeoItemsAsync(int page)
        {
            const int pageSize = 49950;
            if (page < 1)
            {
                return null;
            }
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<SeoDto>($"{Sql.Seo.GetSeoItemsByPage}",
                    new { rowsperpage = pageSize, pageNumber = page });
            }
        }

        public async Task<int> GetSeoItemCountAsync()
        {
            const int pageSize = 49950;
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<int>($"{Sql.Seo.GetSeoItemsCount}");
                return (retVal / pageSize) + 1;
            }
        }

        #endregion





        #region Quiz

        public async Task<Item.QuizSeo> GetQuizSeoAsync(GetQuizSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryFirstAsync<Item.QuizSeo>(Sql.Quiz.QuizSeoQuery, new { query.QuizId });
                return retVal;
            }
        }

        public async Task<Item.FileSeo> GetItemSeoAsync(GetFileSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<Item.FileSeo>(Sql.Seo.FileSeo, new { query.ItemId });
                return retVal;
            }
        }

        public async Task<Box.BoxSeoDto> GetBoxSeoAsync(GetBoxSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<Box.BoxSeoDto>(Sql.Seo.BoxSeo, new { query.BoxId });
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
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = $"{Sql.Quiz.TopUsers} {Sql.Quiz.NumberOfQuizSolved}";
                using (
                    var grid =
                        await conn.QueryMultipleAsync(sql, new { query.QuizId, topusers = query.NumberOfUsers }))
                {
                    var retVal = new Item.QuizSolversWithCountDto
                    {
                        Users = await grid.ReadAsync<Item.QuizBestUser>(),
                        SolversCount = await grid.ReadFirstOrDefaultAsync<int>()
                    };

                    return retVal;
                }
            }
        }

        public async Task<Item.QuizQuestionWithSolvedAnswersDto> GetQuizQuestionWithAnswersAsync(GetQuizQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = $"{Sql.Quiz.Question} {Sql.Quiz.Answer} {Sql.Quiz.UserAnswer} {Sql.Quiz.UserQuiz}";
                using (var grid = await conn.QueryMultipleAsync(sql, new { query.QuizId, query.UserId }))
                {
                    var retVal = new Item.QuizQuestionWithSolvedAnswersDto
                    {
                        Questions = await grid.ReadAsync<Item.QuestionWithDetailDto>()
                    };
                    var answers = grid.Read<Item.AnswerWithDetailDto>().ToList();
                    var solvedQuestion = await grid.ReadAsync<Item.SolveQuestion>();

                    foreach (var question in retVal.Questions)
                    {
                        question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                    }
                    retVal.UserAnswers = solvedQuestion;
                    retVal.Sheet = await grid.ReadFirstOrDefaultAsync<Item.SolveSheet>();
                    return retVal;
                }
            }
        }

        public async Task<Item.QuizWithDetailSolvedDto> GetQuizAsync(GetQuizQuery query)
        {
            var retVal = new Item.QuizWithDetailSolvedDto();
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql =
                    $"{Sql.Quiz.QuizQuery} {Sql.Quiz.Question} {Sql.Quiz.Answer} {Sql.Quiz.UserQuiz} {Sql.Quiz.UserAnswer} {Sql.Quiz.TopUsers}";
                using (var grid = await conn.QueryMultipleAsync(sql, new { query.QuizId, query.BoxId, query.UserId, topusers = 3 }))
                {
                    retVal.Quiz = await grid.ReadFirstAsync<Item.QuizWithDetailDto>();
                    retVal.Quiz.Questions = await grid.ReadAsync<Item.QuestionWithDetailDto>();
                    var answers = grid.Read<Item.AnswerWithDetailDto>().ToList();

                    foreach (var question in retVal.Quiz.Questions)
                    {
                        question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                    }
                    retVal.Sheet = await grid.ReadFirstOrDefaultAsync<Item.SolveSheet>();
                    var solvedQuestion = await grid.ReadAsync<Item.SolveQuestion>();
                    if (retVal.Sheet != null)
                    {
                        retVal.Sheet.Questions = solvedQuestion;
                    }
                    retVal.Quiz.TopUsers = await grid.ReadAsync<Item.QuizBestUser>();
                    return retVal;
                }
            }
        }



        public async Task<IEnumerable<Item.DiscussionDto>> GetDiscussionAsync(GetDisscussionQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Item.DiscussionDto>(Sql.Quiz.Discussion, new { query.QuizId });
            }
        }

        public async Task<Item.QuizWithDetailDto> GetDraftQuizAsync(GetQuizDraftQuery query)
        {

            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(
                    $"{Sql.Quiz.QuizQuery} {Sql.Quiz.Question} {Sql.Quiz.Answer}", new { query.QuizId }))
                {
                    var retVal = await grid.ReadFirstAsync<Item.QuizWithDetailDto>();
                    retVal.Questions = grid.Read<Item.QuestionWithDetailDto>();

                    var answers = grid.Read<Item.AnswerWithDetailDto>().ToList();

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
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryFirstOrDefaultAsync<int>(Sql.Quiz.NumberOfQuizSolved, new { QuizId = quizId });
                return retVal;
            }
        }
        #endregion

        public async Task<IEnumerable<long>> GetUniversityWithCodeAsync()
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = @"select id from zbox.university  where needcode = 1
                                    union 
	                select distinct universityid2 from zbox.student where universityid2 is not null
					union 
					select distinct universityid from zbox.department";
                return await conn.QueryAsync<long>(sql);
            }
        }


        public async Task<IEnumerable<SmallNodeDto>> GetUniversityNodesAsync(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql =
                    @"select l.Name,l.LibraryId as id, l.Settings as type ,
b.boxid as Id,b.BoxName as Name, b.CourseCode as CourseId,b.ProfessorName as ProfessorName , b.quizcount + b.itemcount + b.FlashcardCount as Items,
                                b.MembersCount as Members
from zbox.library l join zbox.box b on l.libraryid = b.libraryid where university = @UniversityId and isdeleted = 0 and Discriminator = 2;";
                const string sql2 = "select l.Name,l.LibraryId as id,l.Settings as type from zbox.library l where id = @UniversityId and settings = 1 and ParentId is null";
                using (
                   var grid = await conn.QueryMultipleAsync(sql + sql2,
                       new { UniversityId = universityId }))
                {
                    var dic = new Dictionary<Guid, SmallNodeDto>();
                    grid.Read<SmallNodeDto, Box.SmallBoxDto, Guid>((node, box) =>
                    {
                        if (dic.ContainsKey(node.Id))
                            dic[node.Id].Boxes.Add(box);
                        else
                        {
                            node.Boxes = new List<Box.SmallBoxDto> { box };
                            dic.Add(node.Id, node);
                        }
                        return node.Id;
                    });
                    var nodes = dic.Select(s => s.Value)
                        .Union(await grid.ReadAsync<SmallNodeDto>())
                        .OrderByDescending(a => a.Boxes?.Count);
                    return nodes;

                }
            }
        }

        public async Task<FlashcardUserDto> GetUserFlashcardAsync(GetUserFlashcardQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "SELECT position FROM [Zbox].[FlashcardPin] where flashcardid = @FlashcardId and userid = @UserId;";
                const string sqlLike = "SELECT  [Id] FROM[Zbox].[FlashcardLike] where flashcardid = @FlashcardId and userid = @UserId;";
                const string sqlOwnerName =
                    "select username from zbox.users u join zbox.Flashcard f on f.UserId = u.UserId where id = @FlashcardId";
                using (
                    var grid = await conn.QueryMultipleAsync(sql + sqlLike + sqlOwnerName, query))
                {
                    var result = new FlashcardUserDto
                    {
                        Pins = await grid.ReadAsync<int>(),
                        Like = await grid.ReadFirstOrDefaultAsync<Guid?>(),
                        OwnerName = await grid.ReadFirstAsync<string>()
                        
                    };
                    return result;
                }
            }
        }
    }
}
