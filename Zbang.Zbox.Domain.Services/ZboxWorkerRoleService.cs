using Dapper;
using NHibernate;
using NHibernate.Criterion;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;


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

        public bool Dbi(int index)
        {
            var retVal = false;
            using (UnitOfWork.Start())
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {

                    //box members
                    var boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
                                         .Where(w => w.IsDeleted == false).Skip(100 * index).Take(100)
                                         .List();
                    foreach (var box in boxes)
                    {
                        retVal = true;
                        box.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(box);
                    }

                    tx.Commit();
                }
                var files =
                          UnitOfWork.CurrentSession.QueryOver<Item>()
                              .Where(w => w.IsDeleted == false).Skip(100 * index)
                              .Take(100).List();


                foreach (var file in files)
                {
                    file.GenerateUrl();
                    UnitOfWork.CurrentSession.Connection.Execute("update zbox.Item set Url = @Url where itemId = @Id"
                        , new { file.Url, file.Id });

                    retVal = true;
                }

                var quizes = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                              .Where(w => w.Publish).Skip(100 * index)
                              .Take(100).List();

                foreach (var quiz in quizes)
                {
                    quiz.GenerateUrl();
                    UnitOfWork.CurrentSession.Connection.Execute("update zbox.Quiz set Url = @Url where Id = @Id"
                        , new { quiz.Url, quiz.Id });

                    retVal = true;
                }
                return retVal;
                // var boxRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IBoxRepository>();
                //var blobProvider = Infrastructure.Ioc.IocFactory.Unity.Resolve<IBlobProvider>();
                //members count
                //using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                //{

                //    //box members
                //    var boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
                //                         .Where(w => w.IsDeleted == false ).Where(Restrictions.On<Box>(x=>x.Name).IsLike("#",MatchMode.Anywhere))
                //                         .Skip(100 * index).Take(100)
                //                         .List();
                //    foreach (var box in boxes)
                //    {
                //        retVal = true;
                //        box.GenerateUrl();
                //        UnitOfWork.CurrentSession.Save(box);
                //    }

                //    tx.Commit();
                //}
                //var files =
                //          UnitOfWork.CurrentSession.QueryOver<Item>()
                //              .Where(w => w.IsDeleted == false)
                //              .Where(Restrictions.On<Item>(x => x.Name).IsLike("#", MatchMode.Anywhere))
                //              .Skip(100 * index)
                //              .Take(100).List();


                //foreach (var file in files)
                //{
                //    file.GenerateUrl();
                //    UnitOfWork.CurrentSession.Connection.Execute("update zbox.Item set Url = @Url where itemId = @Id"
                //        , new { file.Url, file.Id });

                //    retVal = true;
                //}

                //var quizes = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                //              .Where(w => w.Publish)
                //              .Where(Restrictions.On<Quiz>(x => x.Name).IsLike("#", MatchMode.Anywhere))
                //              .Skip(100 * index)
                //              .Take(100).List();

                //foreach (var quiz in quizes)
                //{
                //    quiz.GenerateUrl();
                //    UnitOfWork.CurrentSession.Connection.Execute("update zbox.Quiz set Url = @Url where Id = @Id"
                //        , new { quiz.Url, quiz.Id });

                //    retVal = true;
                //}
                //return retVal;
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

        public void AddCatories(AddCategoriesCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


    }
}
