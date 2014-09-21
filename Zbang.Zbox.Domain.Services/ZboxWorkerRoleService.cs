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
            bool retVal = false;

            using (UnitOfWork.Start())
            {
                using (var tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
                        .Where(w => w.IsDeleted == false).Skip(100*index)
                        .Take(100).List();

                    foreach (var box in boxes)
                    {
                        //quiz.GenerateUrl();
                        box.UpdateItemCount();
                        UnitOfWork.CurrentSession.Save(box);
                        retVal = true;
                    }
                    tx.Commit();
                }
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
                //UnitOfWork.CurrentSession.Delete("from StoreBanner e");
                //UnitOfWork.CurrentSession.Flush();
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


    }
}
