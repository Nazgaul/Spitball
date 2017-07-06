using System;
using Dapper;
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
using Zbang.Zbox.ViewModel.Dto.Qna;
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
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token).ConfigureAwait(false))
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
                                        }, cancellationToken: token)), token).ConfigureAwait(false);
            }

        }



        public async Task<IEnumerable<UserUpdatesDigestDto>> GetUserUpdatesAsync(GetBoxesLastUpdateQuery query,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token).ConfigureAwait(false))
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
                                    }, cancellationToken: token)), token).ConfigureAwait(false);
            }

        }

        public async Task<BoxUpdatesDigestDto> GetUpdatesAsync(GetUpdatesQuery query, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token).ConfigureAwait(false))
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
                                    }).ConfigureAwait(false))
                    {
                        var retVal = new BoxUpdatesDigestDto
                        {
                            Boxes = await grid.ReadAsync<BoxDigestDto>().ConfigureAwait(false),
                            Items = await grid.ReadAsync<ItemDigestDto>().ConfigureAwait(false),
                            Quizzes = await grid.ReadAsync<QuizDigestDto>().ConfigureAwait(false),
                            Comments = await grid.ReadAsync<QnADigestDto>().ConfigureAwait(false),
                            Replies = await grid.ReadAsync<QnADigestDto>().ConfigureAwait(false),
                            QuizDiscussions = await grid.ReadAsync<QuizDiscussionDigestDto>().ConfigureAwait(false)
                        };
                        return retVal;
                    }
                }, token).ConfigureAwait(false);
            }


        }

        public async Task<IEnumerable<BoxDigestDto>> GetBoxesLastUpdatesAsync(GetBoxesLastUpdateQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await conn.QueryAsync<BoxDigestDto>(Email.GetBoxPossibleUpdateByUser,
                    new
                    {
                        Notification = query.MinutesPerNotificationSettings,
                        query.UserId
                    }).ConfigureAwait(false);
            }

        }

        public async Task<BoxUpdatesDigestDto> GetBoxLastUpdatesAsync(GetBoxLastUpdateQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(
                    $"{Email.GetItemUpdateByBox} {Email.GetQuizUpdateByBox} {Email.GetQuizDiscussionUpdateByBox} {Email.GetQuestionUpdateByBox} {Email.GetAnswerUpdateByBox}",
                    new
                    {
                        Notification = query.MinutesPerNotificationSettings,
                        query.BoxId
                    }).ConfigureAwait(false))
                {
                    var retVal = new BoxUpdatesDigestDto
                    {

                        Items = await grid.ReadAsync<ItemDigestDto>().ConfigureAwait(false),
                        Quizzes = await grid.ReadAsync<QuizDigestDto>().ConfigureAwait(false),
                        QuizDiscussions = await grid.ReadAsync<QuizDiscussionDigestDto>().ConfigureAwait(false),
                        Comments = await grid.ReadAsync<QnADigestDto>().ConfigureAwait(false),
                        Replies = await grid.ReadAsync<QnADigestDto>().ConfigureAwait(false)
                    };
                    return retVal;
                }
            }
        }




        public BadItemDto GetFlagItemUserDetail(GetBadItemFlagQuery query)
        {
            using (UnitOfWork.Start())
            {
                var dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBadItemUserDetail");
                dbQuery.SetInt64("UserId", query.UserId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<BadItemDto>());


                var itemQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBadItemDetail");
                itemQuery.SetInt64("ItemId", query.ItemId);
                itemQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<BadItemDto>());

                var fdbQuery = dbQuery.FutureValue<BadItemDto>();
                var fItemQuery = itemQuery.FutureValue<BadItemDto>();

                var retVal = fdbQuery.Value;
                var itemVal = fItemQuery.Value;

                retVal.ItemName = itemVal.ItemName;
                return retVal;
            }
        }

        public async Task<UniversityToUpdateSearchDto> GetUniversitiesDirtyUpdatesAsync(int index, int total, int top)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(Search.GetUniversitiesToUploadToSearch +
                                                                Search.GetUniversitiesPeopleToUploadToSearch +
                                                                Search.GetUniversitiesToDeleteFromSearch
                    , new { index, count = total, top }).ConfigureAwait(false))
                {
                    var retVal = new UniversityToUpdateSearchDto
                    {
                        UniversitiesToUpdate = await grid.ReadAsync<UniversitySearchDto>().ConfigureAwait(false)
                    };
                    var images = await grid.ReadAsync<UserImagesForUniversitySearchDto>().ConfigureAwait(false);
                    var userImagesForUniversitySearchDtos = images as UserImagesForUniversitySearchDto[] ??
                                                            images.ToArray();
                    foreach (var university in retVal.UniversitiesToUpdate)
                    {
                        university.UsersImages =
                            userImagesForUniversitySearchDtos.Where(w => w.UniversityId == university.Id)
                                .Select(s => s.Image);
                    }
                    retVal.UniversitiesToDelete = await grid.ReadAsync<long>().ConfigureAwait(false);

                    return retVal;

                }
            }
        }

        public async Task<UniversitySearchDto> GetUniversityDirtyUpdatesAsync(long id)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(Search.GetUniversityToUploadToSearch +
                                                                Search.GetUniversityPeopleToUploadToSearch
                    , new { id }).ConfigureAwait(false))
                {
                    var retVal = await grid.ReadSingleAsync<UniversitySearchDto>().ConfigureAwait(false);
                    retVal.UsersImages = (await grid.ReadAsync<UserImagesForUniversitySearchDto>().ConfigureAwait(false)).Select(s => s.Image);
                    return retVal;

                }
            }
        }

        public async Task<BoxSearchDto> GetBoxDirtyUpdatesAsync(long id,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token).ConfigureAwait(false))
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
                        , new { boxId = id }).ConfigureAwait(false))
                    {
                        var retVal = await grid.ReadSingleAsync<BoxSearchDto>().ConfigureAwait(false);
                        retVal.UserIds = await grid.ReadAsync<long>().ConfigureAwait(false);
                        retVal.Department = await grid.ReadAsync<string>().ConfigureAwait(false);
                        retVal.Feed = await grid.ReadAsync<string>().ConfigureAwait(false);

                        return retVal;
                    }
                }, token).ConfigureAwait(false);

            }
        }

        public async Task<BoxToUpdateSearchDto> GetBoxesDirtyUpdatesAsync(int index, int total, int top,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token).ConfigureAwait(false))
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
                     Search.GetBoxesToDeleteToSearch, new { index, count = total, top }).ConfigureAwait(false))
                    {
                        var retVal = new BoxToUpdateSearchDto
                        {
                            BoxesToUpdate = await grid.ReadAsync<BoxSearchDto>().ConfigureAwait(false)
                        };
                        var usersInBoxes = (await grid.ReadAsync<UsersInBoxSearchDto>().ConfigureAwait(false)).ToList();
                        var departmentsOfBoxes = (await grid.ReadAsync<DepartmentOfBoxSearchDto>().ConfigureAwait(false)).ToList();
                        var feedOfBoxes = (await grid.ReadAsync<FeedOfBoxSearchDto>().ConfigureAwait(false)).ToList();
                        foreach (var box in retVal.BoxesToUpdate)
                        {
                            var boxId = box.Id;
                            box.UserIds = usersInBoxes.Where(w => w.BoxId == boxId).Select(s => s.UserId);
                            box.Department = departmentsOfBoxes.Where(w => w.BoxId == boxId).Select(s => s.Name);
                            box.Feed = feedOfBoxes.Where(w => w.BoxId == boxId).Select(s => s.Text);
                        }
                        retVal.BoxesToDelete = await grid.ReadAsync<long>().ConfigureAwait(false);
                        return retVal;
                    }
                }, token).ConfigureAwait(false);

            }
        }

        public async Task<ItemToUpdateSearchDto> GetItemsDirtyUpdatesAsync(SearchItemDirtyQuery query, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(
                    new CommandDefinition(Search.SearchItemNew + Search.SearchItemUserBoxRel +
                                          Search.SearchItemTags + Search.GetItemToDeleteToSearch,
                        new { query.Index, count = query.Total, query.Top, query.ItemId }, cancellationToken: token)).ConfigureAwait(false))
                {
                    var retVal = new ItemToUpdateSearchDto
                    {
                        ItemsToUpdate = grid.Read< DocumentSearchDto,ItemCourseSearchDto,
                        ItemUniversitySearchDto,
                        DocumentSearchDto>((doc,course, university) =>
                        {
                            doc.Course = course;
                            doc.University = university;
                            return doc;
                        })
                    };
                    var users = (await grid.ReadAsync<ItemSearchUsers>().ConfigureAwait(false)).ToList();
                    var tags = (await grid.ReadAsync<ItemSearchTag>().ConfigureAwait(false)).ToList();
                    retVal.ItemsToDelete = await grid.ReadAsync<DocumentToDeleteSearchDto>().ConfigureAwait(false);
                    var cacheUsers = new Dictionary<long, IEnumerable<long>>();
                    foreach (var p in retVal.ItemsToUpdate)
                    {
                        if (cacheUsers.TryGetValue(p.Course.Id, out IEnumerable<long> usersIds))
                        {
                            p.UserIds = usersIds;
                        }
                        else
                        {
                            p.UserIds = cacheUsers[p.Course.Id] = users.Where(w => w.BoxId == p.Course.Id).Select(s => s.UserId);
                        }

                        p.Tags = tags.Where(w => w.Id == p.Id).ToList();
                    }
                    return retVal;
                }
            }


        }

        public async Task<FeedToUpdateSearchDto> GetFeedDirtyUpdatesAsync(long? version, int page, int size,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
            {

                using (var grid = await conn.QueryMultipleAsync(
                    $"{Search.GetFeedToDeleteFromSearch} {Search.GetFeedToSearch}"
                    , new { version, PageSize = size, PageNumber = page }).ConfigureAwait(false))
                {
                    var retVal = new FeedToUpdateSearchDto
                    {
                        Deletes = await grid.ReadAsync<FeedSearchDeleteDto>().ConfigureAwait(false),
                        Updates = grid.Read<FeedSearchDto, ItemCourseSearchDto,
                            ItemUniversitySearchDto,
                            FeedSearchDto>((doc, course, university) =>
                        {
                            doc.Course = course;
                            doc.University = university;

                            return doc;
                        })
                        //Updates = await grid.ReadAsync<FeedSearchDto>().ConfigureAwait(false)
                    };
                    using (var subGrid = await conn.QueryMultipleAsync($"{Search.GetFeedAnswers}{Search.GetFeedTags}", new { questionids = retVal.Updates.Select(s => s.Id) }).ConfigureAwait(false))
                    {
                        var answers = (await subGrid.ReadAsync<RepliesSearchDto>().ConfigureAwait(false)).ToList();
                        var tags = (await subGrid.ReadAsync<FeedSearchTag>().ConfigureAwait(false)).ToList();
                        if (answers.Count <= 0 && tags.Count <= 0) return retVal;
                        foreach (var update in retVal.Updates)
                        {
                            update.Replies = answers.Where(w => w.QuestionId == update.Id).Select(s => s.Text);
                            update.Tags = tags.Where(w => w.Id == update.Id).ToList();
                        }
                    }

                    return retVal;
                }

            }
        }


        public async Task<long> GetTrackingCurrentVersionAsync()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
               return await conn.QueryFirstAsync<long>(Search.NextVersionChanges).ConfigureAwait(false);
                //NextVersionChanges
            }
        }


        public async Task<FlashcardToUpdateSearchDto> GetFlashcardsDirtyUpdatesAsync(int index, int total, int top, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync(
                    Search.GetFlashcardToDeleteFromSearch +
                    Search.GetFlashcardToUploadToSearch +
                    Search.GetFlashcardUsersToUploadToSearch +
                    Search.GetFlashcardTagsToUploadToSearch
                    , new { index, count = total, top }).ConfigureAwait(false))
                {
                    var retVal = new FlashcardToUpdateSearchDto
                    {
                        Deletes = await grid.ReadAsync<FlashcardToDeleteSearchDto>().ConfigureAwait(false),
                        Updates = grid.Read<FlashcardSearchDto, ItemCourseSearchDto,
                        ItemUniversitySearchDto,
                            FlashcardSearchDto>((doc,course, university) =>
                        {
                            doc.Course = course;
                            doc.University = university;
                            return doc;
                        })
                    };
                    var users = (await grid.ReadAsync<UsersInBoxSearchDto>().ConfigureAwait(false)).ToList();
                    var tags = (await grid.ReadAsync<ItemSearchTag>().ConfigureAwait(false)).ToList();
                    var cacheUsers = new Dictionary<long, IEnumerable<long>>();
                    foreach (var flashcards in retVal.Updates)
                    {
                        if (cacheUsers.TryGetValue(flashcards.Course.Id, out IEnumerable<long> usersIds))
                        {
                            flashcards.UserIds = usersIds;
                        }
                        else
                        {
                            flashcards.UserIds = cacheUsers[flashcards.Course.Id] = users.Where(w => w.BoxId == flashcards.Course.Id).Select(s => s.UserId);
                        }

                        flashcards.Tags = tags.Where(w => w.Id == flashcards.Id).ToList();
                    }
                    return retVal;
                }
            }

        }

        public async Task<QuizToUpdateSearchDto> GetQuizzesDirtyUpdatesAsync(int index, int total, int top)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                using (var grid = await conn.QueryMultipleAsync
                (Search.GetQuizzesToUploadToSearch +
                 Search.GetQuizzesQuestionToUploadToSearch +
                 Search.GetQuizzesAnswersToUploadToSearch +
                 Search.GetQuizzesUsersToUploadToSearch +
                 Search.GetQuizzesTags +
                 Search.GetQuizzesToDeleteFromSearch,
                    new { index, count = total, top }
                ).ConfigureAwait(false))
                {
                    var retVal = new QuizToUpdateSearchDto
                    {
                        //QuizzesToUpdate = await grid.ReadAsync<QuizSearchDto>().ConfigureAwait(false)

                        QuizzesToUpdate = grid.Read<QuizSearchDto, ItemCourseSearchDto,
                        ItemUniversitySearchDto,
                        QuizSearchDto>((doc, course, university) =>
                        {
                            doc.Course = course;
                            doc.University = university;

                            return doc;
                        })
                    };
                    var questions = (await grid.ReadAsync<QuizQuestionAndAnswersSearchDto>().ConfigureAwait(false)).ToList();
                    var answers = (await grid.ReadAsync<QuizQuestionAndAnswersSearchDto>().ConfigureAwait(false)).ToList();
                    var users = (await grid.ReadAsync<UsersInBoxSearchDto>().ConfigureAwait(false)).ToList();
                    var tags = (await grid.ReadAsync<ItemSearchTag>().ConfigureAwait(false)).ToList();
                    var cacheUsers = new Dictionary<long, IEnumerable<long>>();
                    foreach (var quiz in retVal.QuizzesToUpdate)
                    {
                        var quizId = quiz.Id;
                        if (cacheUsers.TryGetValue(quiz.Course.Id, out IEnumerable<long> usersIds))
                        {
                            quiz.UserIds = usersIds;
                        }
                        else
                        {
                            quiz.UserIds = cacheUsers[quiz.Course.Id] = users.Where(w => w.BoxId == quiz.Course.Id).Select(s => s.UserId);
                        }

                        quiz.Questions = questions.Where(w => w.QuizId == quizId).Select(s => s.Text);
                        quiz.Answers = answers.Where(w => w.QuizId == quizId).Select(s => s.Text);
                        quiz.Tags = tags.Where(w => w.Id == quiz.Id).ToList();
                    }
                    retVal.QuizzesToDelete = await grid.ReadAsync<QuizToDeleteSearchDto>().ConfigureAwait(false);
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
and (creationTime> getdate()-180 or [LastAccessTime] >getdate()-180)
and (membershipUserId is not null or facebookUserId is not null or googleUserId is not null)
and creationTime < dateadd(HOUR,-2,GETUTCDATE())
order by userId
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
and not exists (select userId from zbox.userboxrel ub where ub.userId = u.userId)
and EmailSendSettings = 0
and (membershipUserId is not null or facebookUserId is not null or googleUserId is not null)
and (u.creationTime>getdate()-180 or [LastAccessTime] >getdate()-180) 
and u.creationTime < dateadd(HOUR,-2,GETUTCDATE())
order by userId
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
and (creationTime>getdate()-180 or [LastAccessTime]>getdate()-180)
and exists (select userId from zbox.userboxrel ub where ub.userId = u2.userId)
and (membershipUserId is not null or facebookUserId is not null or googleUserId is not null)
and creationTime < dateadd(HOUR,-2,GETUTCDATE())
order by userId
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY
";
            return GetMarketingDataAsync(query, sql, token);
        }

        public Task<IEnumerable<MarketingDto>> GetUsersFollowingCoursesNoActivityAsync(MarketingQuery query,
            CancellationToken token)
        {
            const string sql = @"with boxWithLowUpdate as (
select b.boxId from zbox.Box b
join zbox.University u on b.University = u.Id and u.NoOfBoxes < 5 and u.Country != 'us'
where b.UpdateTime < dateadd(DAY,-3,GETUTCDATE())
and b.IsDeleted = 0
and Discriminator in (2,3))

