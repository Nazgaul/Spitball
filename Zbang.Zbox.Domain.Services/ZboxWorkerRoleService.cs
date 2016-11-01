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

        public ZboxWorkerRoleService(ICommandBus commandBus, IWithCache cache)
        {
            m_CommandBus = commandBus;
            m_Cache = cache;
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

            RemoveHtmlTags();

        }

        public Task<int> DoDirtyUpdateAsync(CancellationToken token)
        {
            return ExecuteSqlLoopAsync(new[]
            {
                @"update zbox.box
set updatetime = getutcdate()-121
where university in (
select  id from zbox.university where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
)
and updatetime > getutcdate() - 120",
                @"update zbox.box
set isdirty = 1, isdeleted = 1 from zbox.box 
where libraryid in (
select libraryid from Zbox.Library where id in (
select id from zbox.university where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 
)) and isdeleted = 0",
                @"update Zbox.item
set isdirty = 1, isdeleted = 1, updatetime = getutcdate()-121
 where  boxid in (
	select  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) and isdeleted = 0",
                 @"update Zbox.item
set updatetime = getutcdate()-121
 where  boxid in (
	select  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) and isdeleted = 1 and updatetime > getutcdate() - 120",
                @"update Zbox.quiz
set isdirty = 1, isdeleted = 1, updatetime = getutcdate()-121
 where  boxid in (
	select  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) and isdeleted = 0 and publish = 1",
                @"update Zbox.quiz
set updatetime = getutcdate()-121
 where  boxid in (
	select  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) and isdeleted = 1 and updatetime > getutcdate() - 120"
            }, token);
        }
        public async Task<int> DeleteOldQuizAsync(CancellationToken token)
        {
            var counter = 0;
            counter +=
                await ExecuteSqlLoopAsync(
                    new[] {
                        @"delete from Zbox.SolvedQuestion where quizid in (
	select top(10) id from zbox.quiz where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 
)option (maxdop 1)",
                        @"delete from Zbox.QuizQuestion where quizid in (
	select top(10) id from zbox.quiz where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 
)option (maxdop 1)",
                        @"delete from Zbox.QuizAnswer where quizid in (
	select top(10) id from zbox.quiz where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 
)option (maxdop 1)",
                        @"delete from Zbox.SolvedQuiz where quizid in (
	select top(10) id from zbox.quiz where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 
)option (maxdop 1)",
                        "delete top (10) from zbox.quiz where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 option (maxdop 1)"
                    }, token);
            return counter;
        }
        public async Task<int> DeleteOldItemAsync(CancellationToken token)
        {
            var counter = 0;
            counter +=
                await ExecuteSqlLoopAsync(
                    new[] {
                        @"delete from zbox.ItemCommentReply where itemid in (

    select top (100) itemid from zbox.item where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option(maxdop 1)",
                        @"delete from zbox.itemcomment where itemid in (
	select  top (100) itemid from zbox.item where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from Zbox.NewUpdates where itemid in (
	select top (100) itemid from zbox.item where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from Zbox.itemrate where itemid in (
	select top (100) itemid from zbox.item where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        "delete top (100) from zbox.item where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 option (maxdop 1)"
                    }, token);
            return counter;

        }
        public Task<int> DeleteOldBoxAsync(CancellationToken token)
        {
            return
                ExecuteSqlLoopAsync(
                    new[]
                    {
                        @"delete from Zbox.NewUpdates where boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from Zbox.Invite where boxid in (
	select top (3) boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from Zbox.UserBoxRel where boxid in (
	select top (3) boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from Zbox.Message where boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from zbox.replylike where replyid in (
select answerid from zbox.answer where boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
)) option (maxdop 1)",
                        @"delete from zbox.answer where boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from zbox.answer where questionid in (
select questionid from Zbox.Question where boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
)) option (maxdop 1)",
                        @"delete from Zbox.Question where boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from Zbox.ItemTab where boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete  from Zbox.QuizQuestion where quizid in (
	select id  from Zbox.quiz where publish = 0
and boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) 
) option (maxdop 1)",
                        @"delete  from Zbox.quiz where publish = 0
and boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete  from Zbox.item where isdeleted = 1 and isdirty = 0
and boxid in (
	select top(3)  boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                        @"delete from zbox.box where boxid in (
select top (3) boxid  from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 order by boxid
) option(maxdop 1)"
                    }, token);

        }

        public Task<int> DeleteOldUniversityAsync(CancellationToken token)
        {
            return ExecuteSqlLoopAsync(
                new[]
                {
    @"delete from zbox.userlibraryrel where libraryid in (
select libraryid from Zbox.Library where id in (
select top(3) id from zbox.university where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
)) option (maxdop 1)",
                    @"delete from Zbox.Library where id in (
select top(3) id from zbox.university where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
) option (maxdop 1)",
                    @"update zbox.users set universityid = null
where universityid in (
select top(3) id from zbox.university where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
)
",
                    @"delete top(3) from zbox.university where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0 option (maxdop 1)"
                }, token);
        }

        private static async Task<int> ExecuteSqlLoopAsync(string[] sqls, CancellationToken token)
        {
            var needToLoop = true;
            var counter = 0;
            while (needToLoop && !token.IsCancellationRequested)
            {
                using (var conn = await DapperConnection.OpenConnectionAsync(token))
                {
                    try
                    {
                        var i = 0;
                        foreach (var sql in sqls)
                        {
                            i += await conn.ExecuteAsync(new CommandDefinition(sql, cancellationToken: token));
                            await Task.Delay(TimeSpan.FromSeconds(1), token);
                        }
                        //var i = await conn.ExecuteAsync(new CommandDefinition(sql, cancellationToken: token));
                        await Task.Delay(TimeSpan.FromSeconds(1), token);
                        needToLoop = i > 0;
                        counter += i;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == -2) //timeout
                        {
                            TraceLog.WriteError("ExecuteSqlLoop number -2 timout", ex);
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
            return ExecuteSqlLoopAsync(new[] { "delete top (100)  from zbox.newUpdates where CreationTime < getutcdate() - 90 option (maxdop 1)" }, token);
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
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "update zbox.University set isdirty = 0 where id in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text);
            }
        }

        public async Task UpdateSearchBoxDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "update zbox.Box set isdirty = 0 where boxid in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text);
            }
        }
        public async Task UpdateSearchItemDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "update zbox.Item set isdirty = 0 where itemid in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text);
            }
        }

        public async Task UpdateSearchQuizDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "update zbox.Quiz set isdirty = 0 where id in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text);
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
                await m_CommandBus.SendAsync(command);
                unitOfWork.TransactionalFlush();
            }
        }




        public void Statistics(UpdateStatisticsCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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

        public async Task UpdateSpamGunSendAsync(int id, CancellationToken token)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(token, "SpamGun"))
            {
                const string sql = @"UPDATE students 
SET [Sent] = cast ( Coalesce([Sent],'') as nvarchar(max))  + cast (GetUtcDate() as nvarchar(max) ) + ' ',
shouldSend = 0
where id = @id";

                await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: token));
            }
        }



        //        private void UpdateNumberOfBoxesInDepartmentNode()
        //        {
        //            var i = 0;
        //            using (var unitOfWork = UnitOfWork.Start())
        //            {
        //                while (true)
        //                {
        //                    var libs = UnitOfWork.CurrentSession.Connection.Query<Guid>(
        //                        @"select LibraryId from zbox.Library l 
        //where l.LibraryId not in ( select l.ParentId from zbox.Library)
        //and id = 166100
        //order by LibraryId
        //offset @pageNumber*50 ROWS
        //    FETCH NEXT 50 ROWS ONLY", new { pageNumber = i });
        //                    var libraryIds = libs as IList<Guid> ?? libs.ToList();
        //                    if (libraryIds.Count == 0)
        //                    {
        //                        break;
        //                    }
        //                    foreach (var libraryId in libraryIds)
        //                    {
        //                        var library = UnitOfWork.CurrentSession.Load<Library>(libraryId);
        //                        var libBoxes = library.UpdateNumberOfBoxes();
        //                        var libNodes = library.UpdateNumberOfNodes();
        //                        UnitOfWork.CurrentSession.Save(library);
        //                        foreach (var libBox in libBoxes)
        //                        {
        //                            UnitOfWork.CurrentSession.Save(libBox);
        //                        }
        //                        foreach (var libBox in libNodes)
        //                        {
        //                            UnitOfWork.CurrentSession.Save(libBox);
        //                        }
        //                    }
        //                    unitOfWork.TransactionalFlush();
        //                    i++;
        //                }
        //            }
        //        }





    }
}
