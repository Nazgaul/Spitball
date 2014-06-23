using Dapper;
using NHibernate;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;


namespace Zbang.Zbox.Domain.Services
{
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

        public bool Dbi()
        {
            var retVal = false;
            using (UnitOfWork.Start())
            {
                // var boxRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IBoxRepository>();
                //var blobProvider = Infrastructure.Ioc.IocFactory.Unity.Resolve<IBlobProvider>();
                //members count
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {

                    //box members
                    var boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
                                         .Where(w => w.IsDeleted == false && w.Url == null).Take(100)
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
                              .Where(w => w.IsDeleted == false && w.Url == null)
                              .Take(100).List();


                foreach (var file in files)
                {
                    file.GenerateUrl();
                    UnitOfWork.CurrentSession.Connection.Execute("update zbox.Item set Url = @Url where itemId = @Id"
                        , new { file.Url, file.Id });

                    retVal = true;
                }

                var quizes = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                              .Where(w =>  w.Url == null && w.Publish)
                              .Take(100).List();

                foreach (var quiz in quizes)
                {
                    quiz.GenerateUrl();
                    UnitOfWork.CurrentSession.Connection.Execute("update zbox.Quiz set Url = @Url where Id = @Id"
                        , new { quiz.Url, quiz.Id });

                    retVal = true;
                }
                return retVal;
            }
        }


    }
}
