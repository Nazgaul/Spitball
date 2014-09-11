using System;
using System.IO;
using System.Linq;
using Dapper;
using NHibernate;
using NHibernate.Criterion;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Trace;


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
                TraceLog.WriteInfo("Processing universities");

                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var oldUniversities = UnitOfWork.CurrentSession.QueryOver<University2>()
                        .List();
                    foreach (var oldUniversity in oldUniversities)
                    {
                        var newUniversity = new University(oldUniversity.Id,
                            oldUniversity.UniversityName,
                            oldUniversity.Country,
                            oldUniversity.Image,
                            oldUniversity.ImageLarge,
                            "sys")
                        {
                            LetterUrl = oldUniversity.LetterUrl,
                            MailAddress = oldUniversity.MailAddress,
                            NeedCode = oldUniversity.NeedCode,
                            OrgName = oldUniversity.Name,
                            TwitterUrl = oldUniversity.TwitterUrl,
                            TwitterWidgetId = oldUniversity.TwitterWidgetId,
                            WebSiteUrl = oldUniversity.WebSiteUrl,
                            YouTubeUrl = oldUniversity.YouTubeUrl,
                            FacebookUrl = oldUniversity.FacebookUrl

                        };
                        UnitOfWork.CurrentSession.SaveOrUpdate(newUniversity);
                    }
                    tx.Commit();


                }
            }

            InternalDbi();




        }

        private bool InternalDbi()
        {
            var retVal = false;
            using (UnitOfWork.Start())
            {
                TraceLog.WriteInfo("Processing boxes");
                var idGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();
                var dicUniversity = new Dictionary<long, University>();
                var dicDepartment = new Dictionary<KeyValuePair<long, string>, Department>();

                var boxes = UnitOfWork.CurrentSession.QueryOver<AcademicBox>().Where(w => w.IsDeleted == false)
                   .Where(w => w.Department == null)
                    .List();



                foreach (var box in boxes)
                {
                   
                    using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                    {
                        var libraryCollection = box.Library; //.FirstOrDefault();
                        var library = libraryCollection.FirstOrDefault();
                        if (library == null)
                        {
                            continue;
                        }
                        while (library.Parent != null)
                        {
                            library = library.Parent;
                        }

                        University university;
                        if (!dicUniversity.TryGetValue(library.University.Id, out university))
                        {
                            university =
                              UnitOfWork.CurrentSession.QueryOver<University>()
                                  .Where(w => w.Id == library.University.Id)
                                  .SingleOrDefault();
                            if (university == null)
                            {
                                TraceLog.WriteInfo("box id " + box.Id + " don't have university");
                                continue;
                            }
                            dicUniversity.Add(library.University.Id, university);
                        }

                        Department department;
                        if (!dicDepartment.TryGetValue(new KeyValuePair<long, string>(university.Id, library.Name), out department))
                        {
                            department =
                                    UnitOfWork.CurrentSession.QueryOver<Department>()
                                        .Where(w => w.Name == library.Name)
                                        .And(w => w.University == university)
                                        .SingleOrDefault();


                            if (department == null)
                            {
                                department = new Department(idGenerator.GetId(IdGenerator.DepartmentScope), library.Name,
                                    university);
                                UnitOfWork.CurrentSession.Save(department);
                            }
                            dicDepartment.Add(new KeyValuePair<long, string>(university.Id, library.Name), department);
                        }
                        box.UpdateDepartment(department, university);
                        UnitOfWork.CurrentSession.Save(box);
                        retVal = true;
                        tx.Commit();
                    }
                }
            }
            using (UnitOfWork.Start())
            {
                TraceLog.WriteInfo("Processing departments");
                var departments = UnitOfWork.CurrentSession.QueryOver<Department>()
                 .List();
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var department in departments)
                    {
                        var count = UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                             .Where(w => w.Department == department)
                             .And(w => w.IsDeleted == false)
                             .RowCount();
                        department.UpdateNumberOfBoxes(count);
                        UnitOfWork.CurrentSession.Save(department);
                    }
                    tx.Commit();
                }
                

            }
            using (UnitOfWork.Start())
            {
                var universities = UnitOfWork.CurrentSession.QueryOver<University>().List();
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var university in universities)
                    {
                        var count = UnitOfWork.CurrentSession.QueryOver<Department>()
                            .Where(w => w.University == university)
                            .Select(Projections.Sum<Department>(s => s.NoOfBoxes)).SingleOrDefault<int>();
                        university.UpdateNumberOfBoxes(count);
                        UnitOfWork.CurrentSession.Save(university);
                    }
                    tx.Commit();
                }
            }
            using (UnitOfWork.Start())
            {
                TraceLog.WriteInfo("Processing users");
                int index = 0;
                bool needContinue = true;
                while (needContinue)
                {
                    needContinue = false;
                    var users = UnitOfWork.CurrentSession.QueryOver<User>()
                        .Where(w => w.University.Id != w.University2.Id)
                        .Take(100).Skip(100 * index)
                        .List();
                    index++;
                    foreach (var user in users)
                    {
                        needContinue = true;
                        retVal = true;
                        UnitOfWork.CurrentSession.Connection.Execute(
                            "update zbox.users set UniversityId = UniversityId2 where userid = @Id", new { user.Id });
                        var x = UnitOfWork.CurrentSession.Connection.Query(@"
                                    select top(1) b.Department,count(*)  from zbox.UserBoxRel ub 
                join zbox.Box b on ub.BoxId = b.BoxId and b.Discriminator = 2 and IsDeleted = 0
                join zbox.users u on ub.UserId = u.UserId
                where b.university = u.UniversityId
                and u.userid = @id
                group by b.Department
                order by 2 desc", new { id = user.Id });
                        if (x.Count() == 0)
                        {
                            continue;
                        }
                        var department = x.FirstOrDefault().Department;

                        UnitOfWork.CurrentSession.Connection.Execute(
                            "update zbox.users set MainDepartment= CONVERT(bigint, @depart) where userid = @Id",
                            new { user.Id, depart = department });
                    }
                }
            }
            return retVal;
        }

        public bool Dbi(int index)
        {
            bool retVal = InternalDbi();


            //var libraryNodes =
            //          UnitOfWork.CurrentSession.QueryOver<Library>()
            //              .Skip(100 * index)
            //              .Take(100).List();


            //foreach (var node in libraryNodes)
            //{
            //    node.GenerateUrl();
            //    UnitOfWork.CurrentSession.Connection.Execute("update zbox.Library set Url = @Url where libraryid = @Id"
            //        , new { node.Url, node.Id });

            //    retVal = true;
            //}
            using (UnitOfWork.Start())
            {
                var quizes = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                              .Where(w => w.Publish).Skip(100 * index)
                              .Take(100).List();

                foreach (var quiz in quizes)
                {
                    quiz.GenerateUrl();
                    var noOfDiscussion = UnitOfWork.CurrentSession.QueryOver<Discussion>().Where(w => w.Quiz == quiz).RowCount();
                    double? std;
                    int? avg;
                    using (var grid = UnitOfWork.CurrentSession.Connection.QueryMultiple(
                        "select STDEVP(score) from zbox.SolvedQuiz where quizid = @id;" +
                        "select AVG(score) from zbox.SolvedQuiz where quizid = @id;", new { id = quiz.Id }))
                    {
                        std = grid.Read<double?>().FirstOrDefault();
                        avg = grid.Read<int?>().FirstOrDefault();
                    }



                    UnitOfWork.CurrentSession.Connection.Execute(@"update zbox.Quiz
                        set Url = @Url, NumberOfComments = @NCount , [Stdevp] = @std, Average = @average
                        where Id = @Id"
                        , new { quiz.Url, quiz.Id, NCount = noOfDiscussion, std = std, average = avg });

                    retVal = true;
                }

                return retVal;
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
                //UnitOfWork.CurrentSession.Delete("from StoreBanner e");
                //UnitOfWork.CurrentSession.Flush();
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


    }
}
