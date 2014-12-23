using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using NHibernate;
using NHibernate.Proxy;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Domain.DataAccess;
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
                UpdateIdOfQuestion();
                AddItemsToFeedDbi();
            }
        }

        public void UpdateIdOfQuestion()
        {
            var questions = UnitOfWork.CurrentSession.QueryOver<Comment>()
                .Where(w => w.FeedType == FeedType.CreatedCourse)
                .And(w => w.Box.Id == 5339L)
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
                 .And(w => w.Box.Id == 5062L)
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

        private void InternalDbi()
        {

            using (UnitOfWork.Start())
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {

                    var files = UnitOfWork.CurrentSession.QueryOver<File>().Where(w => w.Url == null)
                        .And(w => w.IsDeleted == false)
                        .List();
                    foreach (var file in files)
                    {
                        file.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(file);
                    }
                    tx.Commit();
                }

            }
            //        bool retVal = true;
            //        var dic = new Dictionary<University, Department>();
            //        while (retVal)
            //        {
            //            var users = UnitOfWork.CurrentSession.QueryOver<User>()
            //                .Where(w => w.University2 != null)
            //                .And(w => w.Department == null)
            //                .Take(2000)
            //                .List();
            //            retVal = false;
            //            using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
            //            {
            //                foreach (var user in users)
            //                {
            //                    Department department;
            //                    if (!dic.TryGetValue(user.University2, out department))
            //                    {

            //                        department =
            //                            UnitOfWork.CurrentSession.QueryOver<Department>()
            //                                .Where(w => w.University == user.University2)
            //                                .Take(1).SingleOrDefault();
            //                        dic.Add(user.University2, department);
            //                    }
            //                    if (department == null)
            //                    {
            //                        continue;
            //                    }
            //                    retVal = true;
            //                    user.Department = department;
            //                    UnitOfWork.CurrentSession.Save(user);

            //                }
            //                tx.Commit();
            //            }
            //        }

            //    }


            //    using (UnitOfWork.Start())
            //    {
            //        var universities = UnitOfWork.CurrentSession.QueryOver<University>().List();
            //        using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
            //        {
            //            foreach (var university in universities)
            //            {
            //                var count = UnitOfWork.CurrentSession.QueryOver<Department>()
            //                    .Where(w => w.University == university)
            //                    .Select(Projections.Sum<Department>(s => s.NoOfBoxes)).SingleOrDefault<int>();
            //                university.UpdateNumberOfBoxes(count);
            //                UnitOfWork.CurrentSession.Save(university);
            //            }
            //            tx.Commit();
            //        }
            //    }
        }

        public bool Dbi(int index)
        {
            bool retVal = false;

            using (UnitOfWork.Start())
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var universities = UnitOfWork.CurrentSession.QueryOver<University>().List();
                    var universityRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IUniversityRepository>();
                    foreach (var university in universities)
                    {
                        university.UpdateNumberOfBoxes(universityRepository.GetNumberOfBoxes(university));
                        UnitOfWork.CurrentSession.Save(university);
                    }
                    tx.Commit();
                }
                //        TraceLog.WriteInfo("Processing departments");
                var departments = UnitOfWork.CurrentSession.QueryOver<Library>().List();

                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var department in departments)
                    {

                        var x = UnitOfWork.CurrentSession.Get<Library>(department.Id);
                        x.UpdateNumberOfBoxes();
                        while (x != null)
                        {
                            UnitOfWork.CurrentSession.Save(x);
                            x = x.Parent;
                        }

                    }
                    tx.Commit();
                }
                UnitOfWork.CurrentSession.Connection.Execute("ReputationAdmin",
                    commandType: System.Data.CommandType.StoredProcedure);



            }
            return retVal;
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


    }
}
