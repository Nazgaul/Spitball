using Dapper;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.Dashboard;
using Zbang.Zbox.ViewModel.DTOs.Library;
using Zbang.Zbox.ViewModel.DTOs.Search;
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
        const int DefaultPageSize = 50;

        /// <summary>
        /// used to get the dashboard and the activity and wall in dashboard
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<DashboardDto> GetDashboard(GetBoxesQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                var retVal = new DashboardDto();
                using (var grid = conn.QueryMultiple(Sql.Sql.UserBoxes + Sql.Sql.FriendList + Sql.Sql.GetWallList, new { UserId = query.UserId }))
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
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                var sqlQuery = @"select uWrap.userName as Name, uWrap.UserImageLarge as Img  , uWrap.AdvertismentUrl as AdvertismentUrl
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

                var dbNode = UnitOfWork.CurrentSession.GetNamedQuery(query.libraryQuery);
                dbNode.SetProperties(query);
                dbNode.SetResultTransformer(Transformers.AliasToBean<NodeDto>());
                dbNode.SetMaxResults(DefaultPageSize);
                dbNode.SetFirstResult(query.PageNumber * DefaultPageSize);
                var fnodeResult = dbNode.Future<NodeDto>();



                IEnumerable<BoxDto> fboxesResult = new List<BoxDto>();
                if (query.ParentNode.HasValue)
                {
                    var boxesQuery = UnitOfWork.CurrentSession.GetNamedQuery("ZboxGetAcademibBoxesByNode");
                    boxesQuery.SetReadOnly(true);
                    boxesQuery.SetParameter("ParentNode", query.ParentNode);
                    boxesQuery.SetEnum("Sort", query.Sort);
                    //boxesQuery.SetParameter("UniversityId", query.UniversityId);
                    boxesQuery.SetParameter("UserId", query.UserId);
                    boxesQuery.SetMaxResults(DefaultPageSize);
                    boxesQuery.SetFirstResult(query.PageNumber * DefaultPageSize);
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
            using (var conn = await DapperConnection.OpenConnection())
            {
                var retVal = await conn.QueryAsync<UniversityInfoDto>(Sql.Sql.GetUniversityDataByUserId, new { UserId = query.UserId, UniversityWrapper = query.UniversityWrapperId });
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
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                var dbQeury = @"select * from (select u.UserImage as userpic,
 u.UserName as username,
 m.MessageId as msgId,
  m.CreationTime as date,
  m.NotRead as isread,
  m.New as IsNew,
  m.Text as message,
b.BoxName,
b.BoxId,
boxOwner.UniversityName  as Universityname
from zbox.message m 
inner join zbox.box b on m.BoxId = b.BoxId and b.IsDeleted = 0
inner join zbox.users u on u.UserId = m.SenderId
inner join zbox.users boxOwner on boxOwner.UserId = b.OwnerId
where m.RecepientId = @userid
 and TypeOfMsg = 2
 and isactive = 1
 union all
select u.UserImage as userpic,
 u.UserName as username,
 m.MessageId as msgId,
  m.CreationTime as date,
  m.NotRead as isread,
  m.New as IsNew,
  m.Text as message,
null,
null,
null  as Universityname
 from zbox.message m 
 inner join zbox.users u on u.UserId = m.SenderId
where m.RecepientId = @userid
 and TypeOfMsg = 1 ) t
 order by t.msgid desc

";

                return await conn.QueryAsync<InviteDto>(dbQeury, new { UserId = query.UserId });
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
                var userType = CheckIfUserAllowedToSee(query.BoxId, query.UserId);

                return fbox.Value;
            }
        }



        /// <summary>
        /// Used in box page - give the box detail
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Box.BoxDto GetBox2(GetBoxQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery boxQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBox");
                boxQuery.SetInt64("BoxId", query.BoxId);
                boxQuery.SetResultTransformer(Transformers.AliasToBean<Box.BoxDto>());


                IQuery membersQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxSubscribers");
                membersQuery.SetInt64("BoxId", query.BoxId);
                membersQuery.SetResultTransformer(Transformers.AliasToBean<User.UserDto>());
                membersQuery.SetMaxResults(11);

                IQuery tabsQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxTabs");
                tabsQuery.SetInt64("BoxId", query.BoxId);
                tabsQuery.SetResultTransformer(Transformers.AliasToBean<TabDto>());

                IQuery boxLibQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxLibNode");
                boxLibQuery.SetInt64("BoxId", query.BoxId);
                boxLibQuery.SetResultTransformer(Transformers.AliasToBean<NodeDto>());

                var fbox = boxQuery.FutureValue<Box.BoxDto>();
                var fmembers = membersQuery.Future<User.UserDto>();
                var fTab = tabsQuery.Future<TabDto>();
                var fParent = boxLibQuery.FutureValue<NodeDto>();

                var userType = CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                Box.BoxDto box = fbox.Value;

                if (box == null)
                    throw new BoxDoesntExistException();
                box.Subscribers = fmembers.ToList();
                box.Tabs = fTab.ToList();
                box.UserType = userType;
                box.Parent = fParent.Value;
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
        public PagedDto<Item.ItemDto> GetBoxItemsPaged2(GetBoxItemsPagedQuery query)
        {
            using (UnitOfWork.Start())
            {

                var queryBoxItem = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxItemDtosByBoxId2");
                queryBoxItem.SetReadOnly(true);
                queryBoxItem.SetInt64("BoxId", query.BoxId);
                // queryBoxItem.SetEnum("Order", query.Order);
                queryBoxItem.SetParameter("TabId", query.TabId);
                queryBoxItem.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClassesCtorTransformer(typeof(Item.FileDto), typeof(Item.LinkDto)));

                //queryBoxItem.SetFirstResult(query.PageNumber * query.MaxResult);
                //queryBoxItem.SetMaxResults(query.MaxResult);

                //IQuery queryCountBoxItem = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxItemCountByBoxId");
                //queryCountBoxItem.SetInt64("BoxId", query.BoxId);

                var fitems = queryBoxItem.Future<Item.ItemDto>();
                //var fcount = queryCountBoxItem.FutureValue<long>();

                // CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                var result = new PagedDto<Item.ItemDto> { Dto = fitems.ToList(), Count = 0 /*fcount.Value*/ };
                return result;
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
                dbQuery.SetReadOnly(true);
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
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                //IEnumerable<Activity.AnnotationDto> retVal;
                var sql = @"select Rate from zbox.itemRate where itemid = @itemId and ownerid = @userid";
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
                var items = fItems.ToList();
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

            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                //IEnumerable<Activity.AnnotationDto> retVal;
                var sql = @"SELECT [ItemCommentId] as Id
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
            using (var conn = await DapperConnection.OpenConnection())
            {

                using (var grid = conn.QueryMultiple(string.Format("{0} {1} {2} {3}",
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
                if (retVal.Items.Count() == 0)
                {
                    retVal.OtherItems = await conn.QueryAsync<SearchItems>(Sql.Search.ItemFromOtherUniversities,
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
            return retVal;
        }

        public async Task<IEnumerable<SearchItems>> OtherUniversities(GroupSearchQuery query)
        {
            using (var conn = await DapperConnection.OpenConnection())
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
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<User.UserDto>(Sql.Sql.FriendList, new { UserId = query.UserId });
            }
        }

        public async Task<User.UserMinProfile> GetUserMinProfile(GetUserMinProfileQuery query)
        {
            using (var conn = await DapperConnection.OpenConnection())
            {
                var sql = @"select u.userid as Id, u.username as name, u.UserImageLarge as image, u.userReputation as score, uu.username as universityName
                            from zbox.users u left join zbox.users uu on u.UniversityId2 = uu.UserId
                            where u.userid = @UserId";
                var retVal = await conn.QueryAsync<User.UserMinProfile>(sql, new { UserId = query.UserId });
                return retVal.FirstOrDefault();

            }
        }

        /// <summary>
        /// Get The country the user is in based on the ip address
        /// </summary>
        /// <returns></returns>
        public string GetLocationByIP(long ipNumber)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetLocationByIP");
                dbQuery.SetInt64("IP", ipNumber);
                return dbQuery.UniqueResult<string>();
            }
        }


        /// <summary>
        /// Used for University choose page
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UniversityByPrefixDto>> GetUniversityListByPrefix(GetUniversityByPrefixQuery query)
        {
            //const int pageNumber = 20;
            using (var conn = await DapperConnection.OpenConnection())
            {
                //var transferLetter = IocFactory.Unity.Resolve<IEnglishToHebrewChars>();
                //var stemmer = IocFactory.Unity.Resolve<IHebrewStemmer>();
                //var filter = IocFactory.Unity.Resolve<IFilterWords>();

                //var hebrewLetters = stemmer.StemAHebrewWord(transferLetter.TransferEnglishCharsToHebrew(query.Prefix));
                ////var hebrewLetters = transferLetter.TransferEnglishCharsToHebrew(query.Prefix);
                //hebrewLetters = filter.removeWords(hebrewLetters);
                //var filteredQuery = filter.removeWords(query.Prefix);

                return await conn.QueryAsync<UniversityByPrefixDto>(Sql.Sql.GetUniversitiesList);
                //    , new
                //{
                //    country = query.Country,
                //});
            }
        }

        public async Task<IEnumerable<DepartmentDto>> GetDepartmentList(long universityId)
        {
            using (var conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<DepartmentDto>(Sql.LibraryChoose.GetDepartments, new
                {
                    universityId = universityId
                });
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
            using (var conn = await DapperConnection.OpenConnection())
            {
                var sqlQuery = @"select count(*) from   zbox.Message 
                                where TypeOfMsg = 2
                                and MessageId = @MessageId
                                and isActive = 1";
                var count = await conn.QueryAsync<int>(sqlQuery, new { MessageId = query.MessageId });

                return count.FirstOrDefault() > 0;
            }
        }


        public async Task<IEnumerable<UpdatesDto>> GetUpdates(QueryBase query)
        {
            using (var conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<UpdatesDto>(Sql.Updates.GetUserUpdates, new { userid = query.UserId });
            }
        }

        #region UserPage
        public async Task<IEnumerable<Box.BoxToFriendDto>> GetUserWithFriendBoxes(GetUserWithFriendQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<Box.BoxToFriendDto>(Sql.Sql.UserWithFriendBoxes, new { Me = query.UserId, Myfriend = query.FriendId });
            }
        }
        public async Task<IEnumerable<Item.ItemToFriendDto>> GetUserWithFriendFiles(GetUserWithFriendQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<Item.ItemToFriendDto>(Sql.Sql.UserWithFriendFiles, new { Me = query.UserId, Myfriend = query.FriendId });
            }
        }

        public async Task<IEnumerable<Qna.QuestionToFriendDto>> GetUserWithFriendQuestion(GetUserWithFriendQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<Qna.QuestionToFriendDto>(Sql.Sql.UserWithFriendQuestion, new { Me = query.UserId, Myfriend = query.FriendId });
            }
        }

        public async Task<IEnumerable<Qna.AnswerToFriendDto>> GetUserWithFriendAnswer(GetUserWithFriendQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<Qna.AnswerToFriendDto>(Sql.Sql.UserWithFriendAnswer, new { Me = query.UserId, Myfriend = query.FriendId });
            }
        }

        public async Task<IEnumerable<User.UserInviteDto>> GetUserPersonalInvites(GetInvitesQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<User.UserInviteDto>(Sql.Sql.UserPersonalInvites, new { Me = query.UserId });
            }
        }
        public async Task<User.UserToFriendActivity> GetUserWithFriendActivity(GetUserWithFriendQuery query)
        {
            var retVal = new User.UserToFriendActivity();
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                using (var grid = conn.QueryMultiple(String.Format("{0} {1} {2}", Sql.Sql.UserWithFriendFiles, Sql.Sql.UserWithFriendQuestion, Sql.Sql.UserWithFriendAnswer), new { Me = query.UserId, Myfriend = query.FriendId }))
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
        public async Task<SeoDto> GetSeoBoxesAndItems()
        {
            SeoDto retVal = new SeoDto();
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                using (var grid = conn.QueryMultiple(String.Format("{0} {1}", Sql.Seo.GetBoxes, Sql.Seo.GetItems)))
                {
                    retVal.Boxes = grid.Read<Box.BoxSeoDto>();
                    retVal.Items = grid.Read<Item.ItemSeoDto>();
                }
            }
            return retVal;
        }
        #endregion

        #region Admin
        public async Task<IEnumerable<Zbang.Zbox.ViewModel.DTOs.UserDtos.AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query)
        {
            using (IDbConnection conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<Zbang.Zbox.ViewModel.DTOs.UserDtos.AdminUserDto>(Sql.Admin.UsersInUniversity,
                    new
                    {
                        universityId = query.UniversityId
                    });
            }
        }
        #endregion
    }
}
