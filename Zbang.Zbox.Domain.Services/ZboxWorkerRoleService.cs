using System.Linq;
using Dapper;
using NHibernate;
using NHibernate.Criterion;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
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
            InternalDbi();




        }

        private bool InternalDbi()
        {
            var retVal = false;
            
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
