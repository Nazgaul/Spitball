using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Domain.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ZboxWorkerRoleService : IZboxWorkerRoleService
    {
        private readonly ICommandBus m_CommandBus;
        private readonly IWithCache m_Cache;
        private readonly ILogger m_Logger;

        public ZboxWorkerRoleService(ICommandBus commandBus, IWithCache cache, ILogger logger
/*, IDocumentDbRepository<Flashcard> flashRep*/)
        {
            m_CommandBus = commandBus;
            m_Cache = cache;
            m_Logger = logger;
            // m_FlashcardRepository = flashRep;
        }

        public void UpdateThumbnailPicture(UpdateThumbnailCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void ChangeBoxDepartment(ChangeBoxLibraryCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void OneTimeDbi()
        {
            // DeleteOldLibrary();
            //UpdateNumberOfBoxesInDepartmentNode();
            //UpdateItemUrl();
            UpdateBoxUrl();
            //UpdateFlashcardCardCount();
            //RemoveHtmlTags();

        }

        public Task<int> DoDirtyUpdateAsync(CancellationToken token)
        {
            return ExecuteSqlLoopAsync(new[]
            {
                @"update zbox.box
set updateTime = getUtcDate()-121
where university in (
select  id from zbox.university where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
)
and updateTime > getUtcDate() - 120",
                @"update zbox.box
set isDirty = 1, isDeleted = 1 from zbox.box 
where libraryId in (
select libraryId from Zbox.Library where id in (
select id from zbox.university where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 
)) and isDeleted = 0",
                @"update top(10) Zbox.item
set isDirty = 1, isDeleted = 1, updateTime = getUtcDate()-121
 where  boxId in (
	select  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) and isDeleted = 0",
                 @"update Zbox.item
set updateTime = getUtcDate()-121
 where  boxId in (
	select  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) and isDeleted = 1 and updateTime > getUtcDate() - 120",
                @"update Zbox.quiz
set isDirty = 1, isDeleted = 1, updateTime = getUtcDate()-121
 where  boxId in (
	select  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) and isDeleted = 0 and publish = 1",
                @"update Zbox.quiz
set updateTime = getUtcDate()-121
 where  boxId in (
	select  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) and isDeleted = 1 and updateTime > getUtcDate() - 120",
                @"update Zbox.flashcard
set isDirty = 1, isDeleted = 1, updateTime = getUtcDate()-121
 where  boxId in (
	select  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) and isDeleted = 0 and publish = 1",

            }, token);
        }

        public async Task<int> DeleteOldQuizAsync(CancellationToken token)
        {
            var counter = 0;
            counter +=
                await ExecuteSqlLoopAsync(
                    new[] {
                        @"delete from Zbox.SolvedQuestion where quizId in (
	select top(10) id from zbox.quiz where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 
)option (maxDop 1)",
                        @"delete from Zbox.QuizQuestion where quizId in (
	select top(10) id from zbox.quiz where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 
)option (maxDop 1)",
                        @"delete from Zbox.QuizAnswer where quizId in (
	select top(10) id from zbox.quiz where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 
)option (maxDop 1)",
                        @"delete from Zbox.SolvedQuiz where quizId in (
	select top(10) id from zbox.quiz where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 
)option (maxDop 1)",
                        "delete top (10) from zbox.quiz where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 option (maxDop 1)"
                    }, token).ConfigureAwait(false);
            return counter;
        }

        public async Task<int> DeleteOldFlashcardAsync(CancellationToken token)
        {
            var counter = 0;
            counter +=
                await ExecuteSqlLoopAsync(
                    new[] {
                        @"delete from zbox.ItemTag where flashcardId in (
	select top (10) id  from zbox.flashcard where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
    )option(maxDop 1)",
                        "delete top (10)  from zbox.flashcard where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 option (maxDop 1)"
                    }, token).ConfigureAwait(false);
            return counter;
        }

        public async Task<int> DeleteOldItemAsync(CancellationToken token)
        {
            var counter = 0;
            counter +=
                await ExecuteSqlLoopAsync(
                    new[] {
                        @"delete from zbox.ItemCommentReply where itemId in (

    select top (50) itemId from zbox.item where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option(maxDop 1)",
                        @"delete from zbox.itemComment where itemId in (
	select  top (50) itemId from zbox.item where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        @"delete from Zbox.NewUpdates where itemId in (
	select top (50) itemId from zbox.item where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        @"delete from Zbox.itemRate where itemId in (
	select top (50) itemId from zbox.item where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        @"delete from Zbox.ItemTag where itemId in (
	select top (50) itemId from zbox.item where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        "delete top (50) from zbox.item where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 option (maxDop 1)"
                    }, token).ConfigureAwait(false);
            return counter;
        }

        public async Task<int> DeleteOldBoxAsync(CancellationToken token)
        {
            int result;
            var counter = 0;
            do
            {
                await
                    ExecuteSqlLoopAsync(
                        new[]
                        {
                            @"delete from Zbox.NewUpdates where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
) option (maxDop 1)",
                            @"delete from Zbox.Invite where boxId in (
	select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
) option (maxDop 1)",
                            @"delete from Zbox.UserBoxRel where boxId in (
	select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
) option (maxDop 1)",
                            @"delete from Zbox.Message where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
) option (maxDop 1)",
                            @"delete from zbox.replyLike where replyId in (
