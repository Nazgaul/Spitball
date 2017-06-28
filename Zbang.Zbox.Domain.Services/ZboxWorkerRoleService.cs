using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using NHibernate.Criterion;
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
        // private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;

        public ZboxWorkerRoleService(ICommandBus commandBus, IWithCache cache/*, IDocumentDbRepository<Flashcard> flashRep*/)
        {
            m_CommandBus = commandBus;
            m_Cache = cache;
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
            UpdateItemUrl();
            //UpdateBoxUrl();
            //UpdateFlashcardCardCount();
            //RemoveHtmlTags();

        }

        //private async void UpdateFlashcardCardCount()
        //{
        //    using (var unitOfWork = UnitOfWork.Start())
        //    {
        //        var items2 = UnitOfWork.CurrentSession.CreateSQLQuery(@"select id from zbox.flashcard where publish=1 and isDeleted=0 and cardCount=0 or cardCount is null").List();
        //        foreach (var itemId in items2)
        //        {

        //            var item = UnitOfWork.CurrentSession.Load<FlashcardMeta>((long)itemId);
        //            var flashcard = await m_FlashcardRepository.GetItemAsync(itemId.ToString());
        //            if (flashcard != null && flashcard.Cards != null)
        //            {
        //                item.IsDirty = true;
        //                item.CardCount = flashcard.Cards.Count();
        //                UnitOfWork.CurrentSession.Save(item);
        //            }
        //            //var flash=m_FlashcardRepository.GetItemAsync(item.id);                    
        //        }
        //        unitOfWork.TransactionalFlush();
        //    }
        //}
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
        public Task<int> DeleteOldBoxAsync(CancellationToken token)
        {
            return
                ExecuteSqlLoopAsync(
                    new[]
                    {
                        @"delete from Zbox.NewUpdates where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        @"delete from Zbox.Invite where boxId in (
	select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        @"delete from Zbox.UserBoxRel where boxId in (
	select top (3) boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        @"delete from Zbox.Message where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        @"delete from zbox.replyLike where replyId in (
select answerId from zbox.answer where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
)) option (maxDop 1)",
                        @"delete from zbox.answer where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
) option (maxDop 1)",
                        @"delete from zbox.answer where questionId in (
select questionId from Zbox.Question where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
)) option (maxDop 1)",
                        @"delete from Zbox.CommentLike where commentId in (
