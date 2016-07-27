using System;
using Dapper;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Emails;
using Zbang.Zbox.ViewModel.SqlQueries;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadServiceWorkerRole : IZboxReadServiceWorkerRole
    {
        public async Task<IEnumerable<UserDigestDto>> GetUsersByNotificationSettingsAsync(GetUserByNotificationQuery query, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token))
            {
                return await conn.QueryAsync<UserDigestDto>(Email.GetUserListByNotificationSettings,
                    new
                    {
                        Notification = query.NotificationSettings,
                        NotificationTime = query.MinutesPerNotificationSettings,
                        currentDate = DateTime.UtcNow,
                        query.PageNumber,
                        query.RowsPerPage
                    });
            }

        }

        public async Task<IEnumerable<UserUpdatesDigestDto>> GetUserUpdatesAsync(GetBoxesLastUpdateQuery query, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token))
            {
                return await conn.QueryAsync<UserUpdatesDigestDto>(Email.GetUserUpdates,
                    new
                    {
                        NotificationTime = query.MinutesPerNotificationSettings,
                        currentDate = DateTime.UtcNow,
                        query.UserId
                    });
            }

        }

        public async Task<BoxUpdatesDigestDto> GetUpdatesAsync(GetUpdatesQuery query, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token))
            {
                using (var grid = await conn.QueryMultipleAsync($"{Email.GetBoxUpdates} {Email.GetItemUpdates} {Email.GetQuizUpdates} {Email.GetCommentUpdates} {Email.GetRepliesUpdates} {Email.GetQuizDiscussionUpdates}",
                    new
                    {
                        query.BoxIds,
                        query.CommentsIds,
                        query.DiscussionIds,
                        query.ItemIds,
                        query.QuizIds,
                        query.RepliesIds
                    }))
                {
                    var retVal = new BoxUpdatesDigestDto
                    {
                        Boxes = await grid.ReadAsync<BoxDigestDto>(),
                        Items = await grid.ReadAsync<ItemDigestDto>(),
                        Quizzes = await grid.ReadAsync<QuizDigestDto>(),
                        Comments = await grid.ReadAsync<QnADigestDto>(),
                        Replies = await grid.ReadAsync<QnADigestDto>(),
                        QuizDiscussions = await grid.ReadAsync<QuizDiscussionDigestDto>()
                    };
                    return retVal;
                }
            }
        }

        public async Task<IEnumerable<BoxDigestDto>> GetBoxesLastUpdatesAsync(GetBoxesLastUpdateQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<BoxDigestDto>(Email.GetBoxPossibleUpdateByUser,
                    new
                    {
                        Notification = query.MinutesPerNotificationSettings,
                        query.UserId
                    });
            }

        }

        public async Task<BoxUpdatesDigestDto> GetBoxLastUpdatesAsync(GetBoxLastUpdateQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(
                    $"{Email.GetItemUpdateByBox} {Email.GetQuizUpdateByBox} {Email.GetQuizDiscussionUpdateByBox} {Email.GetQuestionUpdateByBox} {Email.GetAnswerUpdateByBox}",
                    new
                    {
                        Notification = query.MinutesPerNotificationSettings,
                        query.BoxId
                    }))
                {
                    var retVal = new BoxUpdatesDigestDto
                    {

                        Items = await grid.ReadAsync<ItemDigestDto>(),
                        Quizzes = await grid.ReadAsync<QuizDigestDto>(),
                        QuizDiscussions = await grid.ReadAsync<QuizDiscussionDigestDto>(),
                        Comments = await grid.ReadAsync<QnADigestDto>(),
                        Replies = await grid.ReadAsync<QnADigestDto>()
                    };
                    return retVal;
                }
            }
        }




        public BadItemDto GetFlagItemUserDetail(GetBadItemFlagQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBadItemUserDetail");
                dbQuery.SetInt64("UserId", query.UserId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<BadItemDto>());


                IQuery itemQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBadItemDetail");
                itemQuery.SetInt64("ItemId", query.ItemId);
                itemQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<BadItemDto>());

                var fdbQuery = dbQuery.FutureValue<BadItemDto>();
                var fitemQuery = itemQuery.FutureValue<BadItemDto>();

                var retVal = fdbQuery.Value;
                var itemVal = fitemQuery.Value;

                retVal.ItemName = itemVal.ItemName;
                return retVal;
            }
        }

        public async Task<IEnumerable<dynamic>> GetMissingThumbnailBlobsAsync(int index, long start)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                

                return await conn.QueryAsync(@"select itemid, blobname, Discriminator from zbox.item 
                where  Discriminator = 'Link'
                and isdeleted = 0
                and itemid >=  @start
                order by itemid
                OFFSET @Offset ROWS
                FETCH NEXT @RowSize ROWS ONLY", new { Offset = index * 100, RowSize = 100, start });
                
            }
        }

        public async Task<IEnumerable<string>> GetEmailsAsync(int page)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<string>(Sql.MarketingEmailQuery, new { pageNumber = page });
            }
        }

        public async Task<UniversityToUpdateSearchDto> GetUniversityDirtyUpdatesAsync(int index, int total, int top)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(Search.GetUniversityToUploadToSearch +
                                                                Search.GetUniversityPeopleToUploadToSearch +
                                                                Search.GetUniversitiesToDeleteFromSearch
                    , new { index, count = total, top }))
                {
                    var retVal = new UniversityToUpdateSearchDto
                    {
                        UniversitiesToUpdate = await grid.ReadAsync<UniversitySearchDto>()
                    };
                    var images = await grid.ReadAsync<UserImagesForUniversitySearchDto>();
                    var userImagesForUniversitySearchDtos = images as UserImagesForUniversitySearchDto[] ??
                                                            images.ToArray();
                    foreach (var university in retVal.UniversitiesToUpdate)
                    {
                        UniversitySearchDto university1 = university;
                        university.UsersImages =
                            userImagesForUniversitySearchDtos.Where(w => w.UniversityId == university1.Id)
                                .Select(s => s.Image);
                    }
                    retVal.UniversitiesToDelete = await grid.ReadAsync<long>();

                    return retVal;

                }

            }
        }

        public async Task<BoxToUpdateSearchDto> GetBoxDirtyUpdatesAsync(int index, int total, int top)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {

                using (var grid = await conn.QueryMultipleAsync
                    (Search.GetBoxToUploadToSearch +
                     Search.GetBoxUsersToUploadToSearch +
                     Search.GetBoxDepartmentToUploadToSearch +
                     Search.GetBoxToDeleteToSearch, new { index, count = total, top }))
                {
                    var retVal = new BoxToUpdateSearchDto
                    {
                        BoxesToUpdate = await grid.ReadAsync<BoxSearchDto>()
                    };
                    var usersInBoxes = grid.Read<UsersInBoxSearchDto>().ToList();
                    var departmentsOfBoxes = grid.Read<DepartmentOfBoxSearchDto>().ToList();

                    foreach (var box in retVal.BoxesToUpdate)
                    {
                        var boxid = box.Id;
                        box.UserIds = usersInBoxes.Where(w => w.BoxId == boxid).Select(s => s.UserId);
                    }
                    foreach (var box in retVal.BoxesToUpdate)
                    {
                        var boxid = box.Id;
                        box.Department = departmentsOfBoxes.Where(w => w.BoxId == boxid).Select(s => s.Name);
                    }
                    retVal.BoxesToDelete = await grid.ReadAsync<long>();
                    return retVal;
                }

            }
        }

        public async Task<ItemToUpdateSearchDto> GetItemDirtyUpdatesAsync(int index, int total, int top)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync
                    (Search.GetItemsToUploadToSearch +
                     Search.GetItemUsersToUploadToSearch +
                     Search.GetItemToDeleteToSearch, new { index, count = total, top }
                    ))
                {
                    var retVal = new ItemToUpdateSearchDto
                    {
                        ItemsToUpdate = await grid.ReadAsync<ItemSearchDto>()
                    };
                    var usersInItems = grid.Read<UsersInBoxSearchDto>().ToList();


                    foreach (var item in retVal.ItemsToUpdate)
                    {
                        var boxid = item.BoxId;
                        item.UserIds = usersInItems.Where(w => w.BoxId == boxid).Select(s => s.UserId);
                    }
                    retVal.ItemsToDelete = await grid.ReadAsync<long>();
                    return retVal;
                }
            }
        }

        public async Task<QuizToUpdateSearchDto> GetQuizzesDirtyUpdatesAsync(int index, int total, int top)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync
                    (Search.GetQuizzesToUploadToSearch +
                     Search.GetQuizzesQuestionToUploadToSearch +
                     Search.GetQuizzesAnswersToUploadToSearch +
                     Search.GetQuizzesUsersToUploadToSearch +
                     Search.GetQuizzesToDeleteFromSearch, new { index, count = total, top }
                    ))
                {
                    var retVal = new QuizToUpdateSearchDto
                    {
                        QuizzesToUpdate = await grid.ReadAsync<QuizSearchDto>()
                    };
                    var questions = grid.Read<QuizQuestionAndAnswersSearchDto>().ToList();
                    var answers = grid.Read<QuizQuestionAndAnswersSearchDto>().ToList();
                    var usersInQuizzes = grid.Read<UsersInBoxSearchDto>().ToList();

                    foreach (var quiz in retVal.QuizzesToUpdate)
                    {
                        long quizId = quiz.Id;
                        var boxId = quiz.BoxId;

                        quiz.Questions = questions.Where(w => w.QuizId == quizId).Select(s => s.Text);
                        quiz.Answers = answers.Where(w => w.QuizId == quizId).Select(s => s.Text);
                        quiz.UserIds = usersInQuizzes.Where(w => w.BoxId == boxId).Select(s => s.UserId);
                    }
                    retVal.QuizzesToDelete = await grid.ReadAsync<long>();
                    return retVal;
                }
            }
        }


        public Task<IEnumerable<MarketingDto>> GetUsersWithoutUniversityAsync(MarketingQuery query,
            CancellationToken token)
        {
            const string sql = @"select email,Culture,UserName as Name from zbox.Users
where UniversityId is null
and EmailSendSettings = 0
and (creationtime>'2015' or [LastAccessTime] >'2015')
and (membershipUserid is not null or facebookUserid is not null or googleUserid is not null)
and creationtime < dateadd(HOUR,-2,GETUTCDATE())
order by userid
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY";
            return GetMarketingDataAsync(query, sql, token);

        }

        public Task<IEnumerable<MarketingDto>> GetLowContributersUsersAsync(MarketingQuery query,
           CancellationToken token)
        {
            const string sql = @"select email,Culture,UserName as Name
from zbox.users u 
where u.UserReputation < 600
and UniversityId is not null
and exists (select userid from zbox.userboxrel ub where ub.userid = u.userid)
and u.UniversityId in (	select Id from zbox.University u where u.NoOfBoxes >= 5 and isdeleted = 0)
and EmailSendSettings = 0
and (creationtime>'2015' or [LastAccessTime] >'2015')
and (membershipuserid is not null or facebookuserid is not null or googleuserid is not null)
and creationtime < dateadd(HOUR,-2,GETUTCDATE())
order by userid
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY";
            return GetMarketingDataAsync(query, sql, token);
        }

        public Task<IEnumerable<MarketingDto>> GetUsersWithUniversityWithoutSubscribedBoxesAsync(MarketingQuery query,
            CancellationToken token)
        {
            const string sql = @"select email,Culture,UserName as Name 
from zbox.users u join zbox.University uu on u.UniversityId = uu.Id
and uu.Country != 'us'
and not exists (select userid from zbox.userboxrel ub where ub.userid = u.userid)
and EmailSendSettings = 0
and (membershipuserid is not null or facebookuserid is not null or googleuserid is not null)
and (u.creationtime>'2015' or [LastAccessTime] >'2015') 
and u.creationtime < dateadd(HOUR,-2,GETUTCDATE())
order by userid
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY";
            return GetMarketingDataAsync(query, sql, token);
        }

        public Task<IEnumerable<MarketingDto>> GetUsersWithLowActivityUniversitiesAsync(MarketingQuery query,
            CancellationToken token)
        {
            const string sql = @"select email,Culture,UserName as Name 
from zbox.Users u2 where u2.UniversityId in (
select Id from zbox.University u where u.NoOfBoxes < 5 and isdeleted = 0 and u.Country != 'us')
and EmailSendSettings = 0
and (creationtime>'2015' or [LastAccessTime] >'2015')
and exists (select userid from zbox.userboxrel ub where ub.userid = u2.userid)
and (membershipuserid is not null or facebookuserid is not null or googleuserid is not null)
and creationtime < dateadd(HOUR,-2,GETUTCDATE())
order by userid
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY";
            return GetMarketingDataAsync(query, sql, token);
        }

        public Task<IEnumerable<MarketingDto>> GetUsersFollowingCoursesNoActivityAsync(MarketingQuery query,
            CancellationToken token)
        {
            const string sql = @"with boxWithLowUpdate as (
select b.boxid from zbox.Box b
join zbox.University u on b.University = u.Id and u.NoOfBoxes < 5 and u.Country != 'us'
where b.UpdateTime < dateadd(DAY,-3,GETUTCDATE())
and b.IsDeleted = 0
and Discriminator in (2,3))

