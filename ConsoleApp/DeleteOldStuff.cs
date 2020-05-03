using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Storage;
using Cloudents.Query;
using NHibernate;
using NHibernate.Linq;

namespace ConsoleApp
{
    public class DeleteOldStuff
    {

        private static IContainer Container = Program.Container;


        public static async Task DoStuff()
        {
            await DeleteDocumentFromNotSupportCountries();
           // await DeleteFlaggedDocument();
            await DeleteOldQuestion();
            await DeleteOldDocuments();
            await DeleteNotUsedCourses();
            await ResyncTutorRead();
        }

//        private static async Task DeleteFlaggedDocument()
//        {
//            var statelessSession = Container.Resolve<IStatelessSession>();
//            int i;
//            do
//            {
//                Console.WriteLine("Turn to delete");
//                var sqlQuery = statelessSession.CreateSQLQuery(@"
//            update top(1000) sb.Document
//            set state = 'Deleted', DeletedOn = '2020-02-16 18:39:13.9970249'
//            where state = 'Ok' and CourseName in ('Small Talk',
//'Reading for Elementary Students',
//'Justice Administration',
//'Life Science',
//'PADP 6920 Public Personnel Administration',
//'PADP 7110 Research Methods of Public Administration',
//'Preschool Math')");

//                i = await sqlQuery.ExecuteUpdateAsync();
//            } while (i > 0);
//        }