select questionId from Zbox.Question where boxId in (
	select top(3)  boxId  from zbox.box where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0
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
) option(maxDop 1)"
                       
                    }, token);

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
                    @"delete top(3) from zbox.university where isDeleted = 1 and updateTime < getUtcDate() - 120 and isDirty = 0 option (maxDop 1)"
                }, token);
        }

        private static async Task<int> ExecuteSqlLoopAsync(string[] sqls, CancellationToken token)
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
                        foreach (var sql in sqls)
                        {
                            i += await conn.ExecuteAsync(new CommandDefinition(sql, cancellationToken: token)).ConfigureAwait(false);
                            await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
                        }
                        needToLoop = i > 0;
                        counter += i;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == -2) //timeout
                        {
                            TraceLog.WriteError("ExecuteSqlLoop number -2 timeout", ex);
                        }
                        else
                        {
                            throw;
                        }
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
                } while (universitiesIds.Any());
            }
        }

        //public void AddParentIdToBox()
        //{
        //    using (var unitOfWork = UnitOfWork.Start())
        //    {
        //        var query =
        //            UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
        //                .Where(w => w.ParentDepartment == null).Take(100);
        //        var m_DepartmentRepository = IocFactory.IocWrapper.Resolve<ILibraryRepository>();
        //        var list = query.List();
        //        do
        //        {
        //            //var list = query.List();
        //            foreach (var academicBox in list)
        //            {
        //                var topDepartmentId = m_DepartmentRepository.GetTopTreeNode(academicBox.Department.Id);
        //                var topDepartment = m_DepartmentRepository.Load(topDepartmentId);
        //                academicBox.ParentDepartment = topDepartment;
        //                UnitOfWork.CurrentSession.Save(academicBox);
        //                unitOfWork.TransactionalFlush();
        //            }
        //            list = query.List();
        //        } while (list.Count > 0);
        //        //.Where(Restrictions.On<CommentReply>(x => x.Text).IsLike("%quot;%")).Take(100);
        //    }
        //}
        public void RemoveHtmlTags()
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                //var i = 0;
                //var query = UnitOfWork.CurrentSession.QueryOver<Question>()
                //    //.Where(w => w.Quiz.Id == 13970)
                //    .Where(w => w.Text.IsLike("%<%"));


                //var questions = query.Skip(i * 100).Take(100).List();
                //do
                //{
                //    foreach (var question in questions)
                //    {
                //        var textBefore = question.Text;
                //        //question.Text = TextManipulation.EncodeText(question.Text, Question.AllowedHtmlTag);

                //        if (textBefore != question.Text)
                //        {
                //            UnitOfWork.CurrentSession.Save(question);
                //        }
                //    }
                //    unitOfWork.TransactionalFlush();
                //    i++;
                //    questions = query.Skip(i * 100).Take(100).List();
                //} while (questions.Count > 0);

                var query =
                    UnitOfWork.CurrentSession.QueryOver<Comment>()
                        .Where(Restrictions.On<CommentReply>(x => x.Text).IsLike("%quot;%")).Take(100);

                var comments = query.List();
                do
                {
                    foreach (var comment in comments)
                    {
                        //comment.Text = Infrastructure.TextManipulation.RemoveHtmlTags.Replace(comment.Text, string.Empty);
                        comment.Text = WebUtility.HtmlDecode(comment.Text);

                        UnitOfWork.CurrentSession.Save(comment);

                    }
                    unitOfWork.TransactionalFlush();
                    comments = query.List();
                } while (comments.Count > 0);

            }
        }

        //public async Task<long> UpdateFileSizesAsync(Action callback)
        //{
        //    var count = 0;
        //    using (var unitOfWork = UnitOfWork.Start())
        //    {
        //        var query = UnitOfWork.CurrentSession.GetNamedQuery("ItemWithNoSize");
        //        query.SetMaxResults(100);
        //        var itemIds = query.List<long>();

        //        do
        //        {
        //            count += itemIds.Count;
        //            try
        //            {
        //                var command = new UpdateItemWithNoSizeCommand(itemIds);
        //                await m_CommandBus.SendAsync(command);
        //                unitOfWork.TransactionalFlush();
        //            }
        //            catch (Exception ex)
        //            {
        //                TraceLog.WriteError(ex);
        //            }
        //            callback();
        //            query = UnitOfWork.CurrentSession.GetNamedQuery("ItemWithNoSize");
        //            query.SetMaxResults(100);
        //            itemIds = query.List<long>();
        //        } while (itemIds.Any());

        //    }
        //    return count;
        //}




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
                await m_CommandBus.SendAsync(command);
                if (command.UniversityId > 0)
                {
                    await m_Cache.RemoveAsync(command);
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
                    TraceLog.WriteError("error on Add new updates continue", ex);
                }
                unitOfWork.TransactionalFlush();
            }
        }




        public async Task StatisticsAsync(UpdateStatisticsCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
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
            using (var conn = await DapperConnection.OpenConnectionAsync(token, "SpamGun"))
            {
                const string sql = @"UPDATE students 
SET [Sent] = cast ( Coalesce([Sent],'') as nvarchar(max))  + cast (getUtcDate() as nvarchar(max) ) + ' ',
shouldSend = 0
where id = @id";

                await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: token));
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

                var items = UnitOfWork.CurrentSession.QueryOver<Item>().Where(w => w.Url == null && !w.IsDeleted).List();
                foreach (var item in items)
                {
                    item.GenerateUrl();
                    UnitOfWork.CurrentSession.Save(item);
                }
                unitOfWork.TransactionalFlush();

                //                var items2 = UnitOfWork.CurrentSession.CreateSQLQuery(@"select itemId from zbox.item
                //where  CHARINDEX(CAST(boxId as varchar(max)), URL) = 0
                //and isDeleted = 0").List();
                //                foreach (var itemId in items2)
                //                {
                //                    var item = UnitOfWork.CurrentSession.Load<Item>(itemId);
                //                    item.GenerateUrl();
                //                    UnitOfWork.CurrentSession.Save(item);
                //                }
                //                unitOfWork.TransactionalFlush();
            }
        }

        //private void DeleteOldLibrary()
        //{
        //    using (var con = DapperConnection.OpenConnection())
        //    {
        //        var guids = con.Query<Guid>("select libraryId from zbox.library where name like '%~%'");
        //        var i = 0;
        //        foreach (var guid in guids)
        //        {
        //            Console.WriteLine(guid);
        //            try
        //            {
        //                i += con.Execute("delete from zbox.library where parentid = @id", new { id = guid });
        //                i += con.Execute("delete from zbox.library where libraryId = @id", new {id = guid});


        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }
        //        Console.WriteLine(i);

        //    }
        //}

        //private void UpdateNumberOfBoxesInDepartmentNode()
        //{
        //    var i = 0;
        //    using (var unitOfWork = UnitOfWork.Start())
        //    {
        //        while (true)
        //        {
        //            var libs = UnitOfWork.CurrentSession.Connection.Query<Guid>(
        //                @"select libraryId from zbox.Library l 
        //where l.libraryId not in ( select l.ParentId from zbox.Library)
        //order by libraryId
        //offset @pageNumber*50 ROWS
        //    FETCH NEXT 50 ROWS ONLY", new { pageNumber = i });
        //            var libraryIds = libs as IList<Guid> ?? libs.ToList();
        //            if (libraryIds.Count == 0)
        //            {
        //                break;
        //            }
        //            foreach (var libraryId in libraryIds)
        //            {
        //                var library = UnitOfWork.CurrentSession.Load<Library>(libraryId);
        //                var libBoxes = library.UpdateNumberOfBoxes();
        //                var libNodes = library.UpdateNumberOfNodes();
        //                UnitOfWork.CurrentSession.Save(library);
        //                foreach (var libBox in libBoxes)
        //                {
        //                    UnitOfWork.CurrentSession.Save(libBox);
        //                }
        //                foreach (var libBox in libNodes)
        //                {
        //                    UnitOfWork.CurrentSession.Save(libBox);
        //                }
        //            }
        //            unitOfWork.TransactionalFlush();
        //            i++;
        //        }
        //    }
        //}





    }
}
