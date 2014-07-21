using Dapper;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.Dashboard;
using Zbang.Zbox.ViewModel.DTOs.Library;
using Zbang.Zbox.ViewModel.DTOs.Search;
using Zbang.Zbox.ViewModel.DTOs.Store;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.Queries.User;
using Activity = Zbang.Zbox.ViewModel.DTOs.ActivityDtos;
using Box = Zbang.Zbox.ViewModel.DTOs.BoxDtos;
using ExtensionTransformers = Zbang.Zbox.Infrastructure.Data.Transformers;
using Item = Zbang.Zbox.ViewModel.DTOs.ItemDtos;
using Qna = Zbang.Zbox.ViewModel.DTOs.Qna;
using User = Zbang.Zbox.ViewModel.DTOs.UserDtos;
using Sql = Zbang.Zbox.ViewModel.SqlQueries;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadService : BaseReadService, IZboxReadService
    {
        public ZboxReadService(IHttpContextCacheWrapper contextCacheWrapper)
            : base(contextCacheWrapper)
        {
        }
        // 

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
                using (var grid = await conn.QueryMultipleAsync(Sql.Sql.UserBoxes + Sql.Sql.FriendList + Sql.Sql.GetWallList, new { query.UserId }))
                {
                    retVal.Boxes = grid.Read<BoxDto>();
                    retVal.Friends = grid.Read<User.UserDto>();
                    retVal.Wall = grid.Read<WallDto>();
                }
                return retVal;
            }
        }



        /// <summary>
        /// used in dashboard to get user lib in top left
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<UniversityDashboardInfoDto> GetMyData(GetDashboardQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sqlQuery = @"select uWrap.userName as Name, uWrap.UserImageLarge as Img  , uWrap.AdvertismentUrl as AdvertismentUrl
                  from zbox.users uWrap  
                  where uWrap.UserId = @univerSityDbQuery";

                var retVal = await conn.QueryAsync<UniversityDashboardInfoDto>(sqlQuery, new { univerSityDbQuery = query.UniversityId });
                return retVal.FirstOrDefault();
            }
        }

        /// <summary>
        /// University page - used to get parentNode, sub nodes or boxes in the current node
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public NodeBoxesDto GetLibraryNode(GetLibraryNodeQuery query)
        {
            using (UnitOfWork.Start())
            {
                const int defaultPageSize = 50;

                var dbNode = UnitOfWork.CurrentSession.GetNamedQuery(query.libraryQuery);
                dbNode.SetProperties(query);
                dbNode.SetResultTransformer(Transformers.AliasToBean<NodeDto>());
                dbNode.SetMaxResults(defaultPageSize);
                dbNode.SetFirstResult(query.PageNumber * defaultPageSize);
                var fnodeResult = dbNode.Future<NodeDto>();

                IEnumerable<BoxDto> fboxesResult = new List<BoxDto>();
                if (query.ParentNode.HasValue)
                {
                    var boxesQuery = UnitOfWork.CurrentSession.GetNamedQuery("ZboxGetAcademibBoxesByNode");
                    boxesQuery.SetReadOnly(true);
                    boxesQuery.SetParameter("ParentNode", query.ParentNode);
                    boxesQuery.SetEnum("Sort", query.Sort);
                    boxesQuery.SetParameter("UserId", query.UserId);
                    boxesQuery.SetMaxResults(defaultPageSize);
                    boxesQuery.SetFirstResult(query.PageNumber * defaultPageSize);
                    boxesQuery.SetResultTransformer(Transformers.AliasToBeanConstructor(typeof(BoxDto).GetConstructors()[1]));
                    fboxesResult = boxesQuery.Future<BoxDto>();
                }

                IFutureValue<NodeDto> fParent = null;
                if (query.ParentNode.HasValue)
                {
                    var dbQueryParentNode = UnitOfWork.CurrentSession.GetNamedQuery("GetParentNode");
                    dbQueryParentNode.SetReadOnly(true);
                    dbQueryParentNode.SetParameter("ParentNode", query.ParentNode);
                    dbQueryParentNode.SetParameter("UserId", query.UniversityId);
                    dbQueryParentNode.SetResultTransformer(Transformers.AliasToBean<NodeDto>());
                    fParent = dbQueryParentNode.FutureValue<NodeDto>();
                }
                var nodes = fnodeResult.ToList();
                var boxes = fboxesResult.ToList();
                //var boxesCount = fboxesCountResult == null ? 0 : fboxesCountResult.Value;
                var parent = fParent == null ? null : fParent.Value;

                if (nodes.Any() && boxes.Any())
                {
                    throw new InvalidOperationException("cannot have boxes and subnodes in the same node");
                }
                var result = new NodeBoxesDto(nodes, nodes.Count, boxes, 0, parent);
                return result;

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
                var retVal = await conn.QueryAsync<UniversityInfoDto>(Sql.Sql.GetUniversityDataByUserId, new { query.UserId, UniversityWrapper = query.UniversityWrapperId });
                return retVal.FirstOrDefault();
            }
            //using (UnitOfWork.Start())
            //{
            //    var dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUniversityDataByUserId");
            //    dbQuery.SetInt64("UserId", query.UserId);
            //    dbQuery.SetInt64("UniversityWrapper", query.UniversityWrapperId);

            //    dbQuery.SetReadOnly(true);
            //    dbQuery.SetResultTransformer(Transformers.AliasToBean<UniversityInfoDto>());
            //    var fdbQuery = dbQuery.FutureValue<UniversityInfoDto>();


            //    var result = fdbQuery.Value ?? new UniversityInfoDto();
            //    return result;
            //}
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

                var fbox = boxQuery.FutureValue<Box.BoxMetaDto>();
                CheckIfUserAllowedToSee(query.BoxId, query.UserId);

                var retVal = fbox.Value;
                return retVal;
            }
        }



        /// <summary>
        /// Used in box page - give the box detail
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Box.BoxDto GetBox(GetBoxQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery boxQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBox");
                boxQuery.SetInt64("BoxId", query.BoxId);
                boxQuery.SetResultTransformer(Transformers.AliasToBean<Box.BoxDto>());


                IQuery membersQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxSubscribers");
                membersQuery.SetInt64("BoxId", query.BoxId);
                membersQuery.SetResultTransformer(Transformers.AliasToBean<User.UserDto>());
                membersQuery.SetMaxResults(7);

                IQuery tabsQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxTabs");
                tabsQuery.SetInt64("BoxId", query.BoxId);
                tabsQuery.SetResultTransformer(Transformers.AliasToBean<TabDto>());

                var fbox = boxQuery.FutureValue<Box.BoxDto>();
                var fmembers = membersQuery.Future<User.UserDto>();
                var fTab = tabsQuery.Future<TabDto>();
                //var fParent = boxLibQuery.FutureValue<NodeDto>();

                var userType = CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                var box = fbox.Value;

                if (box == null)
                    throw new BoxDoesntExistException();
                box.Subscribers = fmembers.ToList();
                box.Tabs = fTab.ToList();
                box.UserType = userType;
                // box.Parent = fParent.Value;
                return box;
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
                queryBoxItem.SetParameter("TabId", query.TabId);
                queryBoxItem.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClassesCtorTransformer(typeof(Item.FileDto), typeof(Item.LinkDto)));


                var queryQuiz = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxQuiz");
                queryQuiz.SetInt64("BoxId", query.BoxId)
                    .SetResultTransformer(Transformers.AliasToBean<Item.QuizDto>());


                var fitems = queryBoxItem.Future<Item.ItemDto>();
                var fQuiz = queryQuiz.Future<Item.QuizDto>();

                IEnumerable<Item.IItemDto> items = fitems.ToList();
                var quizes = fQuiz.ToList();
                var retVal = items.Union(quizes);

                CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                return retVal;
            }

        }


        /// <summary>
        /// User in Item page to get an item
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Item.ItemWithDetailDto GetItem(GetItemQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetItem");
                dbQuery.SetInt64("ItemId", query.ItemId);
                dbQuery.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClassesCtorTransformer(typeof(Item.FileWithDetailDto), typeof(Item.LinkWithDetailDto)));

                var item = dbQuery.FutureValue<Item.ItemWithDetailDto>();
                var type = CheckIfUserAllowedToSee(query.BoxId, query.UserId);

                if (item.Value == null)
                {
                    throw new ItemNotFoundException();
                }
                var retVal = item.Value;
                retVal.UserType = type;
                return retVal;
            }
        }

        public async Task<ItemRateDto> GetRate(GetItemRateQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                //IEnumerable<Activity.AnnotationDto> retVal;
                const string sql = @"select Rate from zbox.itemRate where itemid = @itemId and ownerid = @userid";
                var retVal = await conn.QueryAsync<ItemRateDto>(sql, new { itemId = query.ItemId, userid = query.UserId });
                return retVal.FirstOrDefault();
            }
        }
        /// <summary>
        /// Used in box page - get the comment related to that box
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<Activity.BaseActivityDto> GetBoxComments(GetBoxCommentsQuery query)
        {
            using (UnitOfWork.Start())
            {
                var dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxComments");
                dbQuery.SetParameter("boxid", query.BoxId);
                dbQuery.SetResultTransformer(ExtensionTransformers.Transformers.AliasToCompositeClasses<Activity.CommentDto>());
                var comments = dbQuery.Future<Activity.BaseActivityDto>();

                CheckIfUserAllowedToSee(query.BoxId, query.UserId);

                return comments.ToList();
            }
        }

        public IEnumerable<Qna.QuestionDto> GetQuestions(GetBoxQuestionsQuery query)
        {
            using (UnitOfWork.Start())
            {
                var questionDbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxQuestion");
                questionDbQuery.SetInt64("boxId", query.BoxId);
                questionDbQuery.SetResultTransformer(Transformers.AliasToBean<Qna.QuestionDto>());
                var fquestion = questionDbQuery.Future<Qna.QuestionDto>();

                var answerDbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxAnswers");
                answerDbQuery.SetInt64("boxId", query.BoxId);
                answerDbQuery.SetInt64("userId", query.UserId);
                answerDbQuery.SetResultTransformer(Transformers.AliasToBean<Qna.AnswerDto>());
                var fanswer = answerDbQuery.Future<Qna.AnswerDto>();

                var itemsDbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxQnAItem");
                itemsDbQuery.SetInt64("boxId", query.BoxId);
                itemsDbQuery.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClassesCtorTransformer(typeof(Qna.FileDto), typeof(Qna.LinkDto)));



                var fItems = itemsDbQuery.Future<Qna.ItemDto>();


                CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                var questions = fquestion.ToList();
                var answers = fanswer.ToList();
                IEnumerable<Qna.ItemDto> items = fItems.ToList();

                foreach (var answer in answers)
                {
                    answer.Files.AddRange(items.Where(w => w.AnserId.HasValue && w.AnserId.Value == answer.Id));

                }
                foreach (var question in questions)
                {
                    question.Files.AddRange(items.Where(w => w.QuestionId.HasValue && w.QuestionId.Value == question.Id));
                    question.Answers.AddRange(answers.Where(s => s.QuestionId == question.Id));
                }


                return questions;
            }

        }


        /// <summary>
        /// Used in item page - get the comment related to that item
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Activity.AnnotationDto>> GetItemComments(GetItemCommentsQuery query)
        {

            using (IDbConnection conn = await DapperConnection.OpenConnectionAsync())
            {
                //IEnumerable<Activity.AnnotationDto> retVal;
                const string sql = @"SELECT [ItemCommentId] as Id
                          ,[ImageId]
                          ,[CordX] as X
                          ,[CordY] as Y
                          ,[Width]
                          ,[Height]
                          ,[Comment]
                          ,ic.CreationTime as CreationDate
	                      ,u.UserImage
	                      ,u.UserName
                          ,u.userid as Uid
	  
                      FROM [Zbox].[ItemComment] ic join zbox.Users u on ic.UserId = u.UserId
                      where itemid = @itemId
                      order by imageid, Y ";

                return await conn.QueryAsync<Activity.AnnotationDto>(sql, new { itemId = query.ItemId });
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
            SearchDto retVal = new SearchDto();
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
                            from zbox.users u left join zbox.users uu on u.UniversityId2 = uu.UserId
                            where u.userid = @UserId";
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
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetLocationByIP");
                dbQuery.SetInt64("IP", ipNumber);
                return dbQuery.UniqueResult<string>();
            }
        }


        public async Task<IEnumerable<DepartmentDto>> GetDepartmentList(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<DepartmentDto>(Sql.LibraryChoose.GetDepartments, new
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
            using (UnitOfWork.Start())
            {
                var queryUser = UnitOfWork.CurrentSession.GetNamedQuery("GetUserData");

                queryUser.SetInt64("UserId", query.UserId);
                queryUser.SetResultTransformer(Transformers.AliasToBean<User.UserDetailDto>());
                var user = queryUser.UniqueResult<User.UserDetailDto>();
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
                var fresult = dbQuery.Future<User.UserMemberDto>();
                CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                return fresult.ToList();
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
        public async Task<IEnumerable<ViewModel.DTOs.UserDtos.AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<ViewModel.DTOs.UserDtos.AdminUserDto>(Sql.Admin.UsersInUniversity,
                    new
                    {
                        universityId = query.UniversityId
                    });
            }
        }
        #endregion

        #region Quiz
        public async Task<Item.QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query)
        {
            var retVal = new Item.QuizWithDetailSolvedDto();
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = string.Format("{0} {1} {2} {3} {4} {5} {6}",
                    Sql.Quiz.QuizQuery,
                    Sql.Quiz.Question,
                    Sql.Quiz.Answer,
                    Sql.Security.GetBoxPrivacySettings,
                    Sql.Security.GetUserToBoxRelationShip,
                    Sql.Quiz.UserQuiz,
                    Sql.Quiz.UserAnswer);

                if (query.NeedCountry)
                {
                    sql = sql + " " + Sql.Quiz.QuizSeoQuery;
                }
                using (var grid = await conn.QueryMultipleAsync(sql, new { query.QuizId, query.BoxId, query.UserId }))
                {
                    retVal.Quiz = grid.Read<Item.QuizWithDetailDto>().First();
                    retVal.Quiz.Questions = grid.Read<Item.QuestionWithDetailDto>();

                    var answers = grid.Read<Item.AnswerWithDetailDto>().ToList();

                    foreach (var question in retVal.Quiz.Questions)
                    {
                        question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                    }

                    var privacySettings = grid.Read<BoxPrivacySettings>().First();
                    var userRelationShip = grid.Read<UserRelationshipType>().FirstOrDefault();
                    GetUserStatusToBox(privacySettings, userRelationShip);

                    retVal.Sheet = grid.Read<Item.SolveSheet>().FirstOrDefault();
                    var solvedQuestion = grid.Read<Item.SolveQuestion>();
                    if (retVal.Sheet != null)
                    {
                        retVal.Sheet.Questions = solvedQuestion;
                    }

                    if (query.NeedCountry)
                    {
                        retVal.Quiz.Seo = grid.Read<Item.QuizSeo>().FirstOrDefault();
                    }
                }
            }
            return retVal;
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
                using (var grid = await conn.QueryMultipleAsync(string.Format("{0} {1} {2}", Sql.Quiz.QuizQuery, Sql.Quiz.Question, Sql.Quiz.Answer), new { query.QuizId }))
                {
                    Item.QuizWithDetailDto retVal = grid.Read<Item.QuizWithDetailDto>().First();
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
        #endregion

        #region Store

        public async Task<IEnumerable<ProductDto>> GetProducts(GetStoreProductByCategoryQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var sql = query.CategoryId.HasValue ? Sql.Store.GetProductsWithCategory : Sql.Store.GetProducts;
                return await conn.QueryAsync<ProductDto>(sql, new { CatId  = query.CategoryId});
            }
        }
        public IEnumerable<CategoryDto> GetCategories()
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<CategoryDto>(Sql.Store.GetCategories);
            }
        }
        #endregion
    }
}
