using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NHibernate;
using NHibernate.Proxy;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;


namespace Zbang.Zbox.Domain.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class ZboxWriteService
    {


        public void UpdateThumbnailPicture(UpdateThumbnailCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void OneTimeDbi()
        {
            using (UnitOfWork.Start())
            {
                AddItemsToFeedDbi(); // this can work
                //UpdateAllUrlsInSystem();
            }
        }

        public void UpdateIdOfQuestion()
        {
            var questions = UnitOfWork.CurrentSession.QueryOver<Comment>()
                .Where(w => w.FeedType == FeedType.CreatedCourse)
                .List();
            foreach (var question in questions)
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var id = IdGenerator.GetGuid(question.DateTimeUser.CreationTime);
                    if (question.Id == id)
                    {
                        continue;

                    }
                    var items = new List<Item>();
                    items.AddRange(question.ItemsReadOnly);
                    var question2 = new Comment(question.User, null, question.Box, id, items,
                        FeedType.CreatedCourse);
                    question2.DateTimeUser.CreationTime = question.DateTimeUser.CreationTime;
                    UnitOfWork.CurrentSession.Save(question2);
                    foreach (var answer in question.AnswersReadOnly)
                    {
                        answer.Question = question2;
                        UnitOfWork.CurrentSession.Save(answer);
                    }
                    UnitOfWork.CurrentSession.Save(question2);
                    //UnitOfWork.CurrentSession.Delete(question);
                    tx.Commit();
                }
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    UnitOfWork.CurrentSession.Delete(question);
                    tx.Commit();
                }


            }
        }

        private void AddItemsToFeedDbi()
        {
            var commentRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IRepository<Comment>>();
            var items = UnitOfWork.CurrentSession.QueryOver<Item>()
                 .Where(w => w.IsDeleted == false)
                 .And(w => w.Comment == null)
                 .And(w => w.Answer == null)
                 .OrderBy(w => w.DateTimeUser.CreationTime).Asc
                 .List();
            foreach (var item in items)
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var comment = GetPreviousCommentId(item.BoxId, item.UploaderId, item.DateTimeUser.CreationTime);
                    if (comment == null)
                    {

                        comment = item.Box.AddComment(item.Uploader, null, IdGenerator.GetGuid(item.DateTimeUser.CreationTime),
                               null, FeedType.AddedItems);
                        comment.DateTimeUser.CreationTime = item.DateTimeUser.CreationTime;
                    }
                    comment.AddItem(item);
                    commentRepository.Save(comment);
                    tx.Commit();
                }
            }

        }
        private Comment GetPreviousCommentId(long box, long user, DateTime time)
        {
            time = time.AddHours(-1);
            var questions = UnitOfWork.CurrentSession.QueryOver<Item>()
                .Where(w => w.DateTimeUser.CreationTime > time)
                .And(w => w.IsDeleted == false)
                .And(w => w.Box.Id == box)
                .And(w => w.Uploader.Id == user)
                .Select(s => s.Comment).Future<Comment>();

            var questions2 = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                .Where(w => w.DateTimeUser.CreationTime > time)
                .And(w => w.Box.Id == box)
                .And(w => w.Owner.Id == user)
                .Select(s => s.Comment).Future<Comment>();

            return questions.Union(questions2)
                .Where(w => w != null)
                //.Where(w => w.Text == null)
                .Where(w => w.FeedType == FeedType.AddedItems)
                .OrderByDescending(o => o.DateTimeUser.CreationTime)
                .FirstOrDefault();
        }


        private void UpdateReputation()
        {
            int i = 0;
            var users = UnitOfWork.CurrentSession.QueryOver<User>().OrderBy(o => o.Id).Asc.Select(s => s.Id)
                .Skip(i * 100).Take(100).List<long>();
            do
            {
                var command = new UpdateReputationCommand(users);
                m_CommandBus.Send(command);
                i++;
                users = UnitOfWork.CurrentSession.QueryOver<User>().OrderBy(o => o.Id).Asc.Select(s => s.Id).Skip(i * 100).Take(100).List<long>();
            } while (users.Count > 0);
        }

        private void UpdateAllUrlsInSystem()
        {
            int i = 0;
            var items = UnitOfWork.CurrentSession.QueryOver<Item>().OrderBy(o => o.Id).Asc
                .Skip(i * 100).Take(100).List();
            do
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in items)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                items = UnitOfWork.CurrentSession.QueryOver<Item>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (items.Count > 0);

            i = 0;
            var boxes = UnitOfWork.CurrentSession.QueryOver<Box>().OrderBy(o => o.Id).Asc
                .Skip(i * 100).Take(100).List();
            do
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in boxes)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                boxes = UnitOfWork.CurrentSession.QueryOver<Box>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (boxes.Count > 0);

            i = 0;
            var quizzes = UnitOfWork.CurrentSession.QueryOver<Quiz>().OrderBy(o => o.Id).Asc
                .Skip(i * 100).Take(100).List();
            do
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in quizzes)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                quizzes = UnitOfWork.CurrentSession.QueryOver<Quiz>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (quizzes.Count > 0);

            i = 0;
            var library = UnitOfWork.CurrentSession.QueryOver<Library>().OrderBy(o => o.Id).Asc
                .Skip(i * 100).Take(100).List();
            do
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in library)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                library = UnitOfWork.CurrentSession.QueryOver<Library>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (library.Count > 0);

            var users = UnitOfWork.CurrentSession.QueryOver<User>().OrderBy(o => o.Id).Asc
               .Skip(i * 100).Take(100).List();
            do
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in users)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                users = UnitOfWork.CurrentSession.QueryOver<User>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (users.Count > 0);
        }

        public bool Dbi(int index)
        {
            UpdateAdminReputation();
            return false;
        }

        private void UpdateAdminReputation()
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                var universitiesIds = conn.Query("SELECT id FROM zbox.university");
                foreach (var universitiesId in universitiesIds)
                {
                    const string sql = @"with topreputation as (
                        select userreputation , ROW_NUMBER() OVER(ORDER BY userreputation DESC) AS Row
                         from zbox.users u
                        where universityid = @id
                        )
                        update zbox.University set AdminScore = (
                        select  top 1 UserReputation  from topreputation 
                        where Row <= (select AdminNoOfPeople from zbox.University where id = @id)
                        order by Row desc)
                        where id = @id;";

                    conn.Execute(sql, new { universitiesId.id });
                }
            }
        }


        public void AddProducts(AddProductsToStoreCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void AddCategories(AddCategoriesCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void AddBanners(AddBannersCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void UpdateReputation(UpdateReputationCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
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


    }
}