select email,Culture,UserName as Name 
from zbox.Users where userId in (
select distinct ub.userId from zbox.UserBoxRel ub
where ub.boxId in (select boxId from boxWithLowUpdate)
except
select distinct ub.userId from zbox.UserBoxRel ub join zbox.UserBoxRel ub2 on ub.userId = ub2.userId 
where ub.boxId in (select boxId from boxWithLowUpdate)
and ub2.boxId not in (select boxId from boxWithLowUpdate))
and EmailSendSettings = 0
and (creationTime>getdate()-180 or [LastAccessTime] >getdate()-180)
and (membershipUserId is not null or facebookUserId is not null or googleUserId is not null)
and creationTime < dateadd(HOUR,-2,GETUTCDATE())
order by userId
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY";
            return GetMarketingDataAsync(query, sql, token);
        }

        private static async Task<IEnumerable<MarketingDto>> GetMarketingDataAsync(MarketingQuery query, string sql,
            CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
            {
                return await conn.QueryAsync<MarketingDto>(sql, new { query.RowsPerPage, query.PageNumber }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<LikesDto>> GetLikesDataAsync(CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
            {
                using (
                    var grid =
                        await
                            conn.QueryMultipleAsync(
                                $"{Email.GetLikesOnItem} {Email.GetLikesOnReplies} {Email.GetLikesOnComments}",
                                new { timeDiff = -1 }).ConfigureAwait(false))
                {
                    IEnumerable<LikesDto> items = await grid.ReadAsync<ItemLikesDto>().ConfigureAwait(false);
                    IEnumerable<LikesDto> replies = await grid.ReadAsync<ReplyLikesDto>().ConfigureAwait(false);
                    IEnumerable<LikesDto> comments = await grid.ReadAsync<CommentLikesDto>().ConfigureAwait(false);

                    return items.Union(replies).Union(comments);
                }
            }
        }

        public async Task<IEnumerable<SpamGunDto>> GetSpamGunDataAsync(int universityId, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenReliableConnectionAsync(token, "SpamGun").ConfigureAwait(false))
            {
                const string sql1 = @"select top 500 s.id, FirstName, Email,mailBody as MailBody,s.chapter,
mailsubject as MailSubject, mailcategory as MailCategory,u.url as UniversityUrl, u.name as school 
from students s join universities u on s.uniid = u.id
where uniid = @UniId
and shouldSend = 1
and chapter is not null
order by s.id
OPTION (TABLE HINT(s, INDEX ([students_shouldsend2])),Recompile);";
                const string sql2 = @"select top 500 s.id, FirstName, LastName, Email,mailBody as MailBody,
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
                            sql1 + sql2, new { UniId = universityId }, cancellationToken: token)).ConfigureAwait(false))
                    {
                        var result = await grid.ReadAsync<GreekPartnerDto>().ConfigureAwait(false);
                        return result.Union(await grid.ReadAsync<SpamGunDto>().ConfigureAwait(false));
                    }
                }, token).ConfigureAwait(false);
            }


        }

        public IEnumerable<long> GetUsersBadgeFollow(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select distinct u.userId from zbox.Users u join zbox.UserBoxRel ub on u.userId = ub.userId
except select userId from zbox.badge where name = 2 and progress = 100
 order by u.userId
 offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }

        public IEnumerable<long> GetUsersBadgeQuiz(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select distinct u.userId from zbox.Users u join zbox.Quiz q on u.userId = q.userId and q.IsDeleted = 0 and q.Publish = 1
except select userId from zbox.badge where name = 3 and progress = 100
 order by u.userId
 offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }
        public IEnumerable<long> GetUsersBadgeItem(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select distinct u.userId from zbox.Users u join zbox.Item q on u.userId = q.userId and q.IsDeleted = 0 
except select userId from zbox.badge where name = 4 and progress = 100
 order by u.userId
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
select userId from zbox.FlashcardLike
union 
select userId from zbox.quizLike2
except select userId from zbox.badge where name = 5 and progress = 100) t
order by OwnerId
offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }

        public async Task<IEnumerable<Tuple<long, string>>> GetDocumentsWithoutMd5Async(long id)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await
                    conn.QueryAsync<long, string, Tuple<long, string>>(
                        "select top 100  itemId,blobName from zbox.item where md5 is null and discriminator = 'file' and isDeleted = 0 and itemId >= @id order by itemId",
                        Tuple.Create, splitOn: "*",param: new { id }).ConfigureAwait(false);
            }
        }


        public async Task<IEnumerable<Tuple<long, decimal>>> GetDuplicateDocumentsAsync()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                return await
                    conn.QueryAsync<long, decimal, Tuple<long, decimal>>(
                        @"select itemId, likeCount *15 + numberOfViews *0.1 + numberOfDownloads *0.2 as score from zbox.item where md5 in (
select top(1) md5 from zbox.item
where isDeleted = 0
group by md5
having count(*) > 1
) order by score desc",
                        Tuple.Create, splitOn: "*").ConfigureAwait(false);
            }
        }

        public IEnumerable<long> GetUserReputationUpdate(int page)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                return conn.Query<long>(
                    @"select * from (
select userId from zbox.badge
union 
select userId from zbox.item where isDeleted = 0 and (NumberOfDownloads > 0 or NumberOfViews > 0 or LikeCount > 0)
union
select userId from zbox.Quiz where IsDeleted = 0 and Publish = 1 and (NumberOfViews > 0 or SolveCount > 0 or LikeCount > 0)
union
select userId from zbox.Flashcard where IsDeleted = 0 and Publish = 1 and (NumberOfViews > 0  or LikeCount > 0)
union
select userId from zbox.Question where LikeCount > 0
union
select userId from zbox.Answer where LikeCount > 0) t
order by userId
offset @page*100 ROWS
    FETCH NEXT 100 ROWS ONLY;", new { page });
            }
        }
    }
}
