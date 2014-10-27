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
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.ViewModel.Dto.Dashboard;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Dto.Store;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.Queries.User;
using Activity = Zbang.Zbox.ViewModel.Dto.ActivityDtos;
using Box = Zbang.Zbox.ViewModel.Dto.BoxDtos;
using ExtensionTransformers = Zbang.Zbox.Infrastructure.Data.Transformers;
using Item = Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Qna = Zbang.Zbox.ViewModel.Dto.Qna;
using User = Zbang.Zbox.ViewModel.Dto.UserDtos;
using Sql = Zbang.Zbox.ViewModel.SqlQueries;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadService : BaseReadService, IZboxReadService
    {


        /// <summary>
        /// used to get the dashboard and the activity and wall in dashboard
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<DashboardDto> GetDashboard(GetBoxesQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = new DashboardDto();
                using (var grid = await conn.QueryMultipleAsync(Sql.Sql.UserBoxes + Sql.Sql.FriendList + Sql.Sql.GetWallList,
                    new { query.UserId }))
                {
                    retVal.Boxes = await grid.ReadAsync<BoxDto>();
                    retVal.Friends = await grid.ReadAsync<User.UserDto>();
                    retVal.Wall = await grid.ReadAsync<WallDto>();
                }
                return retVal;
            }

        }

        /// <summary>
        /// Used in empty state dashboard
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Box.RecommendBoxDto>> GetRecommendedCourses(QueryBase query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Box.RecommendBoxDto>(Sql.Sql.RecommendedCourses, new { query.UserId });
            }
        }



        /// <summary>
        /// used in dashboard to get user lib in top left
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<UniversityDashboardInfoDto> GetMyData(GetDashboardQuery query)
        {
            const string sqlQuery = @"select coalesce( uWrap.OrgName , uWrap.universityName) as Name,
                uWrap.LargeImage as Img  , uWrap.AdvertisementUrl as AdvertisementUrl, NoOfBoxes as NoOfBoxes
                  from zbox.university uWrap  
                  where uWrap.Id = @universityDbQuery";

            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<UniversityDashboardInfoDto>(sqlQuery, new { universityDbQuery = query.UniversityId });
                return retVal.FirstOrDefault();
            }
        }

        /// <summary>
        /// University page - sub nodes or boxes in the current node
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<NodeBoxesDto> GetLibraryNode(GetLibraryNodeQuery query)
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
        public async Task<UniversityInfoDto> GetUniversityDetail(GetUniversityDetailQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<UniversityInfoDto>(Sql.Sql.GetUniversityDataByUserId, new { UniversityWrapper = query.UserId });
                return retVal.FirstOrDefault();
            }

        }

        /// <summary>
        /// Get user invite - header page section
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<InviteDto>> GetInvites(GetInvitesQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<InviteDto>(Sql.Sql.UserInvites, new { query.UserId });
            }
        }

        /// <summary>
        /// Get box meta data for invite page
        /// </summary>
        /// <param name="query"></param>
        public Box.BoxMetaDto GetBoxMeta(GetBoxQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery boxQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxMetaForInvite");
                boxQuery.SetInt64("BoxId", query.BoxId);
                boxQuery.SetResultTransformer(Transformers.AliasToBean<Box.BoxMetaDto>());

                var fBox = boxQuery.FutureValue<Box.BoxMetaDto>();
                var type = CheckIfUserAllowedToSee(query.BoxId, query.UserId);

                var retVal = fBox.Value;
                retVal.RelationshipType = type;
                return retVal;
            }
        }



        /// <summary>
        /// Used in box page - give the box detail
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Obsolete("Use Get box 2")]
        public ViewModel.Dto.BoxDtos.BoxDto GetBox(GetBoxQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery boxQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBox");
                boxQuery.SetInt64("BoxId", query.BoxId);
                boxQuery.SetResultTransformer(Transformers.AliasToBean<ViewModel.Dto.BoxDtos.BoxDto>());


                IQuery membersQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxSubscribers");
                membersQuery.SetInt64("BoxId", query.BoxId);
                membersQuery.SetResultTransformer(Transformers.AliasToBean<User.UserDto>());
                membersQuery.SetMaxResults(7);

                IQuery tabsQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxTabs");
                tabsQuery.SetInt64("BoxId", query.BoxId);
                tabsQuery.SetResultTransformer(Transformers.AliasToBean<TabDto>());

                var fBox = boxQuery.FutureValue<ViewModel.Dto.BoxDtos.BoxDto>();
                var fMembers = membersQuery.Future<User.UserDto>();
                var fTab = tabsQuery.Future<TabDto>();
                //var fParent = boxLibQuery.FutureValue<NodeDto>();

                var userType = CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                var box = fBox.Value;

                if (box == null)
                    throw new BoxDoesntExistException();
                box.Subscribers = fMembers.ToList();
                box.Tabs = fTab.ToList();
                box.UserType = userType;
                // box.Parent = fParent.Value;
                return box;
            }
        }

        public async Task<Box.BoxDto2> GetBox2(GetBoxQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var data = await conn.QueryAsync<Box.BoxDto2>(Sql.Box.BoxData, new { query.BoxId, query.UserId });
                var retVal = data.FirstOrDefault();
                if (retVal == null)
                {
                    throw new BoxDoesntExistException();
                }
                return retVal;
            }
        }



        public async Task<IEnumerable<TabDto>> GetBoxTabs(GetBoxQuery query)
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
        /// <returns></returns>
        public NotificationSettings GetUserBoxNotificationSettings(GetBoxQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxNotificationByUser");
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetInt64("UserId", query.UserId);

                return dbQuery.UniqueResult<NotificationSettings>();
                //dbQuery.SetResultTransformer(Transformers.AliasToBean<NotificationSettings>());
            }
        }

        /// <summary>
        /// Used in box page - get the item related to that page
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<Item.IItemDto> GetBoxItemsPaged2(GetBoxItemsPagedQuery query)
        {
            using (UnitOfWork.Start())
            {

                var queryBoxItem = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxItemDtosByBoxId2");
                queryBoxItem.SetInt64("BoxId", query.BoxId);
                queryBoxItem.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClassesCtorTransformer(typeof(Item.FileDto), typeof(Item.LinkDto)));
                var fitems = queryBoxItem.Future<Item.ItemDto>();
                return fitems.ToList();
            }

        }

        public async Task< IEnumerable<Item.QuizDto>> GetBoxQuizes(GetBoxItemsPagedQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Item.QuizDto>(Sql.Quiz.GetBoxQuiz, new {BoxId = query.BoxId});
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

        public async Task<Item.ItemDetailDto> GetItem2(GetItemQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (
                    var grid =
                        await
                            conn.QueryMultipleAsync(string.Format("{0} {1} {2} {3} {4}",
                            Sql.Item.ItemDetail,
                            Sql.Item.Navigation,
                            Sql.Item.ItemComments,
                            Sql.Item.ItemCommentReply,
                            Sql.Item.UserItemRate),
                                new { query.ItemId, query.BoxId, query.UserId }))
                {
                    var retVal = grid.Read<Item.ItemDetailDto>().FirstOrDefault();
                    if (retVal == null)
                    {
                        throw new ItemNotFoundException();
                    }
                    retVal.Navigation = grid.Read<Item.ItemNavigationDto>().FirstOrDefault();
                    retVal.Comments = await grid.ReadAsync<Activity.AnnotationDto>();


                    IEnumerable<Activity.AnnotationReplyDto> replies =
                        grid.Read<Activity.AnnotationReplyDto>().ToList();

                    foreach (var comment in retVal.Comments)
                    {
                        comment.Replies.AddRange(replies.Where(w => w.ParentId == comment.Id));

                    }
                    retVal.Rate = grid.Read<int>().FirstOrDefault();
                    return retVal;
                }

            }
        }




        public async Task<Qna.FeedDto> GetQuestions(GetBoxQuestionsQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await con.QueryMultipleAsync(string.Format("{0} {1} {2} {3}",
                    Sql.Box.GetBoxQuestion,
                    Sql.Box.GetBoxAnswers,
                    Sql.Box.GetBoxQnAItem,
                    Sql.Box.RecommendedCourses),
                    new { query.BoxId, query.UserId }))
                {
                    var retVal = new Qna.FeedDto();
                    var questions = grid.Read<Qna.QuestionDto>().ToList();
                    var answers = grid.Read<Qna.AnswerDto>().ToList();
                    var items = grid.Read<Qna.ItemDto>().ToList();

                    foreach (var answer in answers)
                    {
                        answer.Files.AddRange(items.Where(w => w.AnswerId.HasValue && w.AnswerId.Value == answer.Id));

                    }
                    foreach (var question in questions)
                    {
                        question.Files.AddRange(items.Where(w => w.QuestionId.HasValue && w.QuestionId.Value == question.Id));
                        question.Answers.AddRange(answers.Where(s => s.QuestionId == question.Id));
                    }
                    retVal.Feed = questions;
                    retVal.RecommendBoxes = await grid.ReadAsync<Box.RecommendBoxDto>();

                    return retVal;
                }
            }
        }





        /// <summary>
        /// Performs a search, returning the results grouped by category 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        #region Search
        public async Task<SearchDto> Search(GroupSearchQuery query)
        {
            var retVal = new SearchDto();
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {

                using (var grid = await conn.QueryMultipleAsync(string.Format("{0} {1} {2} {3}",
                     Sql.Search.OwnedSubscribedBoxes,
                     Sql.Search.UniversityBoxes,
                     Sql.Search.Users,
                     Sql.Search.Items
                     ),
                     new
                     {
                         query = query.Query,
                         universityId = query.UniversityId,
                         userId = query.UserId,
                         offsetV = query.Offset,
                         pageSize = query.PageSize
                     }))
                {
                    var ownedBoxes = grid.Read<SearchBoxes>();
                    var universityBoxes = grid.Read<SearchBoxes>();
                    retVal.Users = grid.Read<SearchUsers>();
                    retVal.Items = grid.Read<SearchItems>();


                    retVal.Boxes = ownedBoxes.Union(universityBoxes, new SearchBoxesComparer()).Take(query.PageSize);
                }
                if (retVal.Items.Any()) return retVal;
                retVal.OtherItems = await conn.QueryAsync<SearchItems>(Sql.Search.ItemFromOtherUniversities,
                    new
                    {
                        query = query.Query,
                        universityId = query.UniversityId,
                        userId = query.UserId,
                        offsetV = query.Offset,
                        pageSize = query.PageSize
                    });
                retVal.OtherItems = retVal.OtherItems;
            }
            return retVal;
        }

        public async Task<IEnumerable<SearchItems>> OtherUniversities(GroupSearchQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<SearchItems>(Sql.Search.ItemFromOtherUniversities,
                       new
                       {
                           query = query.Query,
                           universityId = query.UniversityId,
                           userId = query.UserId,
                           offsetV = query.Offset,
                           pageSize = query.PageSize
                       });
            }
        }

        #endregion
        /// <summary>
        /// Used for autocomplete in the share box dialog
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User.UserDto>> GetUserFriends(GetUserFriendsQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<User.UserDto>(Sql.Sql.FriendList, new { query.UserId });
            }
        }

        public async Task<User.UserMinProfile> GetUserMinProfile(GetUserMinProfileQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = @"select u.userid as Id, u.username as name, u.UserImageLarge as image,
                            u.userReputation as score, uu.universityname as universityName, u.url as Url
                            from zbox.users u left join zbox.university uu on u.UniversityId = uu.id
                            where u.userid =@UserId";
                var retVal = await conn.QueryAsync<User.UserMinProfile>(sql, new { query.UserId });
                return retVal.FirstOrDefault();

            }
        }

        /// <summary>
        /// Get The country the user is in based on the ip address
        /// </summary>
        /// <returns></returns>
        public string GetLocationByIp(long ipNumber)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                const string sql = @" select country_code2  from zbox.ip_range 
    where ip_from <= @IP and @IP <= ip_to";
                var retVal = conn.Query<string>(sql, new { IP = ipNumber });
                return retVal.FirstOrDefault();
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

        public async Task<bool> GetUniversityNeedId(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<int>(Sql.LibraryChoose.GetNeedId, new
                {
                    universityId
                });
                return retVal.FirstOrDefault() > 0;
            }
        }

        public async Task<bool> GetUniversityNeedCode(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<bool>(Sql.LibraryChoose.GetNeedCode, new
                {
                    universityId
                });
                return retVal.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<UniversityByFriendDto>> GetUniversityListByFriendsIds(IEnumerable<long> friendsIds)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                //we can only use 2100 in statement
                using (var grid = await conn.QueryMultipleAsync(
                     string.Format("{0} {1}", Sql.LibraryChoose.GetUniversityByFriendIds, Sql.LibraryChoose.GetFriendsInUniversitiesByFriendsIds),
                    new { FriendsIds = friendsIds.Take(2099) }
                     ))
                {
                    var retVal = grid.Read<UniversityByFriendDto>();
                    var friends = grid.Read<FriendPerUniversityDto>();

                    retVal = retVal.Select(s =>
                       {
                           s.Friends = friends.Where(w => w.UniversityId == s.Id);
                           return s;
                       }).ToList();
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
                var retVal =

                        conn.Query<User.UserDetailDto>(Sql.Sql.UserAuthenticationDetail,
                            new { query.UserId });
                var userDetailDtos = retVal as User.UserDetailDto[] ?? retVal.ToArray();
                if (retVal == null || !userDetailDtos.Any())
                {
                    throw new UserNotFoundException("user is null");
                }
                return userDetailDtos.FirstOrDefault();
            }
        }


        /// <summary>
        /// Get box settings data for ajax request
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public BoxSettingsDto GetBoxSetting(GetBoxQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery boxSettingQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxSettingsDtoById");
                boxSettingQuery.SetParameter("Id", query.BoxId);
                boxSettingQuery.SetParameter("Userid", query.UserId);
                boxSettingQuery.SetResultTransformer(Transformers.AliasToBean<BoxSettingsDto>());
                return boxSettingQuery.UniqueResult<BoxSettingsDto>();
            }
        }


        #region Account Settings
        public User.UserAccountDto GetUserAccountDetails(GetUserDetailsQuery query)
        {
            using (UnitOfWork.Start())
            {
                var queryUser = UnitOfWork.CurrentSession.GetNamedQuery("GetUserAccountSettings");

                queryUser.SetInt64("UserId", query.UserId);
                queryUser.SetReadOnly(true);
                queryUser.SetResultTransformer(Transformers.AliasToBean<User.UserAccountDto>());
                var user = queryUser.UniqueResult<User.UserAccountDto>();
                if (user == null)
                {
                    throw new UserNotFoundException("user is null");
                }
                return user;
            }

        }
        /// <summary>
        /// Used in account settings notification section
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<Box.BoxNotificationDto> GetUserBoxesNotification(GetUserDetailsQuery query)
        {
            using (UnitOfWork.Start())
            {
                var dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserBoxesNotification");
                dbQuery.SetInt64("UserId", query.UserId);
                dbQuery.SetReadOnly(true);
                dbQuery.SetResultTransformer(Transformers.AliasToBeanConstructor(typeof(Box.BoxNotificationDto).GetConstructors()[0]));
                return dbQuery.List<Box.BoxNotificationDto>();

            }
        }

        /// <summary>
        /// Used in box page to get the members pop up
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<User.UserMemberDto> GetBoxMembers(GetBoxQuery query)
        {
            using (UnitOfWork.Start())
            {
                var dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxMembers");
                dbQuery.SetResultTransformer(Transformers.AliasToBean<User.UserMemberDto>());
                dbQuery.SetInt64("BoxId", query.BoxId);
                var fResult = dbQuery.Future<User.UserMemberDto>();
                CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                return fResult.ToList();
            }
        }

        #endregion


        public async Task<bool> GetInvite(GetInviteDetailQuery query)
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


        public async Task<IEnumerable<UpdatesDto>> GetUpdates(QueryBase query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<UpdatesDto>(Sql.Updates.GetUserUpdates, new { userid = query.UserId });
            }
        }

        #region UserPage
        public async Task<IEnumerable<Box.BoxToFriendDto>> GetUserWithFriendBoxes(GetUserWithFriendQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Box.BoxToFriendDto>(Sql.Sql.UserWithFriendBoxes, new { Me = query.UserId, Myfriend = query.FriendId });
                return retVal;
            }
        }
        public async Task<IEnumerable<Item.ItemToFriendDto>> GetUserWithFriendFiles(GetUserWithFriendQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Item.ItemToFriendDto>(Sql.Sql.UserWithFriendFiles, new { Me = query.UserId, Myfriend = query.FriendId });
                return retVal;
            }
        }

        public async Task<IEnumerable<Qna.QuestionToFriendDto>> GetUserWithFriendQuestion(GetUserWithFriendQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Qna.QuestionToFriendDto>(Sql.Sql.UserWithFriendQuestion, new { Me = query.UserId, Myfriend = query.FriendId });
                return retVal;
            }
        }

        public async Task<IEnumerable<Qna.AnswerToFriendDto>> GetUserWithFriendAnswer(GetUserWithFriendQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Qna.AnswerToFriendDto>(Sql.Sql.UserWithFriendAnswer, new { Me = query.UserId, Myfriend = query.FriendId });
                return retVal;
            }
        }

        public async Task<IEnumerable<User.UserInviteDto>> GetUserPersonalInvites(GetInvitesQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<User.UserInviteDto>(Sql.Sql.UserPersonalInvites, new { Me = query.UserId });
                return retVal;
            }
        }
        public async Task<User.UserToFriendActivity> GetUserWithFriendActivity(GetUserWithFriendQuery query)
        {
            var retVal = new User.UserToFriendActivity();
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(String.Format("{0} {1} {2}", Sql.Sql.UserWithFriendFiles,
                    Sql.Sql.UserWithFriendQuestion,
                    Sql.Sql.UserWithFriendAnswer), new { Me = query.UserId, Myfriend = query.FriendId }))
                {
                    retVal.Items = grid.Read<Item.ItemToFriendDto>();
                    retVal.Questions = grid.Read<Qna.QuestionToFriendDto>();
                    retVal.Answers = grid.Read<Qna.AnswerToFriendDto>();
                }
            }
            return retVal;
        }

        #endregion

        #region Seo
        public async Task<IEnumerable<string>> GetSeoItems(int page)
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

        public async Task<int> GetSeoItemCount()
        {
            const int pageSize = 49950;
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<int>(String.Format("{0}", Sql.Seo.GetSeoItemsCount));
                return (retVal.FirstOrDefault() / pageSize) + 1;
            }
        }

        #endregion

        #region Admin
        public async Task<IEnumerable<User.AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<User.AdminUserDto>(Sql.Admin.UsersInUniversity,
                    new
                    {
                        universityId = query.UniversityId
                    });
            }
        }
        #endregion



        #region Quiz

        public async Task<Item.QuizSeo> GetQuizSeo(GetQuizSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Item.QuizSeo>(Sql.Quiz.QuizSeoQuery, new { query.QuizId });
                return retVal.FirstOrDefault();
            }
        }

        public async Task<Item.FileSeo> GetItemSeo(GetFileSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<Item.FileSeo>(Sql.Seo.FileSeo, new { query.ItemId });
                return retVal.FirstOrDefault();
            }
        }

        public async Task<Box.BoxSeoDto> GetBoxSeo(GetBoxSeoQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var d = await conn.QueryAsync<Box.BoxSeoDto>(Sql.Seo.BoxSeo, new { query.BoxId });

                var retVal = d.FirstOrDefault();
                if (retVal == null)
                {
                    throw new BoxDoesntExistException();
                }
                return retVal;
            }
        }

        public async Task<Item.QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query)
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
                using (var grid = await conn.QueryMultipleAsync(sql, new { query.QuizId, query.BoxId, query.UserId }))
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
                    retVal.Quiz.TopUsers = await grid.ReadAsync<Item.QuizBestUsers>();
                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<Item.DiscussionDto>> GetDiscussion(GetDisscussionQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<Item.DiscussionDto>(Sql.Quiz.Discussion, new { query.QuizId });
            }
        }

        public async Task<Item.QuizWithDetailDto> GetDraftQuiz(GetQuizDraftQuery query)
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

        public async Task<int> GetNumberOfSolvers(long quizId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<int>(Sql.Quiz.NumberOfQuizSolved, new { QuizId = quizId });
                return retVal.FirstOrDefault();
            }
        }
        #endregion


        #region Store

        public async Task<IEnumerable<ProductDto>> GetProducts(GetStoreProductsByCategoryQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = query.CategoryId.HasValue ? Sql.Store.GetProductsWithCategory : Sql.Store.GetProducts;
                sql = query.ProducerId.HasValue ? Sql.Store.GetProductsBySupplier : sql;
                return await conn.QueryAsync<ProductDto>(sql, new { CatId = query.CategoryId, query.UniversityId, producerId = query.ProducerId });
            }
        }
        public async Task<IEnumerable<ProductDto>> SearchProducts(SearchProductQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<ProductDto>(Sql.Store.SearchProduct, new { term = query.Term, universityId = query.UniversityId });
            }
        }
        public IEnumerable<CategoryDto> GetCategories()
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<CategoryDto>(Sql.Store.GetCategories);
            }
        }
        public async Task<ProductWithDetailDto> GetProduct(GetStoreProductQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<ProductWithDetailDto>(Sql.Store.GetProduct, new { ProdId = query.ProductId });
                return retVal.FirstOrDefault();
            }
        }

        public async Task<ProductCheckOutDto> GetProductCheckOut(GetStoreProductQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(Sql.Store.GetProductCheckOut + Sql.Store.GetProductFeatures,
                    new { ProdId = query.ProductId }))
                {
                    var product = grid.Read<ProductCheckOutDto>().FirstOrDefault();
                    if (product != null) product.Features = grid.Read<ProductFeatures>();
                    return product;
                }
            }
        }

        public async Task<IEnumerable<BannerDto>> GetBanners(int? universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<BannerDto>(Sql.Store.GetBanners, new { universityId });
            }
        }

        public async Task<bool> ValidateCoupon(int coupon)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<int>(Sql.Store.ValidateCouponCode, new { Coupun = coupon });
                return retVal.FirstOrDefault() > 0;
            }
        }

        public async Task<int?> CloudentsUniversityToStoreUniversity(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await conn.QueryAsync<int>(Sql.Store.MapCloudentsUniversityToStoreUniversity, new { UniversityId = universityId });
                var enumerable = retVal as IList<int> ?? retVal.ToList();
                if (!enumerable.Any())
                {
                    return null;
                }
                return enumerable.FirstOrDefault();
            }
        }
        #endregion

    }
}