select answerId from zbox.answer where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
)) option (maxDop 1)",
                            @"delete from zbox.answer where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
) option (maxDop 1)",
                            @"delete from zbox.answer where questionId in (
select questionId from Zbox.Question where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
)) option (maxDop 1)",
                            @"delete from Zbox.CommentLike where commentId in (
select questionId from Zbox.Question where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
) 
)option (maxDop 1)",
                            @"delete from zbox.commentLike where boxId in (
select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId)",

                            @"delete top(3) from Zbox.Question where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0  order by boxId
) option (maxDop 1)",
                            @"delete from Zbox.itemTab where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0  order by boxId
) option (maxDop 1)",
                            @"delete  from Zbox.QuizQuestion where quizId in (
	select id  from Zbox.quiz where publish = 0
and boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0  order by boxId
) 
) option (maxDop 1)",
                            @"delete from zbox.quizAnswer where quizId in 
(select id  from Zbox.quiz where publish = 0
and boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0  order by boxId
)) option (maxDop 1)",
                            @"delete  from Zbox.quiz where publish = 0
and boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0  order by boxId
) option (maxDop 1)",
                            @"delete  from Zbox.item where isDeleted = 1 and isDirty = 0
and boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0  order by boxId
) option (maxDop 1)",
                            @"delete from zbox.flashcard where boxId in (
select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
)  and isDeleted = 1 and isDirty = 0 option(maxDop 1)",
                            @"update zbox.item
set itemTabId = null , isDirty = 1
where itemTabId in (
select itemTabId from zbox.itemTab where boxId in (
select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
))",
                            @"delete from zbox.itemTab where boxId in (
