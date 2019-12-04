using Autofac;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Storage;
using Cloudents.Infrastructure.Video;
using Cloudents.Persistence;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Search.Tutor;
using Dapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Courses;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Query.Feed;
using Cloudents.Query.Chat;
using Cloudents.Query.Documents;
using Cloudents.Query.HomePage;
using Newtonsoft.Json;
using CloudBlockBlob = Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ConsoleApp
{
    internal static class Program
    {
        public static IContainer _container;

        public enum EnvironmentSettings
        {
            Dev,
            Prod
        }


        public static ConfigurationKeys GetSettings(EnvironmentSettings dev)
        {
            switch (dev)
            {
                case EnvironmentSettings.Dev:
                    return new ConfigurationKeys
                    {
                        SiteEndPoint = { SpitballSite = "https://dev.spitball.co" },
                        Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                            ConfigurationManager.AppSettings["Redis"], DbConnectionString.DataBaseIntegration.None),
                        MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                        Search = new SearchServiceCredentials(

                            ConfigurationManager.AppSettings["AzureSearchServiceName"],
                            ConfigurationManager.AppSettings["AzureSearchKey"], true),
                        Redis = ConfigurationManager.AppSettings["Redis"],
                        Storage = ConfigurationManager.AppSettings["StorageConnectionString"],
                        LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200),
                        BlockChainNetwork = "http://localhost:8545",
                        ServiceBus = ConfigurationManager.AppSettings["ServiceBus"],
                    };
                case EnvironmentSettings.Prod:
                    return new ConfigurationKeys
                    {
                        SiteEndPoint = { SpitballSite = "https://www.spitball.co" },
                        Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBoxProd"].ConnectionString,
                            ConfigurationManager.AppSettings["Redis"], DbConnectionString.DataBaseIntegration.None),
                        MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                        Search = new SearchServiceCredentials(

                            ConfigurationManager.AppSettings["AzureSearchServiceName"],
                            ConfigurationManager.AppSettings["AzureSearchKey"], false),
                        Redis = ConfigurationManager.AppSettings["Redis"],
                        Storage = ConfigurationManager.AppSettings["StorageConnectionStringProd"],
                        LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200),
                        BlockChainNetwork = "http://localhost:8545",
                        ServiceBus = ConfigurationManager.AppSettings["ServiceBus"],
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(dev), dev, null);
            }


        }

        static async Task Main()
        {

            var builder = new ContainerBuilder();

            var env = EnvironmentSettings.Dev;


            builder.Register(_ => GetSettings(env)).As<IConfigurationKeys>();
            builder.RegisterAssemblyModules(Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Persistence"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Search"),
                Assembly.Load("Cloudents.Core"));
            builder.RegisterType<MediaServices>().AsSelf().SingleInstance()
                .As<IVideoService>().WithParameter("isDevelop", env == EnvironmentSettings.Dev);
            builder.RegisterType<HttpClient>().AsSelf().SingleInstance();
            builder.RegisterModule<ModuleFile>();

            _container = builder.Build();

            if (Environment.UserName == "Ram")
            {
                await RamMethod();
            }
            else
            {

                await HadarMethod();
            }


            Console.WriteLine("done");
            Console.Read();




        }



        private static async Task RamMethod()
        {
            var x = new RegistrationEmail("ram@cloudents.com", "https://www.spitball.co", new CultureInfo("en"));
            var json = JsonConvert.SerializeObject(x, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            //var queryBus = _container.Resolve<IQueryBus>();
            //var query = new SimilarDocumentsQuery(242949);
            //var result = await queryBus.QueryAsync(query, default);

        }
        private static async Task ResyncTutorRead()
        {
            var session = _container.Resolve<IStatelessSession>();
            var bus = _container.Resolve<ICommandBus>();
            var eventHandler = _container.Resolve<IEventPublisher>();

            var x = await session.CreateSQLQuery(@"
Select id from sb.tutor t where t.State = 'Ok'").ListAsync();


            foreach (dynamic z in x)
            {
                var e = new SetUniversityEvent(z);
                await eventHandler.PublishAsync(e, default);
                //var command = new TeachCourseCommand(z[0], z[1]);
                //await bus.DispatchAsync(command, default);
            }
        }


        private static async Task ResetVideo()
        {
            var bus = _container.Resolve<ICloudStorageProvider>();
            var d = _container.Resolve<DapperRepository>();
            IEnumerable<long> ids; // 49538
            var mediaServices = _container.Resolve<MediaServices>();
            var queueClient = bus.GetQueueClient();
            using (var con = d.OpenConnection())
            {
                var sql = "Select id from sb.document where documenttype = 'video' and id = 49704";
                ids = await con.QueryAsync<long>(sql);
            }

            foreach (var id in ids)
            {
                Console.WriteLine("Process id " + id);
                await mediaServices.DeleteAssetAsync(id, AssetType.Thumbnail, CancellationToken.None);
                await mediaServices.DeleteAssetAsync(id, AssetType.Short, CancellationToken.None);
                await mediaServices.DeleteAssetAsync(id, AssetType.Long, CancellationToken.None);
                var queue = queueClient.GetQueueReference("generate-blob-preview");
                await queue.AddMessageAsync(new CloudQueueMessage(id.ToString()), null, TimeSpan.FromSeconds(30), null, null);
            }
        }

        private static async Task UpdateMethod()
        {
            //var c = _container.Resolve<UniversitySearchWrite>();
            //await c.CreateOrUpdateAsync(default);


            var c2 = _container.Resolve<TutorSearchWrite>();
            await c2.CreateOrUpdateAsync(default);

            var session = _container.Resolve<ISession>();
            foreach (var tutorId in session.Query<Tutor>().Where(w => w.State == ItemState.Ok).Select(s => s.Id).AsEnumerable())
            {
                var eventHandler = _container.Resolve<IEventHandler<SetUniversityEvent>>();
                await eventHandler.HandleAsync(new SetUniversityEvent(tutorId), default);
            }

            var storageProvider = _container.Resolve<ICloudStorageProvider>();
            var blobClient = storageProvider.GetBlobClient();
            var container = blobClient.GetContainerReference("spitball");
            var directory = container.GetDirectoryReference("AzureSearch");
            var blob = directory.GetBlobReference("tutor-version.txt");
            await blob.DeleteAsync();


            //var c3 = _container.Resolve<QuestionSearchWrite>();
            //await c3.CreateOrUpdateAsync(default);
        }


        //private static async Task ReduPreviewProcessingAsync()
        //{



        //    var bus = _container.Resolve<ICloudStorageProvider>();
        //    var blobClient = bus.GetBlobClient();
        //    var queueClient = bus.GetQueueClient();


        //    var container = blobClient.GetContainerReference("spitball-files");
        //    var dir = container.GetDirectoryReference("files/4782");



        //    BlobContinuationToken blobToken = null;
        //    do
        //    {
        //        var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 
        //            5000, blobToken,
        //            new BlobRequestOptions(),
        //            new OperationContext(), default);

        //        var list = new HashSet<long>();
        //        Console.WriteLine("Receiving a new batch of blobs");
        //        foreach (IListBlobItem blob in result.Results)
        //        {

        //            //var fileNameWithoutDirectory = blob.Parent.Uri.MakeRelativeUri(blob.Uri).ToString();
        //            var id = long.Parse(blob.Uri.Segments[3].TrimEnd('/'));
        //            if (!list.Add(id))
        //            {
        //                continue;
        //            }
        //            var fileDir = container.GetDirectoryReference($"files/{id}");
        //            var blobs = fileDir.ListBlobs(false,BlobListingDetails.Metadata).ToList();

        //            var textBlobItem = blobs.FirstOrDefault(a => a.Uri.AbsoluteUri.Contains("text.txt"));
        //            if (textBlobItem != null)
        //            {

        //                //var textBlob2 = (CloudBlockBlob)textBlobItem;
        //                //textBlob2.FetchAttributes();
        //                //if (!textBlob2.Metadata.ContainsKey("ProcessTags"))
        //                //{
        //                //    var queue = queueClient.GetQueueReference("generate-search-preview");
        //                //    var msg = new CloudQueueMessage(id.ToString());
        //                //    await queue.AddMessageAsync(msg);
        //                //    Console.WriteLine("Processing tags " + id);
        //                //}
        //            }

        //            else
        //            {
        //                var queue = queueClient.GetQueueReference("generate-blob-preview");
        //                var msg = new CloudQueueMessage(id.ToString());
        //                await queue.AddMessageAsync(msg);
        //                Console.WriteLine("Processing regular " + id);
        //                continue;
        //            }

        //            var previewFiles = blobs.Where(a => a.Uri.AbsoluteUri.Contains("preview")).ToList();
        //            var textBlob = (CloudBlockBlob)textBlobItem;
        //           // textBlob.FetchAttributes();
        //            textBlob.Metadata.TryGetValue("PageCount", out var pageCountStr);
        //            int.TryParse(pageCountStr, out var pageCount);
        //            if (previewFiles.Count == 0 || previewFiles.Count < pageCount)
        //            {
        //                var queue = queueClient.GetQueueReference("generate-blob-preview");
        //                var msg = new CloudQueueMessage(id.ToString());
        //                await queue.AddMessageAsync(msg);
        //                //             using (var file =
        //                //new StreamWriter(@"C:\Users\Ram\Documents\regular.txt", true))
        //                //             {

        //                //                 file.WriteLine(id);

        //                //             }
        //                Console.WriteLine("Processing regular " + id);
        //                continue;
        //            }

        //            var duplicatePreview = previewFiles.Cast<CloudBlockBlob>()
        //                .GroupBy(g => new { g.Properties.ContentMD5, g.Properties.Length }).Where(g => g.Count() == Math.Max(previewFiles.Count, 2));
        //            if (duplicatePreview.Any())
        //            {
        //                foreach (var listBlobItem in previewFiles)
        //                {
        //                    if (listBlobItem is CloudBlockBlob p)
        //                    {
        //                        p.Delete();
        //                    }

        //                }

        //                foreach (var listBlobItem in blobs.Where(a => a.Uri.AbsoluteUri.Contains("blur")))
        //                {
        //                    if (listBlobItem is CloudBlockBlob p)
        //                    {
        //                        p.Delete();
        //                    }
        //                }
        //                var queue = queueClient.GetQueueReference("generate-blob-preview");
        //                var msg = new CloudQueueMessage(id.ToString());
        //                await queue.AddMessageAsync(msg);
        //                Console.WriteLine("Duplicate preview " + id);
        //                continue;
        //            }
        //            var blobBlurCount = blobs.Count(a => a.Uri.AbsoluteUri.Contains("blur"));
        //            if (blobBlurCount == 0 || blobBlurCount < Math.Min(pageCount, 10))
        //            {
        //                var queue = queueClient.GetQueueReference("generate-blob-preview-blur");
        //                var msg = new CloudQueueMessage(id.ToString());
        //                await queue.AddMessageAsync(msg);

        //                //               using (var file =
        //                //new StreamWriter(@"C:\Users\Ram\Documents\blur.txt", true))
        //                //               {

        //                //                   file.WriteLine(id);

        //                //               }
        //                Console.WriteLine("Processing blur " + id);
        //            }

        //        }

        //        blobToken = result.ContinuationToken;
        //    } while (blobToken != null);




        //}







        //private static async Task UniversitiesWithSimilarNames()
        //{
        //    var d = _container.Resolve<DapperRepository>();

        //    var universities = await d.WithConnectionAsync(async f =>
        //    {
        //        return await f.QueryAsync<(Guid, string, string)>(
        //            @"Select Id,[Name], Country from sb.University order by Name");
        //    }, default);
        //    StringBuilder sb = new StringBuilder();
        //    string filePath = @"C:\Users\Charlie even\university.csv";
        //    //List<(string,string)> output = new List<(string, string)>();
        //    foreach (var universityName in universities)
        //    {
        //        var res = FindSimilarStringsUniversity(universityName, universities.ToList());

        //        foreach (var university in res)
        //        {
        //            File.AppendAllLines(filePath, new List<string>() { $@"{university.Item1}, {university.Item2}, {university.Item3}, {university.Item4}, {university.Item5}, {university.Item6}" });
        //        }
        //    }
        //}

        //private static async Task CoursesWithSimilarNames()
        //{
        //    var d = _container.Resolve<DapperRepository>();

        //    var courses = await d.WithConnectionAsync(async f =>
        //    {
        //        return await f.QueryAsync<string>(
        //            @"Select [Name] from sb.Course where name like N'%[א-ת]%' order by Name");
        //    }, default);
        //    StringBuilder sb = new StringBuilder();
        //    string filePath = @"C:\Users\Charlie even\course.csv";
        //    //List<(string,string)> output = new List<(string, string)>();
        //    foreach (var courseName in courses)
        //    {
        //        var res = FindSimilarStringsCourse(courseName, courses.ToList());

        //        foreach (var tuple in res)
        //        {
        //            File.AppendAllLines(filePath, new List<string>() { $@"{tuple.Item1}, {tuple.Item2}" });
        //        }
        //    }
        //}
        //private static async Task AddToExtra()
        //{

        //    var d = _container.Resolve<DapperRepository>();

        //    var res = await d.WithConnectionAsync(async f =>
        //    {

        //        return await f.QueryAsync<string>(
        //        @"select Name
        //            from sb.University 
        //            where name like N'%ל%'");

        //    }, default);

        //    foreach (var item in res)
        //    {
        //        var nameArr = item.Split(' ');
        //        string resName = string.Empty;
        //        foreach (var name in nameArr)
        //        {
        //            if (!string.IsNullOrEmpty(name))
        //            {
        //                if (name.Substring(0, 1).Equals("ל", StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    resName += string.Concat(name.Substring(1), ' ');
        //                }
        //                else
        //                {
        //                    resName += string.Concat(name, ' ');
        //                }
        //            }
        //        }
        //        resName = resName.Substring(0, resName.Length - 1);
        //        if (!resName.Equals(item))
        //        {
        //            //update
        //            var resU = await d.WithConnectionAsync(async f =>
        //            {

        //                return await f.ExecuteAsync(
        //                @"update sb.University
        //                    set Extra = CONCAT(Extra, ' ', @name)
        //                    where name = @oldName", new { name = resName, oldName = item });
        //            }, default);
        //        }
        //    }
        //}

        //private static async Task PopulateUsersImageName()
        //{
        //    var uof = _container.Resolve<IUnitOfWork>();
        //    var session = _container.Resolve<ISession>();
        //    var blobProvider = _container.Resolve<IUserDirectoryBlobProvider>();
        //    var repository = _container.Resolve<IRepository<BaseUser>>();

        //    var keyNew = _container.Resolve<IConfigurationKeys>().Storage;
        //    var storageAccount = CloudStorageAccount.Parse(keyNew);
        //    var blobClient = storageAccount.CreateCloudBlobClient();
        //    var container = blobClient.GetContainerReference("spitball-user");


        //    var userIds = await session.Query<User>().Where(w => w.Image != null).Select(s => s.Id).ToListAsync();


        //    foreach (var userId in userIds)
        //    {
        //        var user = await repository.LoadAsync(userId, default);
        //        var dir = container.GetDirectoryReference($"profile/{userId.ToString()}");

        //        var img = dir.ListBlobs().LastOrDefault();
        //        var name = img.StorageUri.PrimaryUri.AbsolutePath.Split('/').LastOrDefault();
        //        if (!string.IsNullOrEmpty(name))
        //        {
        //            user.UpdateUserImageName(name);
        //            await repository.UpdateAsync(user, default);
        //        }
        //    }
        //    await uof.CommitAsync(default);

        //}

        private static async Task HadarMethod()
        {
            var queryBus = _container.Resolve<IQueryBus>();

            var query = new CalendarEventsQuery(161755L, new DateTime(2019, 9, 16, 17, 0, 0), new DateTime(2019, 9, 22, 17, 0, 0));
            var t = await queryBus.QueryAsync(query, default);
            //await PopulateUsersImageName();
            //await commandBus.DispatchAsync(command2, default);
            //var deleteCommand = new SessionReconnectedCommand(id);
            //await commandBus.DispatchAsync(deleteCommand, default);
            //var repo = _container.Resolve<ITutorRepository>();
            //var test = await repo.GetTutorsByCourseAsync("organic chemistry כימיה אורגנית", 638, "IL", default);
            //ResourcesMaintenance.GetOrphanedResources();
            //var queryBus = _container.Resolve<IQueryBus>();
            //var query = new TutorListQuery(159039, "IL",0);
            //var test = await queryBus.QueryAsync(query, default);
            //foreach (var item in test)
            //{
            //    Console.WriteLine(item.UserId);
            //}
            //}
            //var provider = _container.Resolve<IMondayProvider>();
            //await provider.UpdateTextRecordAsync(244705486, "text9", "רופין", default);
            //await provider.UpdateTextRecordAsync(244705486, "_____________1",
            //     string.Concat("ggg", "econ"), default);
            //await provider.UpdateTextRecordAsync(244705486, "________________________", "ttt", default);
            //await provider.CreateRecordAsync("api test", default);
            //var queryBus = _container.Resolve<IQueryBus>();
            //  var query = new QuestionAggregateQuery(638L, 0);
            //            var test = await queryBus.QueryAsync(query, default);

            /* var command = new AddTutorReviewCommand("string", (float)0.5, 160347, 160347);
             await commandBus.DispatchAsync(command, default);*/

            //You can register the QueryFactory in the IoC container

            //var user = db.Query("sb.User").Where("Id", (long)160347).First();
            //var commandBus = _container.Resolve<ICommandBus>();
            //var command = new AddTutorReviewCommand("string", (float)0.5, 160347, 160347);
            //await commandBus.DispatchAsync(command, default);

            //await addToExtra();
            //await FunctionsExtensions.DeleteCourses(_container);
            //await TransferDocuments();
            //var _queryBus = _container.Resolve<IQueryBus>();
            // await FixStorageAsync();
            /* var commandBus = _container.Resolve<ICommandBus>();*/
            //await ReNameFiles();
            //await CoursesWithSimilarNames();
            //         160259, new List<string> { }, "econ 101");
            // await commandBus.DispatchAsync(command, token);
            //await FunctionsExtensions.MergeCourses(_container);

            //var d = _container.Resolve<DapperRepository>();


            /* var res = await d.WithConnectionAsync(async f =>
             {

                 return await f.QueryAsync<(string, long)>(
                 @"declare @tmp table (CourseName nvarchar(max), DocumentId bigint, rn bigint)
                     insert into @tmp
                     select CourseName, Id, ROW_NUMBER () over (Partition By CourseName Order By Id asc) as rn
                     from sb.Document
                     where [state] = 'ok' and CourseName in (select * from ##CourseTbl)
                     select CourseName, DocumentId from @tmp where rn < 4");

             }, default);
             StringBuilder sb = new StringBuilder();
             ////string delimiter = ",";

             ////int rowNumber = 1;

             string filePath = @"C:\Users\Charlie even\DocumentsOfCoursesToDelete.csv";
             //List<(string,string)> output = new List<(string, string)>();
             foreach (var r in res)
             {

                 var b62 = new Base62(r.Item2);
                 string str = "https://www.spitball.co/document/" + b62.ToString();
                 File.AppendAllText(filePath, str + "," + r.Item1 + "," + r.Item2.ToString() + Environment.NewLine);



             }*/
        }






        private static string GetShareAccessUri(string blobname,
                int validityPeriodInMinutes,
                CloudBlobDirectory dir)
        {
            var toDateTime = DateTime.Now.AddMinutes(validityPeriodInMinutes);

            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = null,
                SharedAccessExpiryTime = new DateTimeOffset(toDateTime)
            };

            var blob = dir.GetBlockBlobReference(blobname);
            var sas = blob.GetSharedAccessSignature(policy);
            return blob.Uri.AbsoluteUri + sas;
        }

        //private static async Task FixStorageAsync()
        //{
        //    var key = ConfigurationManager.AppSettings["StorageConnectionStringProd"];
        //    var productionOldStorageAccount = CloudStorageAccount.Parse(key);
        //    var blobClient = productionOldStorageAccount.CreateCloudBlobClient();
        //    var container = blobClient.GetContainerReference("spitball-files");
        //    var dir = container.GetDirectoryReference("files");


        //    BlobContinuationToken blobToken = null;
        //    do
        //    {
        //        var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 5000, blobToken,
        //            new BlobRequestOptions(),
        //            new OperationContext(), default);

        //        var list = new HashSet<string>();
        //        Console.WriteLine("Receiving a new batch of blobs");
        //        foreach (IListBlobItem blob in result.Results)
        //        {
        //            if (blob.Uri.Segments.Contains("220643"))
        //            {
        //                Console.WriteLine(blob.Uri);
        //            }

        //            Console.WriteLine(blob.Uri);

        //            var blobToCheckStr = blob.Uri.Segments[4];
        //            var test = blob.Uri.Segments.Length;
        //            if (test > 5)
        //            {
        //                var dirToRemove = blob.Parent;
        //                string dirToRemoveStr = dirToRemove.Uri.ToString().Split('/')[dirToRemove.Uri.ToString().Split('/').Length - 2];
        //                var blobToMoveList = dirToRemove.ListBlobs();


        //                string blobToMoveStr = blobToMoveList.First().Uri.ToString().Split('/')[blobToMoveList.First().Uri.ToString().Split('/').Length - 1];
        //                var blobToMove = dirToRemove.GetBlockBlobReference(blobToMoveStr);

        //                var name = blob.Uri.ToString().Split('/')[blob.Uri.ToString().Split('/').Length - 2].Replace('/', '-');
        //                var ect = Path.GetExtension(blob.Uri.ToString().Split('/')[blob.Uri.ToString().Split('/').Length - 1]).TrimStart('.');
        //                var id = blob.Uri.Segments[blob.Uri.Segments.Length - 3].Trim('/');

        //                CloudBlockBlob blobDestination = dir.GetBlockBlobReference(
        //                    $"{id}/{name}.{ect}");


        //                var sharedAccessUri = GetShareAccessUri(blobToMoveStr, 360, dirToRemove);

        //                var blobUri = new Uri(sharedAccessUri);

        //                await blobDestination.StartCopyAsync(blobUri);
        //                while (blobDestination.CopyState.Status != CopyStatus.Success)
        //                {
        //                    Console.WriteLine(blobDestination.CopyState.Status);
        //                    await Task.Delay(TimeSpan.FromSeconds(1));
        //                    await blobDestination.ExistsAsync();
        //                }


        //                blobToMove.DeleteIfExists();
        //            }
        //        }

        //        blobToken = result.ContinuationToken;
        //    } while (blobToken != null);

        //}


        //private static async Task ReNameFiles()
        //{
        //    var key = ConfigurationManager.AppSettings["StorageConnectionStringProd"];
        //    var productionOldStorageAccount = CloudStorageAccount.Parse(key);
        //    var blobClient = productionOldStorageAccount.CreateCloudBlobClient();


        //    var container = blobClient.GetContainerReference("spitball-files");
        //    var dir = container.GetDirectoryReference("files");


        //    BlobContinuationToken blobToken = null;
        //    do
        //    {
        //        var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 5000, blobToken,
        //            new BlobRequestOptions(),
        //            new OperationContext(), default);

        //        var list = new HashSet<long>();
        //        Console.WriteLine("Receiving a new batch of blobs");
        //        foreach (IListBlobItem blob in result.Results)
        //        {

        //            //var fileNameWithoutDirectory = blob.Parent.Uri.MakeRelativeUri(blob.Uri).ToString();
        //            var id = long.Parse(blob.Uri.Segments[3].TrimEnd('/'));
        //            if (!list.Add(id))
        //            {
        //                continue;
        //            }
        //            var fileDir = container.GetDirectoryReference($"files/{id}");
        //            var blobs = fileDir.ListBlobs().ToList();
        //            var textBlobItem = blobs.FirstOrDefault(a => a.Uri.AbsoluteUri.Contains("file-"));

        //            Regex rgx = new Regex(@"[^\x00-\x7F]+|\s+");

        //            var t = rgx.Replace(textBlobItem.Uri.Segments.Last().Replace("%20", " "), string.Empty);
        //            //t = ".pdf";
        //            if (Path.GetFileNameWithoutExtension(t.Split('-').Last()) == string.Empty)
        //            {
        //                t = RandomString(3) + t;
        //            }

        //            if ($"/spitball-files/files/{id}/" + t != textBlobItem.Uri.LocalPath.Replace(" ", ""))
        //            {
        //                var dirToRemove = blob.Parent;
        //                string dirToRemoveStr = dirToRemove.Uri.ToString().Split('/')[dirToRemove.Uri.ToString().Split('/').Length - 2];
        //                var blobToMoveList = dirToRemove.ListBlobs();


        //                string blobToMoveStr = blobToMoveList.First().Uri.ToString().Split('/')[blobToMoveList.First().Uri.ToString().Split('/').Length - 1];
        //                var blobToMove = dirToRemove.GetBlockBlobReference(blobToMoveStr);

        //                var name = blob.Uri.ToString().Split('/')[blob.Uri.ToString().Split('/').Length - 2].Replace('/', '-');
        //                var ect = Path.GetExtension(blob.Uri.ToString().Split('/')[blob.Uri.ToString().Split('/').Length - 1]).TrimStart('.');
        //                var newName = rgx.Replace(name, string.Empty);
        //                CloudBlockBlob blobDestination = dir.GetBlockBlobReference(
        //                    $"{id}/{newName}.{ect}");

        //                var sharedAccessUri = GetShareAccessUri(blobToMoveStr, 360, dirToRemove);

        //                var blobUri = new Uri(sharedAccessUri);
        //                await blobDestination.StartCopyAsync(blobUri);
        //                while (blobDestination.CopyState.Status != CopyStatus.Success)
        //                {
        //                    Console.WriteLine(blobDestination.CopyState.Status);
        //                    await Task.Delay(TimeSpan.FromSeconds(1));
        //                    await blobDestination.ExistsAsync();
        //                }


        //                blobToMove.DeleteIfExists();
        //            }

        //        }

        //        blobToken = result.ContinuationToken;
        //    } while (blobToken != null);

        //}

        private static Random _random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        //       public static async Task TransferDocuments()
        //       {
        //           var d = _container.Resolve<IDapperRepository>();


        //           /* var key = ConfigurationManager.AppSettings["StorageConnectionStringProd"];
        //            var productionOldStorageAccount = CloudStorageAccount.Parse(key);
        //            var oldBlobClient = productionOldStorageAccount.CreateCloudBlobClient();
        //            var oldContainer = oldBlobClient.GetContainerReference("zboxfiles");*/



        //           var keyNew = _container.Resolve<IConfigurationKeys>().Storage;
        //           var storageAccount = CloudStorageAccount.Parse(keyNew);
        //           var blobClient = storageAccount.CreateCloudBlobClient();
        //           var container = blobClient.GetContainerReference("spitball-files/files");

        //           //CloudBlobContainer directoryToPutFiles = container.Get .GetDirectoryReference("./files");

        //           Dictionary<int, string> docType = new Dictionary<int, string>
        //                   {
        //                       {1, "Exam"},
        //                       {2, "Exam"},
        //                       {7, "Exam"},
        //                       {8, "Exam"},
        //                       {9, "Lecture"},
        //                       {10, "Lecture"},
        //                       {5, "Textbook"}
        //                   };


        //           var supportedFiles = ExcelProcessor.Extensions
        //               .Union(ImageProcessor.Extensions)
        //               .Union(PdfProcessor.Extensions)
        //               .Union(PowerPoint2007Processor.Extensions)
        //               .Union(TextProcessor.Extensions)
        //               .Union(TiffProcessor.Extensions)
        //               .Union(WordProcessor.Extensions).ToList();

        //           var cacheUsers = new ConcurrentDictionary<string, long?>();
        //           List<dynamic> z;
        //           long itemId = 15053;
        //           do
        //           {
        //               z = await d.WithConnectionAsync(async f =>
        //               {
        //                   return (await f.QueryAsync(
        //                       @"select top 1000 I.ItemId, I.BlobName, I.Name,  B.BoxName, ZU.Email,ZUni.UniversityName, ZUNI.Country,  B.ProfessorName, 
        //       ISNULL(I.DocType,0) as DocType, I.NumberOfViews + I.NumberOfDownloads as [Views], I.CreationTime,
        //       			            STRING_AGG((T.Name), ',') as Tags
        //                               FROM [Zbox].[Item] I
        //                               join zbox.Box B
        //       	                        on I.BoxId = B.BoxId 
        //								--and b.discriminator in (2,3)
        //								and b.PrivacySetting = 3

        //                               join Zbox.Users ZU
        //       	                        on I.UserId = ZU.UserId
        //                             	 join zbox.Users uTemp on uTemp.UserId = b.OwnerId
        // join zbox.University ZUNI on uTemp.UniversityId = ZUNI.Id and ZUNI.Id = 920
        //and ZUNI.id not In ( 170460,790) and ZUNI.country = 'IL'
        //       						left join zbox.ItemTag IT
        //       							on IT.ItemId = I.ItemId
        //       						left join zbox.Tag T
        //       							on IT.TagId = T.Id and len(T.Name) >= 4
        //                               where I.Discriminator = 'File'
        //       						and i.itemid > @itemId
        //       	                        and I.IsDeleted = 0 
        //       							and I.ItemId not in (select D.OldId from sb.Document D where I.ItemId = D.OldId)

        //                               group by I.ItemId, I.BlobName, I.Name,  B.BoxName, ZU.Email,ZUni.UniversityName,ZUNI.Country, B.ProfessorName,
        //       						 ISNULL(I.DocType,0),I.NumberOfViews + I.NumberOfDownloads, I.CreationTime
        //       						 order by i.itemid
        //                       ", new { itemId })).ToList();
        //               }, default);

        //               //if (z.Count() == 0)
        //               //{
        //               //    return;
        //               //}


        //               using (var child = _container.BeginLifetimeScope())
        //               {

        //                   var commandBus = child.Resolve<ICommandBus>();
        //                   var session = child.Resolve<IStatelessSession>();


        //                   foreach (var pair in z)
        //                   {
        //                       itemId = pair.ItemId;
        //                       Console.WriteLine($"processing {itemId}");

        //                       string extension = Path.GetExtension(pair.BlobName);

        //                       if (!supportedFiles.Contains(extension, StringComparer.OrdinalIgnoreCase))
        //                       {
        //                           Console.ForegroundColor = ConsoleColor.Red;
        //                           Console.WriteLine($"{pair.ItemId} not blob support");
        //                           Console.ResetColor();
        //                           //itemsAlreadyProcessed.Add(pair.ItemId);
        //                           continue;
        //                       }

        //                       string country = pair.Country, email = pair.Email;

        //                       var userId = cacheUsers.GetOrAdd(email, x =>
        //                       {
        //                           long? id = GetUserId(x, country);
        //                           return id;
        //                       });
        //                       if (userId == null)
        //                       {
        //                           Console.ForegroundColor = ConsoleColor.Red;
        //                           Console.WriteLine($"{pair.ItemId} doesn't have userid to assign country {country}");
        //                           Console.ResetColor();
        //                           continue;
        //                       }

        //                       Guid? uniId = GetUniversityId(pair.UniversityName, country);
        //                       if (uniId == null)
        //                       {

        //                           Console.ForegroundColor = ConsoleColor.Red;
        //                           Console.WriteLine($"{pair.ItemId} doesn't have uniId to assign");
        //                           Console.ResetColor();
        //                           continue;
        //                       }

        //                       var newBlobName = await CopyBlobFromOldContainerAsync(pair.BlobName, itemId);


        //                       string[] words = null;
        //                       if (pair.Tags != null)
        //                       {
        //                           words = pair.Tags.Split(',');
        //                       }

        //                       var type = "None";

        //                       if (docType.ContainsKey(pair.DocType))
        //                       {
        //                           docType.TryGetValue(pair.DocType, out type);
        //                       }

        //                       string courseName = pair.BoxName;
        //                       while (courseName.Length < Course.MinLength)
        //                       {
        //                           courseName += "-";
        //                       }


        //                       string itemName = pair.Name;
        //                       CreateDocumentCommand command =
        //                           CreateDocumentCommand.DbiOnly(newBlobName,
        //                               itemName.Substring(0, Math.Min(150, itemName.Length)),
        //                               type, courseName, words?.Where(Tag.ValidateTag),
        //                               userId.Value, pair.ProfessorName, uniId.Value);

        //                       await commandBus.DispatchAsync(command, default);

        //                       int views = pair.Views;
        //                       itemId = pair.ItemId;
        //                       DateTime updateTime = pair.CreationTime;

        //                       var doc = session.Query<Document>().Where(w => w.Id == command.Id)
        //                           .UpdateBuilder()
        //                           .Set(x => x.Views, x => views)
        //                           .Set(x => x.OldId, x => itemId)
        //                           .Set(x => x.TimeStamp.UpdateTime, x => updateTime)
        //                           .Update();
        //                       await Task.Delay(TimeSpan.FromSeconds(0.5));
        //                   }

        //               }
        //           } while (z.Count > 0);

        //           //await TransferDocumants();
        //       }

        private static async Task<string> CopyBlobFromOldContainerAsync(string blobName, long itemId)
        {
            var key = ConfigurationManager.AppSettings["StorageConnectionStringProd"];
            var productionOldStorageAccount = CloudStorageAccount.Parse(key);
            var oldBlobClient = productionOldStorageAccount.CreateCloudBlobClient();
            var oldContainer = oldBlobClient.GetContainerReference("zboxfiles");



            var keyNew = _container.Resolve<IConfigurationKeys>().Storage;
            var storageAccount = CloudStorageAccount.Parse(keyNew);
            var blobClient = storageAccount.CreateCloudBlobClient();


            var container = blobClient.GetContainerReference("spitball-files");
            var filesBaseDir = container.GetDirectoryReference("files");
            CloudBlockBlob blobDestination =
                filesBaseDir.GetBlockBlobReference(
                    $"file-{Path.GetFileNameWithoutExtension(blobName)}-{itemId}.{Path.GetExtension(blobName).TrimStart('.')}");

            var sharedAccessUri = GetShareAccessUri(blobName, 360, oldContainer);
            var blobUri = new Uri(sharedAccessUri);

            await blobDestination.StartCopyAsync(blobUri).ConfigureAwait(false);
            while (blobDestination.CopyState.Status != CopyStatus.Success)
            {
                Console.WriteLine(blobDestination.CopyState.Status);
                await Task.Delay(TimeSpan.FromSeconds(1));
                await blobDestination.ExistsAsync();
            }

            return blobDestination.Uri.Segments.Last();
        }

        //        private static long? GetUserId(string email, string country)
        //        {
        //            var d = _container.Resolve<DapperRepository>();
        //            return d.WithConnection<long?>(connection =>
        //            {
        //                const string sql = @"select id from sb.[user] where email = @email;
        //select top 1 id from sb.[user] where Fictive = 1 and country = @country order by newid()";
        //                using (var multi = connection.QueryMultiple(sql, new { email, country = country }))
        //                {
        //                    var val = multi.ReadFirstOrDefault<long?>();
        //                    if (val.HasValue)
        //                    {
        //                        return val.Value;
        //                    }

        //                    val = multi.ReadFirstOrDefault<long?>();
        //                    if (val.HasValue)
        //                    {
        //                        return val.Value;
        //                    }

        //                    return null;
        //                }



        //            });
        //        }
        //private static Guid? GetUniversityId(string name, string country)
        //{
        //    //select id from sb.University where Name = N'המכללה האקדמית בית ברל' and country = 'IL'
        //    var d = _container.Resolve<DapperRepository>();
        //    return d.WithConnection<Guid?>(connection =>
        //    {
        //        const string sql = @"select id from sb.University where Name = @Name and country = @country";
        //        using (var multi = connection.QueryMultiple(sql, new { Name = name, country = country }))
        //        {
        //            var val = multi.ReadFirstOrDefault<Guid?>();
        //            if (val.HasValue)
        //            {
        //                return val.Value;
        //            }

        //            return null;
        //        }
        //    });
        //}

        private static string GetShareAccessUri(string blobname,
            int validityPeriodInMinutes,
            CloudBlobContainer container)
        {
            var toDateTime = DateTime.Now.AddMinutes(validityPeriodInMinutes);

            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = null,
                SharedAccessExpiryTime = new DateTimeOffset(toDateTime)
            };

            var blob = container.GetBlockBlobReference(blobname);
            var sas = blob.GetSharedAccessSignature(policy);
            return blob.Uri.AbsoluteUri + sas;
        }

    }
}