select email,Culture,UserName as Name 
from zbox.Users where userid in (
select distinct ub.userid from zbox.UserBoxRel ub
where ub.boxid in (select boxid from boxWithLowUpdate)
except
select distinct ub.userid from zbox.UserBoxRel ub join zbox.UserBoxRel ub2 on ub.UserId = ub2.UserId 
where ub.boxid in (select boxid from boxWithLowUpdate)
and ub2.boxid not in (select boxid from boxWithLowUpdate))
and EmailSendSettings = 0
and (creationtime>'2015' or [LastAccessTime] >'2015')
and (membershipuserid is not null or facebookuserid is not null or googleuserid is not null)
and creationtime < dateadd(HOUR,-2,GETUTCDATE())
order by UserId
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY";
            return GetMarketingDataAsync(query, sql, token);
        }

        private async Task<IEnumerable<MarketingDto>> GetMarketingDataAsync(MarketingQuery query, string sql,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token))
            {
                return await conn.QueryAsync<MarketingDto>(sql, new { query.RowsPerPage, query.PageNumber });
            }
        }

        public async Task<IEnumerable<LikesDto>> GetLikesDataAsync(CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token))
            {
                using (var grid = await conn.QueryMultipleAsync($"{Email.GetLikesOnItem} {Email.GetLikesOnReplies} {Email.GetLikesOnComments}", new { timeDiff = -1 }))
                {
                    IEnumerable<LikesDto> items = await grid.ReadAsync<ItemLikesDto>();
                    IEnumerable<LikesDto> replies = await grid.ReadAsync<ReplyLikesDto>();
                    IEnumerable<LikesDto> comments = await grid.ReadAsync<CommentLikesDto>();

                    return items.Union(replies).Union(comments);
                }
            }
        }

    }
}