        private static async Task ResyncTutorRead()
        {
            var session = Container.Resolve<IStatelessSession>();
            //var bus = Container.Resolve<ICommandBus>();
            var eventHandler = Container.Resolve<IEventPublisher>();

            var x = await session.CreateSQLQuery(@"
Select id from sb.tutor t where t.State = 'Ok'").ListAsync();


            foreach (dynamic z in x)
            {
                var e = new RemoveCourseEvent(z);
                await eventHandler.PublishAsync(e, default);
                //var command = new TeachCourseCommand(z[0], z[1]);
                //await bus.DispatchAsync(command, default);
            }

            var storageProvider = Container.Resolve<ICloudStorageProvider>();
            var blobClient = storageProvider.GetBlobClient();
            var container = blobClient.GetContainerReference("spitball");
            var directory = container.GetDirectoryReference("AzureSearch");
            var blob = directory.GetBlobReference("tutor-version.txt");
            await blob.DeleteAsync();
        }

        private static async Task DeleteNotUsedCourses()
        {
            var statelessSession = Container.Resolve<IStatelessSession>();
            while (true)
            {


                var sqlQuery =
                    statelessSession.CreateSQLQuery(@"select top 100 * from (
Select c.Name from sb.Course c
EXCEPT
Select distinct d.CourseName from sb.Document d
EXCEPT
select distinct q.CourseId from sb.Question q ) t");

                var list = await sqlQuery.ListAsync<string>();
                Console.WriteLine("courses " + list.Count);
                if (list.Count == 0)
                {
                    break;
                }

                foreach (var course in list)
                {
                    const string sql = @"BEGIN TRANSACTION;  
delete from sb.userscourses where courseid = :Id;
delete from sb.course where name = :Id
COMMIT;  ";
                    await statelessSession.CreateSQLQuery(sql).SetString("Id", course).ExecuteUpdateAsync();
                    Console.WriteLine(course);
                    //using (var child = Container.BeginLifetimeScope())
                    //{
                    //    var unitOfWork = child.Resolve<IUnitOfWork>();
                    //    var _session = child.Resolve<ISession>();
                    //    var d = await _session.GetAsync<Course>(course);
                    //    await _session.DeleteAsync(d);
                    //    await unitOfWork.CommitAsync(default);

                    //}
                }

            }



        }

        private static async Task DeleteOldQuestion()
        {
            var statelessSession = Container.Resolve<IStatelessSession>();
            while (true)
            {
                var deletedDocuments = await statelessSession.Query<Question>()
                    .Where(w => w.Status.State == ItemState.Deleted && w.Status.DeletedOn < DateTime.UtcNow
                                    .AddDays(-60))
                    .Take(100)
                    .ToListAsync();
                Console.WriteLine("question " + deletedDocuments.Count);
                if (deletedDocuments.Count == 0)
                {
                    break;

                }
                foreach (var deletedDocument in deletedDocuments)
                {

                    Console.WriteLine(deletedDocument.Id);
                    try
                    {

                        //await statelessSession.Query<Document>().Where(w => w.Id == deletedDocument.Id)

                        //    .DeleteAsync(default);

                        var sqlQuery =
                            statelessSession.CreateSQLQuery("update sb.question set CorrectAnswer_id = null where id =  :Id");
                        sqlQuery.SetInt64("Id", deletedDocument.Id);
                        sqlQuery.ExecuteUpdate();


                        using (var child = Container.BeginLifetimeScope())
                        {
                            var unitOfWork = child.Resolve<IUnitOfWork>();
                            var session = child.Resolve<ISession>();
                            var d = await session.GetAsync<Question>(deletedDocument.Id);

                            foreach (var dAnswer in d.Answers)
                            {
                                await session.DeleteAsync(dAnswer);
                            }

                            await session.DeleteAsync(d);
                            await unitOfWork.CommitAsync(default);

                        }

                        //await statelessSession.Query<QuestionTransaction>().Where(w => w.Question.Id == deletedDocument.Id)
                        //    .DeleteAsync(default);

                        await statelessSession.Query<Answer>().Where(w => w.Question.Id == deletedDocument.Id)
                            .DeleteAsync(default);
                        await statelessSession.Query<Question>().Where(w => w.Id == deletedDocument.Id)
                            .DeleteAsync(default);
                    }
                    catch (Exception e)
                    {

                    }
                    //var d = await _session.GetAsync<Document>(deletedDocument.Id);
                    //await _session.DeleteAsync(d);
                    //await unitOfWork.CommitAsync(default);


                    //   blobProvider.DeleteDirectoryAsync(eventMessage.Document.Id.ToString(), token);
                }
            }
        }

        private static async Task DeleteDocumentFromNotSupportCountries()
        {
            var statelessSession = Container.Resolve<IStatelessSession>();
            int count;
            do
            {


                var v = await statelessSession.Query<Document>().Fetch(f => f.User)
                    .Where(w => Country.CountriesNotSupported.Contains(w.User.Country))
                    .Where(w => w.Status.State == ItemState.Ok)
                    .OrderBy(o => o.Id)
                    .Take(100)
                    
                    .Select(s => s.Id).ToListAsync();
                count = v.Count;
                if (count > 0)
                {
                    await statelessSession.Query<Document>()
                        .Where(w => v.Contains(w.Id))
                        .UpdateBuilder().Set(s => s.Status.State, ItemState.Deleted)
                        .Set(s => s.Status.DeletedOn, DateTime.UtcNow.AddDays(-90))
                        .UpdateAsync(default);
                }
            } while (count > 0);

        }

        private static async Task DeleteOldDocuments()
        {
            var statelessSession = Container.Resolve<IStatelessSession>();

            var blobProvider = Container.Resolve<IDocumentDirectoryBlobProvider>();

            while (true)
            {


                var deletedDocuments = await statelessSession.Query<Document>()
                    .Where(w => w.Status.State == ItemState.Deleted && w.Status.DeletedOn < DateTime.UtcNow
                    .AddDays(-60))
                    .Take(100)
                    .ToListAsync();
                Console.WriteLine("document " + deletedDocuments.Count);
                if (deletedDocuments.Count == 0)
                {
                    break;

                }
                foreach (var deletedDocument in deletedDocuments)
                {

                    Console.WriteLine(deletedDocument.Id);
                    var v = await blobProvider.FilesInDirectoryAsync("", deletedDocument.Id.ToString(), default);
                    if (v.Any())
                    {
                        await blobProvider.DeleteDirectoryAsync(deletedDocument.Id.ToString(), default);
                    }

                    var sqlQuery =
                        statelessSession.CreateSQLQuery("delete from sb.DocumentsTags where documentid = :Id");
                    sqlQuery.SetInt64("Id", deletedDocument.Id);
                    sqlQuery.ExecuteUpdate();


                    //var sqlQuery2 =
                    //    statelessSession.CreateSQLQuery("delete from  sb.Vote where documentid = :Id");
                    //sqlQuery2.SetInt64("Id", deletedDocument.Id);
                    //sqlQuery2.ExecuteUpdate();


                    //var sqlQuery3 =
                    //    statelessSession.CreateSQLQuery("delete from  sb.DocumentDownload where documentid = :Id");
                    //sqlQuery3.SetInt64("Id", deletedDocument.Id);
                    //sqlQuery3.ExecuteUpdate();


                    try
                    {
                        await statelessSession.Query<Document>().Where(w => w.Id == deletedDocument.Id)
                            .DeleteAsync(default);
                    }
                    catch
                    {
                        using (var child = Container.BeginLifetimeScope())
                        {
                            var unitOfWork = child.Resolve<IUnitOfWork>();
                            var session = child.Resolve<ISession>();
                            var d = await session.GetAsync<Document>(deletedDocument.Id);
                            await session.DeleteAsync(d);
                            await unitOfWork.CommitAsync(default);

                        }
                        await statelessSession.Query<Document>().Where(w => w.Id == deletedDocument.Id)
                            .DeleteAsync(default);
                    }
                    //var d = await _session.GetAsync<Document>(deletedDocument.Id);
                    //await _session.DeleteAsync(d);
                    //await unitOfWork.CommitAsync(default);


                    //   blobProvider.DeleteDirectoryAsync(eventMessage.Document.Id.ToString(), token);
                }
            }
        }
    }
}
