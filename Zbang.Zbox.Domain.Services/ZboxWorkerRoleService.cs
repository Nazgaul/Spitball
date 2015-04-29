using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Proxy;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
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
            //UpdateQuestionHtml();
            //int i = 0;
            //IList<long> users;


            //using (UnitOfWork.Start())
            //{
            //    users = UnitOfWork.CurrentSession.QueryOver<User>()
            //        .Where(w => w.IsRegisterUser)
            //        .OrderBy(o => o.Id).Asc
            //        .Select(s => s.Id)
            //        .Skip(i * 100).Take(100)
            //        .List<long>();
            //}
            //do
            //{
            //    TraceLog.WriteInfo("index: " + i);
            //    var command = new UpdateReputationCommand(users);
            //    UpdateReputation(command);
            //    i++;

            //    using (UnitOfWork.Start())
            //    {
            //        users = UnitOfWork.CurrentSession.QueryOver<User>()
            //            .Where(w => w.IsRegisterUser)
            //            .OrderBy(o => o.Id).Asc
            //            .Select(s => s.Id)
            //            .Skip(i * 100).Take(100)
            //            .List<long>();
            //    }
            //} while (users.Count > 0);

        }

//        private void UpdateQuestionHtml()
//        {
//            var questions = new List<dynamic>();
//            using (UnitOfWork.Start())
//            {
//                questions =
//                    UnitOfWork.CurrentSession.Connection.Query(
//                        @"select q.userid,qq.id,qq.text from zbox.quizquestion qq 
//                    join zbox.quiz q on qq.quizid = q.id
//                  where qq.text like '%&lt;%'
//                    ").ToList();
//            }
//            foreach (dynamic question in questions)
//            {
//                var text = question.text;
//                text = System.Net.WebUtility.HtmlDecode(text);
//                var command = new Zbang.Zbox.Domain.Commands.Quiz.UpdateQuestionCommand(question.userid, question.id,
//                    text);

//                UpdateQuestion(command);

//            }
           
//        }


        //private void UpdateAllUrlsInSystem()
        //{
        //    int i = 667;
        //    TraceLog.WriteInfo("starting on item");
        //    var items = UnitOfWork.CurrentSession.QueryOver<Item>()
        //        //.Where(Restrictions.Gt(Projections.SqlFunction("CHARINDEX",
        //        //    NHibernateUtil.String,
        //        //    Projections.Constant("("),
        //        //    Projections.Property<Item>(p => p.Url)
        //        //    ),
        //        //    0))
        //        .And(w => w.IsDeleted == false)
        //        .OrderBy(o => o.Id).Asc
        //        .Skip(i * 100).Take(100).List();
        //    do
        //    {
        //        TraceLog.WriteInfo("item index " + i);
        //        using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
        //        {
        //            foreach (var item in items)
        //            {
        //                item.GenerateUrl();
        //                UnitOfWork.CurrentSession.Save(item);
        //            }
        //            tx.Commit();
        //            i++;
        //        }
        //        items = UnitOfWork.CurrentSession.QueryOver<Item>()
        //            .Where(w => w.IsDeleted == false)
        //            .OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
        //    } while (items.Count > 0);

        //    i = 0;
        //    TraceLog.WriteInfo("starting on box");
        //    var boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
        //        .Where(w => w.IsDeleted == false)
        //        .OrderBy(o => o.Id).Asc
        //        .Skip(i * 100).Take(100).List();
        //    do
        //    {
        //        TraceLog.WriteInfo("box index " + i);
        //        using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
        //        {
        //            foreach (var item in boxes)
        //            {
        //                item.GenerateUrl();
        //                UnitOfWork.CurrentSession.Save(item);
        //            }
        //            tx.Commit();
        //            i++;
        //        }
        //        boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
        //            .Where(w => w.IsDeleted == false)
        //            .OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
        //    } while (boxes.Count > 0);

        //    i = 0;
        //    TraceLog.WriteInfo("starting on quiz");
        //    var quizzes = UnitOfWork.CurrentSession.QueryOver<Quiz>()
        //        .Where(w => w.Publish)
        //        .OrderBy(o => o.Id).Asc
        //        .Skip(i * 100).Take(100).List();
        //    do
        //    {
        //        TraceLog.WriteInfo("quiz index " + i);
        //        using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
        //        {
        //            foreach (var item in quizzes)
        //            {
        //                item.GenerateUrl();
        //                UnitOfWork.CurrentSession.Save(item);
        //            }
        //            tx.Commit();
        //            i++;
        //        }
        //        quizzes = UnitOfWork.CurrentSession.QueryOver<Quiz>()
        //            .Where(w => w.Publish)
        //            .OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
        //    } while (quizzes.Count > 0);

        //    TraceLog.WriteInfo("starting on library");
        //    i = 0;
        //    var library = UnitOfWork.CurrentSession.QueryOver<Library>().OrderBy(o => o.Id).Asc
        //        .Skip(i * 100).Take(100).List();
        //    do
        //    {
        //        TraceLog.WriteInfo("library index " + i);
        //        using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
        //        {
        //            foreach (var item in library)
        //            {
        //                item.GenerateUrl();
        //                UnitOfWork.CurrentSession.Save(item);
        //            }
        //            tx.Commit();
        //            i++;
        //        }
        //        library = UnitOfWork.CurrentSession.QueryOver<Library>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
        //    } while (library.Count > 0);

        //    TraceLog.WriteInfo("starting on user");
        //    var users = UnitOfWork.CurrentSession.QueryOver<User>().OrderBy(o => o.Id).Asc
        //       .Skip(i * 100).Take(100).List();
        //    do
        //    {
        //        TraceLog.WriteInfo("user index " + i);
        //        using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
        //        {
        //            foreach (var item in users)
        //            {
        //                item.GenerateUrl();
        //                UnitOfWork.CurrentSession.Save(item);
        //            }
        //            tx.Commit();
        //            i++;
        //        }
        //        users = UnitOfWork.CurrentSession.QueryOver<User>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
        //    } while (users.Count > 0);
        //}

        public bool Dbi(int index)
        {
            UpdateUniversityStats();
            return false;
        }

        private void UpdateUniversityStats()
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                var universitiesIds = UnitOfWork.CurrentSession.Query<University>()
                    .Where(s => s.IsDeleted == false)
                    .Select(s => s.Id);
                var command = new UpdateUniversityStatsCommand(universitiesIds);
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }


        public void AddProducts(AddProductsToStoreCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void AddCategories(AddCategoriesCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void AddBanners(AddBannersCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
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
