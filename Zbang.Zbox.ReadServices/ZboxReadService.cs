using Dapper;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
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
using ExtensionTransformers = Zbang.Zbox.Infrastructure.Data.Transformers;
using Item = Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Qna = Zbang.Zbox.ViewModel.Dto.Qna;
using User = Zbang.Zbox.ViewModel.Dto.UserDtos;
using Sql = Zbang.Zbox.ViewModel.SqlQueries;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadService : BaseReadService, IZboxReadService, IUniversityWithCode
    {


        /// <summary>
        /// used to get the dashboard and the activity and wall in dashboard
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<IEnumerable<BoxDto>> GetUserBoxesOld(GetBoxesQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<BoxDto>(Sql.Dashboard.UserBoxesOld,
                    new { query.UserId, query.RowsPerPage, query.PageNumber });

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
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<LeaderBoardDto>(Sql.Sql.UniversityLeaderBoard, new { query.UniversityId });
            }
        }


        //public async Task<DashboardDto> GetDashboardSideBarAsync(GetDashboardQuery query)
        //{
        //    using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
        //    {
        //        using (var grid = await conn.QueryMultipleAsync(string.Format("{0} {1}",
        //            Sql.Sql.DashboardInfo,
        //            Sql.Sql.UniversityLeaderBoard),
        //            new { query.UniversityId }))
        //        {
        //            var retVal = new DashboardDto
        //            {

        //                Info = grid.Read<UniversityDashboardInfoDto>().FirstOrDefault(),
        //                LeaderBoard = await grid.ReadAsync<LeaderBoardDto>()

        //            };

        //            return retVal;
        //        }
        //    }
        //}

        /// <summary>
        /// Used in dashboard
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Box.RecommendBoxDto>> GetRecommendedCoursesAsync(RecommendedCoursesQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
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
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<UniversityDashboardInfoDto>(Sql.Sql.DashboardInfo,
                    new { query.UniversityId });
                return retVal.First();
            }
        }

        /// <summary>
        /// University page - sub nodes or boxes in the current node
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<NodeBoxesDto> GetLibraryNodeAsync(GetLibraryNodeQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
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
                    retVal.Details = grid.Read<NodeDetails>().FirstOrDefault();
                    return retVal;
                }
            }

        }




        /// <summary>
        /// University page - the header with all the details
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<UniversityInfoDto> GetUniversityDetailAsync(GetUniversityDetailQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<UniversityInfoDto>(Sql.Sql.GetUniversityDataByUserId, new { UniversityWrapper = query.UserId });
                return retVal.First();
            }

        }

        /// <summary>
        /// Get user invite - header page section
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<InviteDto>> GetInvitesAsync(GetInvitesQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<InviteDto>(Sql.Sql.UserInvites, new { query.UserId });
            }
        }







        public async Task<Box.BoxDto2> GetBox2Async(GetBoxQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Box.BoxDto2>(Sql.Box.BoxData, new { query.BoxId });
                var box = retVal.FirstOrDefault();
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

                using (var grid = await conn.QueryMultipleAsync(string.Format("{0} {1}", Sql.Box.BoxData, Sql.Box.BoxMembersWithoutInvited), new
                {
                    query.BoxId,
                    top = numberOfMembers
                }))
                {
                    var box = grid.Read<Box.BoxDtoWithMembers>().FirstOrDefault();
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
        public async Task<NotificationSettings> GetUserBoxNotificationSettingsAsync(GetBoxQuery query, long userId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string getBoxNotification =
                    "select NotificationSettings from zbox.UserBoxRel where userid = @UserId and boxid = @BoxId";
                var val = await conn.QueryAsync<NotificationSettings>(getBoxNotification, new
                {
                    UserId = userId, query.BoxId
                });
                return val.FirstOrDefault();
            }
            //using (UnitOfWork.Start())
            //{
            //    IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxNotificationByUser");
            //    dbQuery.SetInt64("BoxId", query.BoxId);
            //    dbQuery.SetInt64("UserId", userId);

            //    return dbQuery.UniqueResult<NotificationSettings>();
            //    //dbQuery.SetResultTransformer(Transformers.AliasToBean<NotificationSettings>());
            //}
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
                     rowsperpage = query.RowsPerPage, query.TabId
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


        /// <summary>
        /// User in Item page to get an item
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Obsolete("Use getItem2")]
        public Item.ItemWithDetailDto GetItem(GetItemQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetItem");
                dbQuery.SetInt64("ItemId", query.ItemId);
                dbQuery.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClassesCtorTransformer(typeof(Item.FileWithDetailDto), typeof(Item.LinkWithDetailDto)));

                var item = dbQuery.FutureValue<Item.ItemWithDetailDto>();

                if (item.Value == null)
                {
                    throw new ItemNotFoundException();
                }
                var retVal = item.Value;
                return retVal;
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

        public async Task<Item.ItemDetailDto> GetItem2Async(GetItemQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (
                    var grid =
                        await
                            conn.QueryMultipleAsync(string.Format("{0} {1}",
                            Sql.Item.ItemDetail,
                            Sql.Item.Navigation
                    //Sql.Item.ItemComments,
                    //Sql.Item.ItemCommentReply,
                    //Sql.Item.UserItemRate
                            ),
                                new { query.ItemId, query.BoxId, query.UserId }))
                {
                    var retVal = grid.Read<Item.ItemDetailDto>().FirstOrDefault();
                    if (retVal == null)
                    {
                        throw new ItemNotFoundException();
                    }
                    retVal.Navigation = grid.Read<Item.ItemNavigationDto>().FirstOrDefault();
                    //retVal.Comments = await grid.ReadAsync<Activity.AnnotationDto>();


                    //IEnumerable<Activity.AnnotationReplyDto> replies =
                    //    grid.Read<Activity.AnnotationReplyDto>().ToList();

                    //foreach (var comment in retVal.Comments)
                    //{
                    //    comment.Replies.AddRange(replies.Where(w => w.ParentId == comment.Id));

                    //}
                    //retVal.Rate = grid.Read<int>().FirstOrDefault();
                    return retVal;
                }

            }
        }

        public async Task<IEnumerable<Box.RecommendBoxDto>> GetBoxRecommendedCoursesAsync(GetBoxSideBarQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                return await con.QueryAsync<Box.RecommendBoxDto>(Sql.Box.RecommendedCourses,
                    new { query.BoxId, query.UserId });
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

        /// <summary>
        /// used in desktop and mobile web site
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Qna.QuestionDto>> GetQuestionsWithAnswersAsync(GetBoxQuestionsQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await con.QueryMultipleAsync(string.Format("{0} {1} {2} {3}",
                    Sql.Box.GetBoxQuestion,
                    Sql.Box.GetBoxAnswers,
                    Sql.Box.GetBoxQnAItem,
                    Sql.Box.GetBoxQnaQuiz
                    ),
                    new { query.BoxId, query.PageNumber, query.RowsPerPage }))
                {
                    var questions = grid.Read<Qna.QuestionDto>().ToList();
                    var answers = grid.Read<Qna.AnswerDto>().ToList();
                    var items = grid.Read<Qna.ItemDto>().Union(grid.Read<Qna.ItemDto>()).ToList();

                    foreach (var answer in answers)
                    {
                        answer.Files.AddRange(items.Where(w => w.AnswerId.HasValue && w.AnswerId.Value == answer.Id));
                    }
                    foreach (var question in questions)
                    {
                        question.Files.AddRange(items.Where(w => w.QuestionId.HasValue && w.QuestionId.Value == question.Id));
                        question.Answers.AddRange(answers.Where(s => s.QuestionId == question.Id));
                    }

                    return questions;
                }
            }
        }

        /// <summary>
        /// Used in mobile service to retrieve the comment and the last reply
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Qna.QuestionDto>> GetQuestionsWithLastAnswerAsync(GetBoxQuestionsQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await con.QueryMultipleAsync(string.Format("{0} {1} {2} {3}",
                    Sql.Box.GetBoxCommentsForMobile,
                    Sql.Box.GetLastCommentRepliesForMobile,
                    Sql.Box.GetBoxItemForCommentInMobile,
                    Sql.Box.GetBoxQuizFromCommentInMobile
                    ),
                    new { query.BoxId, query.PageNumber, query.RowsPerPage }))
                {
                    var comments = grid.Read<Qna.QuestionDto>().ToList();
                    var replies = grid.Read<Qna.AnswerDto>().ToList();
                    var items = grid.Read<Qna.ItemDto>().Union(grid.Read<Qna.ItemDto>()).ToList();

                    foreach (var reply in replies)
                    {
                        reply.Files.AddRange(items.Where(w => w.AnswerId.HasValue && w.AnswerId.Value == reply.Id));
                    }
                    foreach (var comment in comments)
                    {
                        comment.Files.AddRange(items.Where(w => w.QuestionId.HasValue && w.QuestionId.Value == comment.Id));
                        var reply = replies.FirstOrDefault(s => s.QuestionId == comment.Id);
                        if (reply != null)
                        {
                            comment.Answers.Add(reply);
                        }

                    }

                    return comments;
                }
            }
        }

        public async Task<Qna.QuestionDto> GetQuestionAsync(GetQuestionQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await con.QueryMultipleAsync(string.Format("{0} {1}",
                    Sql.Box.GetCommentForMobile,
                    Sql.Box.GetCommentFileForMobile
                    ),
                    new { query.BoxId, query.QuestionId }))
                {
                    var comment = grid.Read<Qna.QuestionDto>().First();
                    comment.Files = grid.Read<Qna.ItemDto>().ToList();
                    return comment;

                }
            }
        }

        public async Task<IEnumerable<Qna.AnswerDto>> GetRepliesAsync(GetCommentRepliesQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await con.QueryMultipleAsync(string.Format("{0} {1}",
                    Sql.Box.GetCommentRepliesInMobile,
                    Sql.Box.GetCommentRepliesItemsInMobile
                    ),
                    new { query.BoxId, query.PageNumber, query.RowsPerPage, query.CommentId }))
                {
                    var replies = grid.Read<Qna.AnswerDto>().ToList();
                    var items = grid.Read<Qna.ItemDto>().ToList();

                    foreach (var reply in replies)
                    {
                        reply.Files.AddRange(items.Where(w => w.AnswerId.HasValue && w.AnswerId.Value == reply.Id));
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
                var retVal = await conn.QueryAsync<User.UserMinProfile>(sql, new { query.UserId });
                return retVal.First();

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
                    Me = query.UserId,
                    Myfriend = query.FriendId,
                }))
                {
                    var retVal = grid.Read<User.UserWithStats>().FirstOrDefault();
                    if (retVal != null)
                    {
                        retVal.NumClass = grid.Read<int>().FirstOrDefault();
                        retVal.NumItem = grid.Read<int>().FirstOrDefault();
                        retVal.NumFeed = grid.Read<int>().FirstOrDefault();
                        retVal.NumQuiz = grid.Read<int>().FirstOrDefault();
                        retVal.NumFriend = grid.Read<int>().FirstOrDefault();

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
                var retVal = await conn.QueryAsync<string>(sql, new { IP = query.IpAddress });
                return retVal.First();
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
        /// Used in api to ivite to box
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User.UserWithImageNameDto>> GetUsersByTermAsync(UserSearchQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<User.UserWithImageNameDto>(Sql.Search.GetUsersByTerm,
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

        public async Task<IEnumerable<RussianDepartmentDto>> GetRussianDepartmentList(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<RussianDepartmentDto>(Sql.LibraryChoose.GetRussianDepartments, new
                {
                    universityId
                });
            }
        }

        public async Task<UniversityWithCodeDto> GetUniversityNeedIdAsync(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<UniversityWithCodeDto>(Sql.LibraryChoose.GetNeedId, new
                {
                    universityId
                });
                return retVal.FirstOrDefault();
            }
        }

        //public async Task<bool> GetUniversityNeedCode(long universityId)
        //{
        //    using (var conn = await DapperConnection.OpenConnectionAsync())
        //    {
        //        var retVal = await conn.QueryAsync<bool>(Sql.LibraryChoose.GetNeedCode, new
        //        {
        //            universityId
        //        });
        //        return retVal.FirstOrDefault();
        //    }
        //}

        public async Task<IEnumerable<UniversityByPrefixDto>> GetUniversityListByFriendsIdsAsync(IEnumerable<long> friendsIds)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                //we can only use 2100 in statement
                using (var grid = await conn.QueryMultipleAsync(
                     string.Format("{0} {1}", Sql.LibraryChoose.GetUniversityByFriendIds,
                     Sql.LibraryChoose.GetFriendsInUniversitiesByFriendsIds),
                    new { FriendsIds = friendsIds.Take(2099) }
                     ))
                {
                    var retVal = await grid.ReadAsync<UniversityByPrefixDto>();
                    var friends = await grid.ReadAsync<FriendPerUniversityDto>();
                    retVal = retVal.Select(s =>
                    {
                        s.UserImages = friends.Where(w => w.UniversityId == s.Id).Select(s1 => s1.Image);
                        return s;
                    });
                    //retVal = retVal.Select(s =>
                    //   {
                    //       s.Friends = friends.Where(w => w.UniversityId == s.Id);
                    //       return s;
                    //   }).ToList();
                    return retVal;
                }
            }
        }


        /// <summary>
        /// Get user data for user detail in all the pages.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public User.UserDetailDto GetUserData(GetUserDetailsQuery query)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                var retVal = conn.Query<User.UserDetailDto>(Sql.Sql.UserAuthenticationDetail,
                            new { query.UserId });

                var user = retVal.FirstOrDefault();
                if (user == null)
                {
                    throw new UserNotFoundException("user is null");
                }
                return user;
            }
        }
        public async Task<User.UserDetailDto> GetUserDataAsync(GetUserDetailsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal =
                       await conn.QueryAsync<User.UserDetailDto>(Sql.Sql.UserAuthenticationDetail,
                       new { query.UserId });
                var user = retVal.FirstOrDefault();
                if (user == null)
                {
                    throw new UserNotFoundException("user is null");
                }
                return user;
            }
        }


        /// <summary>
        /// Get box settings data for ajax request
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public BoxSettingsDto GetBoxSetting(GetBoxQuery query, long userId)
        {
            using (UnitOfWork.Start())
            {
                IQuery boxSettingQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxSettingsDtoById");
                boxSettingQuery.SetParameter("Id", query.BoxId);
                boxSettingQuery.SetParameter("Userid", userId);
                boxSettingQuery.SetResultTransformer(Transformers.AliasToBean<BoxSettingsDto>());
                return boxSettingQuery.UniqueResult<BoxSettingsDto>();
            }
        }


        #region Account Settings
        public async Task<User.UserAccountDto> GetUserAccountDetailsAsync(GetUserDetailsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var result = await conn.QueryAsync<User.UserAccountDto>(Sql.Sql.GetUserAccountData, new { query.UserId });
                return result.First();
            }
        }
        /// <summary>
        /// Used in account settings notification section
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Box.BoxNotificationDto>> GetUserBoxesNotificationAsync(GetUserDetailsQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Box.BoxNotificationDto>(@"select b.BoxId as Id, b.BoxName as Name,  ub.NotificationSettings as Notifications, b.Url as url, u.UserName
                    from zbox.Box b 
	                inner join zbox.UserBoxRel ub on b.BoxId = ub.BoxId
					inner join zbox.Users u on b.OwnerId = u.UserId
                    and ub.UserId = @UserId", new { query.UserId });
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
        public async Task<IEnumerable<long>> GetBoxUsersIdAsync(GetBoxWithUserQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<long>(Sql.Box.BoxUserIds,
                    new
                    {
                        query.BoxId,
                        pageNumber = query.PageNumber,
                        rowsperpage = query.RowsPerPage,
                        query.UserId
                    });
            }
        }

        #endregion


        public async Task<bool> GetInviteAsync(GetInviteDetailQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sqlQuery = @"select count(*) from   zbox.Message 
                                where TypeOfMsg = 2
                                and MessageId = @MessageId
                                and isActive = 1";
                var count = await conn.QueryAsync<int>(sqlQuery, new { query.MessageId });

                return count.FirstOrDefault() > 0;
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
                var retVal = await conn.QueryAsync<BoxDto>(Sql.Sql.UserWithFriendBoxes,
                    new
                    {
                        Me = query.UserId,
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
                return await conn.QueryAsync<Item.QuizDto>(Sql.Sql.UserWithFriendQuizzes, new
                {
                    Me = query.UserId,
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
                return await conn.QueryAsync<Item.ItemDto>(Sql.Sql.UserWithFriendFiles, new
                  {
                      Me = query.UserId,
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
                    Me = query.UserId,
                    Myfriend = query.FriendId,
                    pageNumber = query.PageNumber,
                    rowsperpage = query.RowsPerPage
                });
            }
        }

        #endregion

        #region Seo
        public async Task<IEnumerable<string>> GetSeoItemsAsync(int page)
        {
            const int pageSize = 49950;
            if (page < 1)
            {
                return null;
            }
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<string>(String.Format("{0}", Sql.Seo.GetSeoItemsByPage),
                    new { rowsperpage = pageSize, pageNumber = page });
            }
        }

        public async Task<int> GetSeoItemCountAsync()
        {
            const int pageSize = 49950;
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<int>(String.Format("{0}", Sql.Seo.GetSeoItemsCount));
                return (retVal.FirstOrDefault() / pageSize) + 1;
            }
        }

        #endregion

        //#region Admin
        //public async Task<IEnumerable<User.AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query)
        //{
        //    using (var conn = await DapperConnection.OpenConnectionAsync())
        //    {
        //        return await conn.QueryAsync<User.AdminUserDto>(Sql.Admin.UsersInUniversity,
        //            new
        //            {
        //                universityId = query.UniversityId
        //            });
        //    }
        //}
        //#endregion



        #region Quiz

        public async Task<Item.QuizSeo> GetQuizSeoAsync(GetQuizSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Item.QuizSeo>(Sql.Quiz.QuizSeoQuery, new { query.QuizId });
                return retVal.First();
            }
        }

        public async Task<Item.FileSeo> GetItemSeoAsync(GetFileSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Item.FileSeo>(Sql.Seo.FileSeo, new { query.ItemId });
                return retVal.FirstOrDefault();
            }
        }

        public async Task<Box.BoxSeoDto> GetBoxSeoAsync(GetBoxSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Box.BoxSeoDto>(Sql.Seo.BoxSeo, new { query.BoxId });
                var box = retVal.FirstOrDefault();
                if (retVal == null)
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
                var sql = string.Format("{0} {1}",
                    Sql.Quiz.TopUsers,
                    Sql.Quiz.NumberOfQuizSolved
                    );
                using (
                    var grid =
                        await conn.QueryMultipleAsync(sql, new { query.QuizId, topusers = query.NumberOfUsers }))
                {
                    var retVal = new Item.QuizSolversWithCountDto
                    {
                        Users = await grid.ReadAsync<Item.QuizBestUser>(),
                        SolversCount = grid.Read<int>().FirstOrDefault()
                    };

                    return retVal;
                }
            }
        }

        public async Task<Item.QuizQuestionWithSolvedAnswersDto> GetQuizQuestionWithAnswersAsync(GetQuizQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = string.Format("{0} {1} {2} {3}",
                   Sql.Quiz.Question,
                   Sql.Quiz.Answer,
                   Sql.Quiz.UserAnswer,
                   Sql.Quiz.UserQuiz

                   );
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
                    retVal.Sheet = grid.Read<Item.SolveSheet>().FirstOrDefault();
                    return retVal;
                }
            }
        }

        public async Task<Item.QuizWithDetailSolvedDto> GetQuizAsync(GetQuizQuery query)
        {
            var retVal = new Item.QuizWithDetailSolvedDto();
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = string.Format("{0} {1} {2} {3} {4} {5}",
                    Sql.Quiz.QuizQuery,
                    Sql.Quiz.Question,
                    Sql.Quiz.Answer,

                    Sql.Quiz.UserQuiz,
                    Sql.Quiz.UserAnswer,
                    Sql.Quiz.TopUsers
                    );
                using (var grid = await conn.QueryMultipleAsync(sql, new { query.QuizId, query.BoxId, query.UserId, topusers = 3 }))
                {
                    retVal.Quiz = grid.Read<Item.QuizWithDetailDto>().First();
                    retVal.Quiz.Questions = await grid.ReadAsync<Item.QuestionWithDetailDto>();
                    var answers = grid.Read<Item.AnswerWithDetailDto>().ToList();

                    foreach (var question in retVal.Quiz.Questions)
                    {
                        question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                    }
                    retVal.Sheet = grid.Read<Item.SolveSheet>().FirstOrDefault();
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
                using (var grid = await conn.QueryMultipleAsync(string.Format("{0} {1} {2}",
                    Sql.Quiz.QuizQuery,
                    Sql.Quiz.Question,
                    Sql.Quiz.Answer), new { query.QuizId }))
                {
                    var retVal = grid.Read<Item.QuizWithDetailDto>().First();
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
                var retVal = await conn.QueryAsync<int>(Sql.Quiz.NumberOfQuizSolved, new { QuizId = quizId });
                return retVal.FirstOrDefault();
            }
        }
        #endregion

        public async Task<IEnumerable<long>> GetUniversityWithCode()
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
    }
}
