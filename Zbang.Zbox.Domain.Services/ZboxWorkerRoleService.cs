using NHibernate;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Storage;


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

        public bool Dbi(int paging)
        {
            using (UnitOfWork.Start())
            {

                var boxRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IBoxRepository>();
                var blobProvider = Infrastructure.Ioc.IocFactory.Unity.Resolve<IBlobProvider>();
                //members count
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    //box members
                    var boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
                                         .Where(w => w.IsDeleted == false)
                                         .List();
                    foreach (var box in boxes)
                    {

                        box.CalculateMembers();
                        box.UpdateItemCount();
                        box.UpdateQnACount(boxRepository.QnACount(box.Id));
                        //box.UpdateBoxPicutureUrl()
                        var picture = box.Picture;
                        if (picture == null)
                        {
                            box.RemovePicture();

                        }
                        else
                        {
                            box.AddPicture(picture, blobProvider.GetThumbnailUrl(picture));
                        }
                        UnitOfWork.CurrentSession.Save(box);
                    }
                    tx.Commit();
                }

                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var files =
                        UnitOfWork.CurrentSession.QueryOver<File>()
                            .Where(w => w.IsDeleted == false)
                            .List();
                    foreach (var file in files)
                    {
                        var url = blobProvider.GetThumbnailUrl(file.ThumbnailBlobName);
                        file.UpdateThumbnail(file.ThumbnailBlobName, url);
                    }
                    tx.Commit();
                }
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var links =
                        UnitOfWork.CurrentSession.QueryOver<Link>()
                            .Where(w => w.IsDeleted == false)
                            .List();
                    foreach (var link in links)
                    {
                        var url = blobProvider.GetThumbnailLinkUrl();
                        link.UpdateThumbnail(link.ThumbnailBlobName, url);
                    }
                    tx.Commit();
                }
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var users = UnitOfWork.CurrentSession.QueryOver<University>().Where(w => w.Url == null).List();
                    foreach (var user in users)
                    {
                        user.GenerateUrl();

                    }
                    tx.Commit();

                }
                var retVal = false;
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var users = UnitOfWork.CurrentSession.QueryOver<User>()
                        .Where(w => w.Url == null).Take(paging).List();
                    foreach (var user in users)
                    {
                        retVal = true;
                        user.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(user);
                    }

                    tx.Commit();

                }
                return retVal;

            }
        }


    }
}
