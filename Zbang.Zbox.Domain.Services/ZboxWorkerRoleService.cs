using System.IO;
using System.Threading.Tasks;
using Dapper;
using NHibernate;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Storage;


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
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var blobProvider = Infrastructure.Ioc.IocFactory.Unity.Resolve<IBlobProvider>();
                    var files = UnitOfWork.CurrentSession.QueryOver<File>()
                        .Where(w => w.IsDeleted == false)
                        .Where(w => w.Size == -1).List();

                    foreach (var file in files)
                    {
                        var blob = blobProvider.GetFile(file.ItemContentUrl);
                        blob.FetchAttributes();
                        file.Size = blob.Properties.Length;
                        var extension = Path.GetExtension(file.ItemContentUrl);
                        if (string.IsNullOrEmpty(extension))
                        {
                            extension = Path.GetExtension(file.Name);
                            blobProvider.RenameBlob(file.ItemContentUrl, file.ItemContentUrl + extension);
                            file.ItemContentUrl = file.ItemContentUrl + extension;
                        }
                        UnitOfWork.CurrentSession.Save(file);
                    }
                    tx.Commit();
                }
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
                    var noOfDiscussion = UnitOfWork.CurrentSession.QueryOver<Discussion>().Where(w => w.Quiz == quiz).RowCount();
                    UnitOfWork.CurrentSession.Connection.Execute("update zbox.Quiz set Url = @Url, NumberOfComments = @NCount where Id = @Id"
                        , new { quiz.Url, quiz.Id, NCount = noOfDiscussion });

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
                UnitOfWork.CurrentSession.Delete("from StoreBanner e");
                UnitOfWork.CurrentSession.Flush();
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


    }
}
