using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NHibernate.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
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

        public ZboxWorkerRoleService(ICommandBus commandBus)
        {
            m_CommandBus = commandBus;
        }

        public void UpdateThumbnailPicture(UpdateThumbnailCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void OneTimeDbi()
        {
            using (UnitOfWork.Start())
            {
                var items = UnitOfWork.CurrentSession.QueryOver<Item>().Where(w => !w.IsDeleted && w.Url == null).List();
                foreach (var item in items)
                {
                    item.GenerateUrl();
                    UnitOfWork.CurrentSession.Save(item);
                }
                UnitOfWork.Current.TransactionalFlush();
            }
            //UpdateUsersReputation();
            //UpdateMismatchUrl();
            // UpdateHierarchyInLibrary();
        }

        /*private void UpdateUsersReputation()
        {
            int i = 827;
            IList<long> users;


            using (UnitOfWork.Start())
            {
                users = UnitOfWork.CurrentSession.QueryOver<User>()
                    .Where(w => w.Id == 496060)
                    //.OrderBy(o => o.Id).Asc
                    .Select(s => s.Id)
                    //.Skip(i * 100).Take(100)
                    .List<long>();
            }
            do
            {
                TraceLog.WriteInfo("index: " + i);
                var command = new UpdateReputationCommand(users);
                UpdateReputation(command);
                i++;

                using (UnitOfWork.Start())
                {
                    users = UnitOfWork.CurrentSession.QueryOver<User>()
                        .Where(w => w.IsRegisterUser)
                        .OrderBy(o => o.Id).Asc
                        .Select(s => s.Id)
                        .Skip(i * 100).Take(100)
                        .List<long>();
                }
            } while (users.Count > 0);


        }*/

        /* private void UpdateNumberOfBoxesInDepartmentNode()
         {
             var i = 0;
             using (var unitOfWork = UnitOfWork.Start())
             {
                 while (true)
                 {
                     var libs = UnitOfWork.CurrentSession.Connection.Query<Guid>(
                         @"select LibraryId from zbox.Library l where l.LibraryId not in ( select l.ParentId from zbox.Library)
 order by LibraryId
 offset @pageNumber*50 ROWS
     FETCH NEXT 50 ROWS ONLY", new { pageNumber = i });
                     var libraryIds = libs as IList<Guid> ?? libs.ToList();
                     if (libraryIds.Count == 0)
                     {
                         break;
                     }
                     foreach (var libraryId in libraryIds)
                     {
                         var library = UnitOfWork.CurrentSession.Load<Library>(libraryId);
                         library.UpdateNumberOfBoxes();
                         UnitOfWork.CurrentSession.Save(library);
                     }
                     unitOfWork.TransactionalFlush();
                     i++;
                 }
             }
         }*/


        //        private void UpdateHierarchyInLibrary()
        //        {
        //            //const long universityId = 920;


        //            //first round
        //            using (var unitOfWork = UnitOfWork.Start())
        //            {
        //                var universitiesId = UnitOfWork.CurrentSession.Connection.Query<long?>(@"select distinct id from zbox.Library");
        //                foreach (var universityId in universitiesId)
        //                {
        //                    IList libraryIds =
        //                        UnitOfWork.CurrentSession.Connection.Query<Guid>(
        //                            @"select LibraryId from zbox.library where id =@universityId and ParentId is null and level is null;", new { universityId }).ToList();


        //                    var updateMade = false;
        //                    //var libraries = UnitOfWork.CurrentSession.Query<Library>()
        //                    //    .Where(w => w.University.Id == universityId && w.Parent == null)
        //                    //    .ToList();
        //                    foreach (var libraryId in libraryIds)
        //                    {
        //                        var lib = UnitOfWork.CurrentSession.Load<Library>(libraryId);
        //                        updateMade = lib.UpdateLevel();
        //                        if (updateMade)
        //                            UnitOfWork.CurrentSession.Save(lib);

        //                    }
        //                    if (updateMade)
        //                        unitOfWork.TransactionalFlush();

        //                    updateMade = false;
        //                    libraryIds =
        //                        UnitOfWork.CurrentSession.Connection.Query<Guid>(@"select LibraryId from zbox.Library where ParentId in (
        //select LibraryId from zbox.Library where id = @universityId and level is not null) and level is null;",
        //                            new { universityId }).ToList();
        //                    do
        //                    {
        //                        foreach (var libraryId in libraryIds)
        //                        {
        //                            var lib = UnitOfWork.CurrentSession.Load<Library>(libraryId);
        //                            updateMade = lib.UpdateLevel();
        //                            if (updateMade)
        //                                UnitOfWork.CurrentSession.Save(lib);
        //                        }
        //                        if (updateMade)
        //                            unitOfWork.TransactionalFlush();
        //                        updateMade = false;
        //                        libraryIds =
        //                            UnitOfWork.CurrentSession.Connection.Query<Guid>(@"select LibraryId from zbox.Library where ParentId in (
        //select LibraryId from zbox.Library where id = @universityId and level is not null) and level is null;",
        //                                new { universityId }).ToList();
        //                    } while (libraryIds.Count > 0);

        //                }
        //            }

        //        }
        /// <summary>
        /// this function is greated due to merge tool
        /// </summary>
//        private void UpdateMismatchUrl()
//        {
//            using (var unitOfWork = UnitOfWork.Start())
//            {
//                const string sql1 = @"select top 10 itemid from zbox.item 
//where CHARINDEX(CAST(boxid as varchar),url) <= 0
//order by 1";
//                const string sql2 = @"select id from zbox.quiz where CHARINDEX(CAST(boxid as varchar),url) <= 0
//order by 1";

//                var breakLoop = true;
//                while (breakLoop)
//                {
//                    var quizzes = UnitOfWork.CurrentSession.Connection.Query<long>(sql2);

//                    breakLoop = false;
//                    //var items = UnitOfWork.CurrentSession.Query<Item>().Where(w => !w.IsDeleted && w.Url == null).ToList();
//                    foreach (var dQuiz in quizzes)
//                    {
//                        breakLoop = true;
//                        var quiz = UnitOfWork.CurrentSession.Load<Quiz>(dQuiz);
//                        quiz.GenerateUrl();
//                        quiz.IsDirty = true;
//                        UnitOfWork.CurrentSession.Save(quiz);
//                    }

//                    unitOfWork.TransactionalFlush();
//                }
//                breakLoop = true;
//                while (breakLoop)
//                {
//                    var items = UnitOfWork.CurrentSession.Connection.Query<long>(sql1);

//                    breakLoop = false;
//                    //var items = UnitOfWork.CurrentSession.Query<Item>().Where(w => !w.IsDeleted && w.Url == null).ToList();
//                    foreach (var dItem in items)
//                    {
//                        breakLoop = true;
//                        var item = UnitOfWork.CurrentSession.Load<Item>(dItem);
//                        item.GenerateUrl();
//                        item.IsDirty = true;
//                        UnitOfWork.CurrentSession.Save(item);
//                    }

//                    unitOfWork.TransactionalFlush();
//                }

//            }
//        }


        //        private void UpdateFeedDbi()
        //        {
        //            const string sql = @"update zbox.Question 
        //set ReplyCount = (select count(*) from zbox.Answer a where a.QuestionId = zbox.Question.QuestionId),
        //lastreplyid = (select max(a.AnswerId) from zbox.Answer a where a.QuestionId = zbox.Question.QuestionId)
        //";
        //            using (var con = DapperConnection.OpenConnection())
        //            {
        //                con.Execute(sql);
        //            }
        //        }



        //public bool Dbi()
        //{
        //    //DeleteOldUpdates();
        //    UpdateUniversityStats();
        //    //UpdateMismatchUrl();
        //    //UpdateHierarchyInLibrary();
        //    //UpdateFeedDbi();
        //    return false;
        //}

        public int DeleteOldUpdates()
        {

            var needToLoop = true;
            var counter = 0;
            while (needToLoop)
            {
                using (UnitOfWork.Start())
                {
                    var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteOldUpdates");
                    var result = query.ExecuteUpdate();
                    needToLoop = result > 0;
                    counter += result;
                }
            }
            using (UnitOfWork.Start())
            {
                var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteNotConnectedUpdates");
                var result = query.ExecuteUpdate();
                counter += result;
                //TraceLog.WriteInfo("delete old updates Delete Not Connected Updates " + result);
            }
            return counter;
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


    }
}
