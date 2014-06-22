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
                        //box.CalculateMembers();
                        //box.UpdateItemCount();
                        //box.UpdateQnACount(boxRepository.QnACount(box.Id));
                        box.GenerateUrl();
                        //box.UpdateBoxPicutureUrl()
                        //var picture = box.Picture;
                        //if (picture == null)
                        //{
                        //    box.RemovePicture();

                        //}
                        //else
                        //{
                        //    box.AddPicture(picture, blobProvider.GetThumbnailUrl(picture));
                        //}
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
                        , new { Url = file.Url, Id = file.Id });
                    //using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                    //{

                    retVal = true;

                    ////UnitOfWork.CurrentSession.CreateSQLQuery("update zbox.Item set Url = :Url where itemId = :Id")
                    ////    .SetString("Url",file.Url).SetInt64("Id", file.Id)
                    ////    .ExecuteUpdate();
                    //UnitOfWork.CurrentSession.Save(file);
                    //tx.Commit();
                }


                //using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                //{
                //    var links =
                //        UnitOfWork.CurrentSession.QueryOver<Link>()
                //            .Where(w => w.IsDeleted == false && w.Url == null)
                //            .Take(100).List();
                //    foreach (var link in links)
                //    {
                //        retVal = true;
                //        link.GenerateUrl();
                //        UnitOfWork.CurrentSession.Save(link);
                //    }
                //    tx.Commit();
                //}
                //using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                //{
                //    var users = UnitOfWork.CurrentSession.QueryOver<University>().Where(w => w.Url == null).List();
                //    foreach (var user in users)
                //    {
                //        user.GenerateUrl();

                //    }
                //    tx.Commit();

                //}
                //var retVal = false;
                //using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                //{
                //    var users = UnitOfWork.CurrentSession.QueryOver<User>()
                //        .Where(w => w.Url == null).Take(paging).List();
                //    foreach (var user in users)
                //    {
                //        retVal = true;
                //        user.GenerateUrl();
                //        UnitOfWork.CurrentSession.Save(user);
                //    }

                //    tx.Commit();

                //}
                //return retVal;
                return retVal;
            }
        }


    }
}
