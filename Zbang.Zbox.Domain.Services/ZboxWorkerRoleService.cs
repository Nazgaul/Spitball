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

                    //var files = UnitOfWork.CurrentSession.QueryOver<File>()
                    //    .Where(w => w.IsDeleted == false)
                    //    .Where(w => w.Size == -1).List();

                    //foreach (var file in files)
                    //{
                    //    var blob = blobProvider.GetFile(file.ItemContentUrl);
                    //    blob.FetchAttributes();
                    //    file.Size = blob.Properties.Length;
                    //    var extension = Path.GetExtension(file.ItemContentUrl);
                    //    if (string.IsNullOrEmpty(extension))
                    //    {
                    //        extension = Path.GetExtension(file.Name);
                    //        blobProvider.RenameBlob(file.ItemContentUrl, file.ItemContentUrl + extension);
                    //        file.ItemContentUrl = file.ItemContentUrl + extension;
                    //    }
                    //    UnitOfWork.CurrentSession.Save(file);
                    //}
                    //tx.Commit();


                }
            }

        }

        public bool Dbi(int index)
        {
            var retVal = false;
            using (UnitOfWork.Start())
            {

                var idGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();
                var boxes = UnitOfWork.CurrentSession.QueryOver<AcademicBox>().Where(w => w.IsDeleted == false)
                   .Where(w => w.Department == null)
                   .Where(w => w.Owner.Id == 920)
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
                        var department =
                            UnitOfWork.CurrentSession.QueryOver<Department>()
                                .Where(w => w.Name == library.Name)
                                .SingleOrDefault();
                        var university =
                            UnitOfWork.CurrentSession.QueryOver<University>()
                                .Where(w => w.Id == library.University.Id)
                                .SingleOrDefault();
                        if (department == null)
                        {
                            department = new Department(idGenerator.GetId(IdGenerator.DepartmentScope), library.Name,
                                university);
                            UnitOfWork.CurrentSession.Save(department);
                        }
                        box.UpdateDepartment(department, university);
                        UnitOfWork.CurrentSession.Save(box);

                        tx.Commit();
                    }
                }




                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var oldUniversities = UnitOfWork.CurrentSession.QueryOver<University2>()
                        .Skip(100 * index)
                        .Take(100).List();
                    foreach (var oldUniversity in oldUniversities)
                    {
                        retVal = true;
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

                        };
                        UnitOfWork.CurrentSession.SaveOrUpdate(newUniversity);
                    }
                    tx.Commit();


                }

                var users = UnitOfWork.CurrentSession.QueryOver<User>()
                        .Where(w => w.University != null)
                        .Skip(100 * index)
                        .Take(100).List();
                foreach (var user in users)
                {
                    retVal = true;
                    UnitOfWork.CurrentSession.Connection.Execute("update zbox.users set UniversityId = UniversityId2 where userid = @Id", new { user.Id });
                }



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