select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
) option(maxDop 1)",

                        }, token).ConfigureAwait(false);

                using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
                {
                    result = await conn.ExecuteAsync(@"delete from zbox.box where boxId in (
                select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 order by boxId
                ) option(maxDop 1)").ConfigureAwait(false);
                    counter += result;
                }
            } while (result > 0);
            return counter;
        }

        public Task<int> DeleteOldUniversityAsync(CancellationToken token)
        {
            return ExecuteSqlLoopAsync(
                new[]
                {
    @"delete from zbox.userLibraryRel where libraryId in (
select libraryId from Zbox.Library where id in (
select top(3) id from zbox.university where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
)) option (maxDop 1)",
                    @"delete from Zbox.Library where id in (
select top(3) id from zbox.university where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                    @"update zbox.users set universityId = null
where universityId in (
select top(3) id from zbox.university where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
)
",
                    "delete top(3) from zbox.university where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 option (maxDop 1)"
                }, token);
        }

        private async Task<int> ExecuteSqlLoopAsync(string[] sqlStatements, CancellationToken token)
        {
            var needToLoop = true;
            var counter = 0;
            while (needToLoop && !token.IsCancellationRequested)
            {
                using (var conn = await DapperConnection.OpenConnectionAsync(token).ConfigureAwait(false))
                {
                    try
                    {
                        var i = 0;
                        foreach (var sql in sqlStatements)
                        {
                            i += await conn.ExecuteAsync(new CommandDefinition(sql, cancellationToken: token)).ConfigureAwait(false);
                            await Task.Delay(TimeSpan.FromMilliseconds(1000), token).ConfigureAwait(false);
                        }
                        needToLoop = i > 0;
                        counter += i;
                    }
                    catch (SqlException ex) when (ex.Number == -2)
                    {
                        m_Logger.Exception(ex, new Dictionary<string, string> {["ExecuteSqlLoopAsync"] = "timeout" });
                    }
                }
            }
            return counter;
        }

        public Task<int> DeleteOldUpdatesAsync(CancellationToken token)
        {
            return ExecuteSqlLoopAsync(new[] { "delete top (100)  from zbox.newUpdates where CreationTime < getUtcDate() - 45 option (maxDop 1)" }, token);
        }

        public void UpdateUniversityStats(DateTime dateTime)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                var i = 0;
                var query = UnitOfWork.CurrentSession.GetNamedQuery("UniversityData");
                query.SetDateTime("dateTime", dateTime);
                query.SetMaxResults(100);
                query.SetFirstResult(i * 100);
                var universitiesIds = query.List<long>();
                do
                {
                    var command = new UpdateUniversityStatsCommand(universitiesIds);
                    m_CommandBus.Send(command);
                    unitOfWork.TransactionalFlush();
                    i++;
                    query = UnitOfWork.CurrentSession.GetNamedQuery("UniversityData");
                    query.SetDateTime("dateTime", dateTime);
                    query.SetMaxResults(100);
                    query.SetFirstResult(i * 100);
                    universitiesIds = query.List<long>();
                } while (universitiesIds.Count > 0);
            }
        }

        public void UpdateReputation(UpdateReputationCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public async Task UpdateSearchUniversityDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = "update zbox.University set isDirty = 0 where id in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text).ConfigureAwait(false);
            }
        }

        public async Task UpdateSearchBoxDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = "update zbox.Box set isDirty = 0 where boxId in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text).ConfigureAwait(false);
            }
        }

        public async Task UpdateSearchItemDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = "update zbox.Item set isDirty = 0 where itemId in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text).ConfigureAwait(false);
            }
        }

        public async Task UpdateSearchQuizDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = "update zbox.Quiz set isDirty = 0 where id in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text).ConfigureAwait(false);
            }
        }

        public async Task UpdateSearchFlashcardDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync().ConfigureAwait(false))
            {
                const string sql = "update zbox.Flashcard set isDirty = 0 where id in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text).ConfigureAwait(false);
            }
        }

        public void UpdateUserFromUnsubscribe(UnsubscribeUsersFromEmailCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public async Task DeleteBoxAsync(DeleteBoxCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command).ConfigureAwait(true);
                if (command.UniversityId > 0)
                {
                    await m_Cache.RemoveAsync(command).ConfigureAwait(true);
                }
                unitOfWork.TransactionalFlush();
            }
        }

        public async Task AddNewUpdateAsync(AddNewUpdatesCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                try
                {
                    await m_CommandBus.SendAsync(command).ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    //this can only happen from the await
                    m_Logger.Exception(ex, new Dictionary<string, string> {["AddNewUpdateAsync"] = "" });
                }
                unitOfWork.TransactionalFlush();
            }
        }

        public async Task StatisticsAsync(UpdateStatisticsCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command).ConfigureAwait(true);
                unitOfWork.TransactionalFlush();
            }
        }

        public void UpdateQuota(UpdateQuotaCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void UpdateBadges(UpdateBadgesCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public async Task UpdateSpamGunSendAsync(int id, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token, "SpamGun").ConfigureAwait(false))
            {
                const string sql = @"UPDATE students2 
SET [Sent] = cast ( Coalesce([Sent],'') as nvarchar(max))  + cast (getUtcDate() as nvarchar(max) ) + ' ',
shouldSend = 0
where id = @id";

                await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: token)).ConfigureAwait(false);
            }
        }

        public void UpdateBoxUrl()
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                var items = UnitOfWork.CurrentSession.QueryOver<Box>().Where(w => w.Url == null && !w.IsDeleted).Take(100).List();
                do
                {
                    foreach (var item in items)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    unitOfWork.TransactionalFlush();
                    items = UnitOfWork.CurrentSession.QueryOver<Box>().Where(w => w.Url == null && !w.IsDeleted)
                        .Take(100).List();
                } while (items.Count > 0);
            }
        }

        public void UpdateItemUrl()
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                var items = UnitOfWork.CurrentSession.QueryOver<Item>().Where(w => w.Url == null && !w.IsDeleted)
                    .List();
                foreach (var item in items)
                {
                    item.GenerateUrl();
                    UnitOfWork.CurrentSession.Save(item);
                }
                unitOfWork.TransactionalFlush();
            }
        }
    }
}
