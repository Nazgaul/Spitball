using Dapper;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Emails;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.SqlQueries;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadServiceWorkerRole : IZboxReadServiceWorkerRole
    {
        private static RetryPolicy GetRetryPolicy()
        {
            var retryPolicy = RetryManager.Instance.GetDefaultSqlCommandRetryPolicy();
            retryPolicy.Retrying += (sender, args) =>
            {
                // Log details of the retry.
                var msg = $"Retry - Count:{args.CurrentRetryCount}, Delay:{args.Delay}, Exception:{args.LastException}";
                TraceLog.WriteWarning(msg);
            };
            return retryPolicy;
        }

        public async Task<IEnumerable<UserDigestDto>> GetUsersByNotificationSettingsAsync(
            GetUserByNotificationQuery query, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token))
            {
                var policy = GetRetryPolicy();
                return
                    await
                        policy.ExecuteAsync(
                             // ReSharper disable once AccessToDisposedClosure
                             () =>
                                    conn.QueryAsync<UserDigestDto>(
                                        new CommandDefinition(Email.GetUserListByNotificationSettings,
                                            new
                                            {
                                                Notification = query.NotificationSettings,
                                                NotificationTime = query.MinutesPerNotificationSettings,
                                                query.PageNumber,
                                                query.RowsPerPage,
                                                query.UtcOffset
                                            }, cancellationToken: token)), token);
            }

        }



        public async Task<IEnumerable<UserUpdatesDigestDto>> GetUserUpdatesAsync(GetBoxesLastUpdateQuery query,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token))
            {
                var policy = GetRetryPolicy();
                return
                    await
                        policy.ExecuteAsync(
                             // ReSharper disable once AccessToDisposedClosure
                             () =>
                                 conn.QueryAsync<UserUpdatesDigestDto>(new CommandDefinition(Email.GetUserUpdates,
                                    new
                                    {
                                        NotificationTime = query.MinutesPerNotificationSettings,
                                        query.UserId
                                    }, cancellationToken: token)), token);
            }

        }

        public async Task<BoxUpdatesDigestDto> GetUpdatesAsync(GetUpdatesQuery query, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token))
            {
                var policy = GetRetryPolicy();
                return await policy.ExecuteAsync(async () =>
                {
                    using (
                        var grid =
                            await
                                // ReSharper disable once AccessToDisposedClosure
                                conn.QueryMultipleAsync(
                                    $"{Email.GetBoxUpdates} {Email.GetItemUpdates} {Email.GetQuizUpdates} {Email.GetCommentUpdates} {Email.GetRepliesUpdates} {Email.GetQuizDiscussionUpdates}",
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
                }, token);
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

        public async Task<UniversityToUpdateSearchDto> GetUniversitiesDirtyUpdatesAsync(int index, int total, int top)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(Search.GetUniversitiesToUploadToSearch +
                                                                Search.GetUniversitiesPeopleToUploadToSearch +
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

        public async Task<UniversitySearchDto> GetUniversityDirtyUpdatesAsync(long id)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(Search.GetUniversityToUploadToSearch +
                                                                Search.GetUniversityPeopleToUploadToSearch
                    , new { id }))
                {
                    var retVal = await grid.ReadSingleAsync<UniversitySearchDto>();
                    retVal.UsersImages = (await grid.ReadAsync<UserImagesForUniversitySearchDto>()).Select(s => s.Image);
                    return retVal;

                }
            }
        }

        public async Task<BoxSearchDto> GetBoxDirtyUpdatesAsync(long id,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token))
            {
                var retry = GetRetryPolicy();
                return await retry.ExecuteAsync(async () =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    using (var grid = await conn.QueryMultipleAsync
                        (Search.GetBoxToUploadToSearch +
                         Search.GetBoxUsersToUploadToSearch +
                         Search.GetBoxDepartmentToUploadToSearch +
                         Search.GetBoxFeedToUploadToSearch
                         , new { boxid = id }))
                    {
                        var retVal = await grid.ReadSingleAsync<BoxSearchDto>();
                        retVal.UserIds = await grid.ReadAsync<long>();
                        retVal.Department = await grid.ReadAsync<string>();
                        retVal.Feed = await grid.ReadAsync<string>();

                        return retVal;
                    }
                }, token);

            }
        }

        public async Task<BoxToUpdateSearchDto> GetBoxesDirtyUpdatesAsync(int index, int total, int top,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token))
            {
                var retry = GetRetryPolicy();
                return await retry.ExecuteAsync(async () =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    using (var grid = await conn.QueryMultipleAsync
                        (Search.GetBoxesToUploadToSearch +
                         Search.GetBoxesUsersToUploadToSearch +
                         Search.GetBoxesDepartmentToUploadToSearch +
                         Search.GetBoxesFeedToUploadToSearch +
                         Search.GetBoxesToDeleteToSearch, new { index, count = total, top }))
                    {
                        var retVal = new BoxToUpdateSearchDto
                        {
                            BoxesToUpdate = await grid.ReadAsync<BoxSearchDto>()
                        };
                        var usersInBoxes = grid.Read<UsersInBoxSearchDto>().ToList();
                        var departmentsOfBoxes = grid.Read<DepartmentOfBoxSearchDto>().ToList();
                        var feedOfBoxes = grid.Read<FeedOfBoxSearchDto>().ToList();
                        foreach (var box in retVal.BoxesToUpdate)
                        {
                            var boxid = box.Id;
                            box.UserIds = usersInBoxes.Where(w => w.BoxId == boxid).Select(s => s.UserId);
                            box.Department = departmentsOfBoxes.Where(w => w.BoxId == boxid).Select(s => s.Name);
                            box.Feed = feedOfBoxes.Where(w => w.BoxId == boxid).Select(s => s.Text);
                        }
                        retVal.BoxesToDelete = await grid.ReadAsync<long>();
                        return retVal;
                    }
                }, token);

            }
        }

        public async Task<ItemToUpdateSearchDto> GetItemsDirtyUpdatesAsync(SearchItemDirtyQuery query)
        {
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(DocumentSearchDto), new List<string> { "Id" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(ItemSearchUsers), new List<string> { "Id" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(ItemSearchTag), new List<string> { "Name" });
            
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync
                    (Search.GetItemsToUploadToSearch2 +
                     Search.GetItemToDeleteToSearch, new { query.Index, count = query.Total, query.Top, query.ItemId }
                    ))
                {
                    var dynamic = await grid.ReadAsync();
                    var retVal = new ItemToUpdateSearchDto
                    {
                        ItemsToUpdate = Slapper.AutoMapper.MapDynamic<DocumentSearchDto>(dynamic),
                        ItemsToDelete = await grid.ReadAsync<DocumentToDeleteSearchDto>()
                    };
                    return retVal;
                }
            }
        }

        //public async Task<DocumentSearchDto> GetItemDirtyUpdatesAsync(long itemId)
        //{
        //    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(ItemSearchDto), new List<string> { "Id" });
        //    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(ItemSearchUsers), new List<string> { "Id" });
        //    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(ItemSearchTag), new List<string> { "Name" });

        //    using (var conn = await DapperConnection.OpenConnectionAsync())
        //    {
        //        var dynamic = conn.QueryAsync(Search.GetItemToUploadToSearch);
        //        return Slapper.AutoMapper.MapDynamic<DocumentSearchDto>(dynamic);
        //    }
        //}

        public async Task<FlashcardToUpdateSearchDto> GetFlashcardsDirtyUpdatesAsync(int index, int total, int top, CancellationToken token)
        {
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(FlashcardSearchDto), new List<string> { "Id" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(ItemSearchUsers), new List<string> { "Id" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(ItemSearchTag), new List<string> { "Name" });

            using (var conn = await DapperConnection.OpenConnectionAsync(token))
            {
                using (var grid = await conn.QueryMultipleAsync(
                    Search.GetFlashcardToDeleteFromSearch +
                    Search.GetFlashcardToUploadToSearch
                    , new { index, count = total, top }))
                {
                    var retVal = new FlashcardToUpdateSearchDto
                    {
                        Deletes = await grid.ReadAsync<FlashcardToDeleteSearchDto>(),
                    };
                    var dynamic = await grid.ReadAsync();
                    retVal.Updates = Slapper.AutoMapper.MapDynamic<FlashcardSearchDto>(dynamic);
                    return retVal;
                }
            }
        }

        public async Task<QuizToUpdateSearchDto> GetQuizzesDirtyUpdatesAsync(int index, int total, int top)
        {
            //find join all to use Slapper is performance hit
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync
                    (Search.GetQuizzesToUploadToSearch +
                     Search.GetQuizzesQuestionToUploadToSearch +
                     Search.GetQuizzesAnswersToUploadToSearch +
                     Search.GetQuizzesUsersToUploadToSearch +
                     Search.GetQuizzesToDeleteFromSearch,
                     new { index, count = total, top }
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
                        var quizId = quiz.Id;
                        var boxId = quiz.BoxId;

                        quiz.Questions = questions.Where(w => w.QuizId == quizId).Select(s => s.Text);
                        quiz.Answers = answers.Where(w => w.QuizId == quizId).Select(s => s.Text);
                        quiz.UserIds = usersInQuizzes.Where(w => w.BoxId == boxId).Select(s => s.UserId);
                    }
                    retVal.QuizzesToDelete = await grid.ReadAsync<QuizToDeleteSearchDto>();
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
where u.score < 600
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
                using (
                    var grid =
                        await
                            conn.QueryMultipleAsync(
                                $"{Email.GetLikesOnItem} {Email.GetLikesOnReplies} {Email.GetLikesOnComments}",
                                new { timeDiff = -1 }))
                {
                    IEnumerable<LikesDto> items = await grid.ReadAsync<ItemLikesDto>();
                    IEnumerable<LikesDto> replies = await grid.ReadAsync<ReplyLikesDto>();
                    IEnumerable<LikesDto> comments = await grid.ReadAsync<CommentLikesDto>();

                    return items.Union(replies).Union(comments);
                }
            }
        }

        public async Task<IEnumerable<SpamGunDto>> GetSpamGunDataAsync(int universityId, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token, "SpamGun"))
            {
                const string sql1 = @"select top 500 s.id, FirstName, Email,mailbody as MailBody,s.chapter,
mailsubject as MailSubject, mailcategory as MailCategory,u.url as UniversityUrl, u.name as school 
from students s join universities u on s.uniid = u.id
where uniid = @UniId
and shouldSend = 1
and chapter is not null
order by s.id
OPTION (TABLE HINT(s, INDEX ([students_shouldsend2])),Recompile);";
                const string sql2 = @"select top 500 s.id, FirstName, LastName, Email,mailbody as MailBody,
mailsubject as MailSubject, mailcategory as MailCategory,u.url as UniversityUrl 
from students s join universities u on s.uniid = u.id
where uniid = @UniId
and shouldSend = 1
and chapter is null
order by s.id
OPTION (TABLE HINT(s, INDEX ([students_shouldsend2])),Recompile);";
                var policy = GetRetryPolicy();
                return await policy.ExecuteAsync(async () =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    using (var grid = await conn.QueryMultipleAsync(
                        new CommandDefinition(
                            sql1 + sql2, new { UniId = universityId }, cancellationToken: token)))
                    {
                        var result = await grid.ReadAsync<GreekPartnerDto>();
                        return result.Union(await grid.ReadAsync<SpamGunDto>()); ;
                    }
                }, token);
            }


        }

        public IEnumerable<long> GetUsersBadgeFollow(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select distinct u.userid from zbox.Users u join zbox.UserBoxRel ub on u.UserId = ub.UserId
except select userid from zbox.badge where name = 2 and progress = 100
 order by u.userid
 offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }

        public IEnumerable<long> GetUsersBadgeQuiz(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select distinct u.userid from zbox.Users u join zbox.Quiz q on u.UserId = q.UserId and q.IsDeleted = 0 and q.Publish = 1
except select userid from zbox.badge where name = 3 and progress = 100
 order by u.userid
 offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }
        public IEnumerable<long> GetUsersBadgeItem(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select distinct u.userid from zbox.Users u join zbox.Item q on u.UserId = q.UserId and q.IsDeleted = 0 
except select userid from zbox.badge where name = 4 and progress = 100
 order by u.userid
 offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }
        public IEnumerable<long> GetUsersBadgeLike(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select * from (select OwnerId from zbox.CommentLike
union
select OwnerId from zbox.ReplyLike
union
select OwnerId from zbox.ItemRate
union
select UserId from zbox.FlashcardLike
union 
select userid from zbox.quizlike2
except select userid from zbox.badge where name = 5 and progress = 100) t
order by OwnerId
offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }

        public IEnumerable<long> GetUserReputationUpdate(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select * from (
select userid from zbox.badge
union 
select userid from zbox.item where isdeleted = 0 and (NumberOfDownloads > 0 or NumberOfViews > 0 or LikeCount > 0)
union
select userid from zbox.Quiz where IsDeleted = 0 and Publish = 1 and (NumberOfViews > 0 or SolveCount > 0 or LikeCount > 0)
union
select userid from zbox.Flashcard where IsDeleted = 0 and Publish = 1 and (NumberOfViews > 0  or LikeCount > 0)
union
select userid from zbox.Question where LikeCount > 0
union
select userid from zbox.Answer where LikeCount > 0) t
order by userid
offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }
    }
}
