using Autofac;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Storage;
using Cloudents.Infrastructure.Video;
using Cloudents.Persistence;
using Dapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Api;
using CloudBlockBlob = Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob;


namespace ConsoleApp
{
    internal static class Program
    {
        public static IContainer Container;

        public enum EnvironmentSettings
        {
            Dev,
            Prod
        }

        /*
         *  "PayPal": {
    "ClientId": "AcaET-3DaTqu01QZ0Ad7-5C52pMZ5s4nx59TmbCqdn8gZpfJoM3UPLYCnZmDELZfc-22N_yhmaGEjS3e",
    "ClientSecret": "EPBamUk7w8Ibrld_eNRV18FYp1zqcYBqx8gCpBBUU9_W5h4tBf8_PhqYS9rzyBBjXJhZ0elFoXoLvdk8"
  }
         */
        public static ConfigurationKeys GetSettings(EnvironmentSettings dev)
        {
            switch (dev)
            {
                case EnvironmentSettings.Dev:
                    return new ConfigurationKeys
                    {
                        SiteEndPoint = { SpitballSite = "https://dev.spitball.co", FunctionSite = "https://spitball-function.azureedge.net" },
                        Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                            ConfigurationManager.AppSettings["Redis"],
                            DbConnectionString.DataBaseIntegration.Update),
                        Search = new SearchServiceCredentials(

                            ConfigurationManager.AppSettings["AzureSearchServiceName"],
                            ConfigurationManager.AppSettings["AzureSearchKey"], true),
                        Redis = ConfigurationManager.AppSettings["Redis"],
                        Storage = ConfigurationManager.AppSettings["StorageConnectionString"],
                        ServiceBus = ConfigurationManager.AppSettings["ServiceBus"],
                        PayPal = new PayPalCredentials(
                            "AcaET-3DaTqu01QZ0Ad7-5C52pMZ5s4nx59TmbCqdn8gZpfJoM3UPLYCnZmDELZfc-22N_yhmaGEjS3e",
                            "EPBamUk7w8Ibrld_eNRV18FYp1zqcYBqx8gCpBBUU9_W5h4tBf8_PhqYS9rzyBBjXJhZ0elFoXoLvdk8",
                            true)
                    };
                case EnvironmentSettings.Prod:
                    return new ConfigurationKeys
                    {
                        SiteEndPoint = { SpitballSite = "https://www.spitball.co", FunctionSite = "https://spitball-dev-function.azureedge.net" },
                        Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBoxProd"].ConnectionString,
                            ConfigurationManager.AppSettings["Redis"], DbConnectionString.DataBaseIntegration.None),
                        Search = new SearchServiceCredentials(

                            ConfigurationManager.AppSettings["AzureSearchServiceName"],
                            ConfigurationManager.AppSettings["AzureSearchKey"], false),
                        Redis = ConfigurationManager.AppSettings["Redis"],
                        Storage = ConfigurationManager.AppSettings["StorageConnectionStringProd"],
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
            builder.RegisterAssemblyModules(
                //Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Persistence"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Search"),
                Assembly.Load("Cloudents.Core"));
            builder.RegisterType<MediaServices>().AsSelf().SingleInstance()
                .As<IVideoService>().WithParameter("isDevelop", env == EnvironmentSettings.Dev);
            builder.RegisterType<HttpClient>().AsSelf().SingleInstance();
            //builder.RegisterModule<ModuleFile>();
            builder.RegisterType<MLRecommendation>().AsSelf();



            Container = builder.Build();

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
            var unitOfWork = Container.Resolve<IUnitOfWork>();
            var _commandBus = Container.Resolve<ICommandBus>();
            var command = new PaymentDeclineCommand(Guid.Parse("dce63420-8645-4b0b-842b-ab9900c1e5a6"), 482226);
            await _commandBus.DispatchAsync(command, default);

            //  Console.WriteLine(result);


            //var sessionFuture = await _session.Query<StudyRoomSessionUser>()
            //    .Fetch(f=>f.StudyRoomSession)
            //    .ThenFetch(f=>f.StudyRoom)
            //    .Fetch(f=>f.User)
            //    .Where(w => w.StudyRoomSession.StudyRoom.Tutor.Id == 638 && w.Duration > TimeSpan.FromMinutes(10))
            //    .Select(s => new SessionSaleDto()
            //    {
            //        SessionId = s.StudyRoomSession.Id,
            //        PaymentStatus = s.Receipt != null ? PaymentStatus.Paid :
            //            s.TutorApproveTime != null ? PaymentStatus.Pending:
            //            PaymentStatus.PendingApproval,
            //        Date = s.StudyRoomSession.Created,
            //        Price = s.TotalPrice,
            //        StudentName = s.User.Name,
            //        Duration = s.TutorApproveTime ?? s.Duration.Value,
            //        StudentImage = s.User.ImageName,
            //        StudentId = s.User.Id
            //    })
            //    .ToListAsync();




            //ResourcesMaintenance.DeleteStuffFromJs();
            //await Convert();
            //var result = await s.GetPaymentAsync("4J34525079381873W", default);
            ////var x = await s.QueryAsync(new StudyRoomQuery(Guid.Parse("9f54280c-103e-46a6-8184-aabf00801beb"), 638), default);
        }

     

//        private static async Task ResyncTutorRead()
//        {
//            var session = Container.Resolve<IStatelessSession>();
//            var bus = Container.Resolve<ICommandBus>();
//            var eventHandler = Container.Resolve<IEventPublisher>();

//            var x = await session.CreateSQLQuery(@"
//Select id from sb.tutor t where t.State = 'Ok'").ListAsync();


//            foreach (dynamic z in x)
//            {
//                var e = new SetUniversityEvent(z);
//                await eventHandler.PublishAsync(e, default);
//                //var command = new TeachCourseCommand(z[0], z[1]);
//                //await bus.DispatchAsync(command, default);
//            }
//        }

        private static async Task Convert()
        {

            Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Client.Configuration.Default.AddApiKey("Apikey", "07af4ce1-40eb-4e97-84e0-c02b4974b190");
            Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Client.Configuration.Default.Timeout = 300000; //base on support
            var _convertDocumentApi = new ConvertDocumentApi();
            var id = 104135;
            var storage = Container.Resolve<ICloudStorageProvider>();
            var client = storage.GetBlobClient();
            var container = client.GetContainerReference("spitball-files");
            var dir1 = container.GetDirectoryReference("files");
            var dir2 = dir1.GetDirectoryReference($"{id}");
            var blobs = await dir2.ListBlobsSegmentedAsync(null);
            var blob = (CloudBlockBlob)blobs.Results.FirstOrDefault(f => ((CloudBlockBlob)f).Name.Contains("file-"));


            var sr = await blob.OpenReadAsync();

            // var sr = new FileStream("C:\\Users\\Ram\\Downloads\\xxx\\file-52936bce-e08a-4138-9639-4971c22640ba-142339.pptx", System.IO.FileMode.Open); // System.IO.Stream | Input file to perform the operation on.
            var text2 = await _convertDocumentApi.ConvertDocumentPptxToTxtAsync(sr);
            sr.Seek(0, SeekOrigin.Begin);
            var result = await _convertDocumentApi.ConvertDocumentAutodetectToPngArrayAsync(sr);

            Console.WriteLine("here");
            //var image = new Image<Rgba32>(500, 500);
            //image.Mutate(c=>c.BackgroundColor(Color.Aqua));
            //var ms = new MemoryStream();
            //image.SaveAsJpeg(ms);
            //try
            //{

            //    //var request = new DrawTextRequest();
            //    //byte[] result2 = apiInstance3.EditDrawText(request);
            //    var sw = new Stopwatch();
            //    sw.Start();
            //    var bytes = ms.ToArray();
            //    //apiInstance3.EditDrawText(new DrawTextRequest())
            //    var result = apiInstance3.EditDrawText(
            //        new DrawTextRequest(
            //            BaseImageBytes: bytes,
            //            TextToDraw: new List<DrawTextInstance>()
            //    {

            //        new DrawTextInstance(
            //            "בקרוב תראו תוצאות וציונים שיעלו לכם חיוך על הפנים :) (אפילו אם כרגע זה נראה בלתי אפשרי). בעל ניסיון של 6 שנים!",
            //            FontFamilyName: "Georgia",
            //            FontSize:32,
            //            Color:"black",0,0,500,500
            //            )
            //    }));

            //    File.WriteAllBytes(@"c:\Users\Ram\Downloads\ram1.jpg",result);
            //    //var v = apiInstance.ConvertDocumentDocxToTxt(inputFile);
            //    //inputFile.Seek(0, SeekOrigin.Begin);

            //    //var f = apiInstance.ConvertDocumentAutodetectGetInfo(inputFile);
            //   // var result = apiInstance.ConvertDocumentAutodetectToPngArray(inputFile);

            //    //apiInstance.ConvertDocumentAutodetectGetInfo()
            //    //var result = apiInstance.ConvertDocumentAutodetectToPngArray(inputFile);

            //    // Word DOCX to PDF
            //    //Object result = apiInstance.ConvertDocumentDocxToPdf(inputFile);
            //    sw.Stop();

            //    Debug.WriteLine(result);
            //}
            //catch (Exception e)
            //{
            //    Debug.Print("Exception when calling ConvertDocumentApi.ConvertDocumentDocxToPdf: " + e.Message);
            //}
        }

        private static async Task ResetVideo()
        {
            var bus = Container.Resolve<ICloudStorageProvider>();
            var d = Container.Resolve<DapperRepository>();
            IEnumerable<long> ids; // 49538
            var mediaServices = Container.Resolve<MediaServices>();
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

        //private static async Task UpdateMethod()
        //{
        //    //var c = _container.Resolve<UniversitySearchWrite>();
        //    //await c.CreateOrUpdateAsync(default);


        //    var c2 = Container.Resolve<TutorSearchWrite>();
        //    await c2.CreateOrUpdateAsync(default);

        //    var session = Container.Resolve<ISession>();
        //    foreach (var tutorId in session.Query<Tutor>().Where(w => w.State == ItemState.Ok).Select(s => s.Id).AsEnumerable())
        //    {
        //        var eventHandler = Container.Resolve<IEventHandler<SetUniversityEvent>>();
        //        await eventHandler.HandleAsync(new SetUniversityEvent(tutorId), default);
        //    }

        //    var storageProvider = Container.Resolve<ICloudStorageProvider>();
        //    var blobClient = storageProvider.GetBlobClient();
        //    var container = blobClient.GetContainerReference("spitball");
        //    var directory = container.GetDirectoryReference("AzureSearch");
        //    var blob = directory.GetBlobReference("tutor-version.txt");
        //    await blob.DeleteAsync();


        //    //var c3 = _container.Resolve<QuestionSearchWrite>();
        //    //await c3.CreateOrUpdateAsync(default);
        //}


        private static async Task ReduPreviewProcessingAsync()
        {



            var bus = Container.Resolve<ICloudStorageProvider>();
            var blobClient = bus.GetBlobClient();
            var queueClient = bus.GetQueueClient();


            var container = blobClient.GetContainerReference("spitball-files");
            var dir = container.GetDirectoryReference("files");



            BlobContinuationToken blobToken = null;
            do
            {
                var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None,
                    5000, blobToken,
                    new BlobRequestOptions(),
                    new OperationContext(), default);

                var list = new HashSet<long>();
                Console.WriteLine("Receiving a new batch of blobs");
                foreach (IListBlobItem blob in result.Results)
                {

                    //var fileNameWithoutDirectory = blob.Parent.Uri.MakeRelativeUri(blob.Uri).ToString();
                    var id = long.Parse(blob.Uri.Segments[3].TrimEnd('/'));
                    if (!list.Add(id))
                    {
                        continue;
                    }
                    var fileDir = container.GetDirectoryReference($"files/{id}");

                    var blobs = (await fileDir.ListBlobsSegmentedAsync(false, BlobListingDetails.Metadata, null, null, null, null)).Results.ToList();

                    var fileItem = (CloudBlockBlob)blobs.First(a => a.Uri.AbsoluteUri.Contains("file-"));
                    var extension = Path.GetExtension(fileItem.Name);

                    var blurFiles = blobs.Where(a => a.Uri.AbsoluteUri.Contains("blur-")).ToList();

                    if (blurFiles.Count > 0)
                    {
                        foreach (var listBlobItem in blurFiles)
                        {
                            var blobToDelete = (CloudBlockBlob)listBlobItem;
                            await blobToDelete.DeleteAsync();
                        }
                    }

                    if (!FileTypesExtension.PowerPoint.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                        //var queue = queueClient.GetQueueReference("generate-blob-preview");
                        //var msg = new CloudQueueMessage(id.ToString());
                        //await queue.AddMessageAsync(msg);
                        //Console.WriteLine("Processing regular " + id);
                    }
                    var textBlobItem = blobs.FirstOrDefault(a => a.Uri.AbsoluteUri.Contains("text.txt"));
                    if (textBlobItem != null)
                    {

                        //var textBlob2 = (CloudBlockBlob)textBlobItem;
                        //textBlob2.FetchAttributes();
                        //if (!textBlob2.Metadata.ContainsKey("ProcessTags"))
                        //{
                        //    var queue = queueClient.GetQueueReference("generate-search-preview");
                        //    var msg = new CloudQueueMessage(id.ToString());
                        //    await queue.AddMessageAsync(msg);
                        //    Console.WriteLine("Processing tags " + id);
                        //}
                    }

                    else
                    {
                        var queue = queueClient.GetQueueReference("generate-blob-preview");
                        var msg = new CloudQueueMessage(id.ToString());
                        await queue.AddMessageAsync(msg);
                        Console.WriteLine("Processing regular " + id);
                        continue;
                    }

                    var previewFiles = blobs.Where(a => a.Uri.AbsoluteUri.Contains("preview")).ToList();
                    var textBlob = (CloudBlockBlob)textBlobItem;
                    // textBlob.FetchAttributes();
                    textBlob.Metadata.TryGetValue("PageCount", out var pageCountStr);
                    int.TryParse(pageCountStr, out var pageCount);
                    if (previewFiles.Count == 0 || previewFiles.Count < pageCount)
                    {
                        var queue = queueClient.GetQueueReference("generate-blob-preview");
                        var msg = new CloudQueueMessage(id.ToString());
                        await queue.AddMessageAsync(msg);
                        //             using (var file =
                        //new StreamWriter(@"C:\Users\Ram\Documents\regular.txt", true))
                        //             {

                        //                 file.WriteLine(id);

                        //             }
                        Console.WriteLine("Processing regular " + id);
                        continue;
                    }




                }

                blobToken = result.ContinuationToken;
            } while (blobToken != null);




        }


        private static async Task HadarMethod()
        {
            //var t = new PlaylistUpdates();
            //t.Create();

            var s = new UploadVideo();
            s.Upload();
            
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





        private static async Task DeleteUserAsync(long id)
        {
            var session  = Container.Resolve<ISession>();
            var unitOfWork = Container.Resolve<IUnitOfWork>();

            var user = await session.LoadAsync<User>(id);
            await session.DeleteAsync(user);
            await unitOfWork.CommitAsync(default);
        }

        

        private static async Task<string> CopyBlobFromOldContainerAsync(string blobName, long itemId)
        {
            var key = ConfigurationManager.AppSettings["StorageConnectionStringProd"];
            var productionOldStorageAccount = CloudStorageAccount.Parse(key);
            var oldBlobClient = productionOldStorageAccount.CreateCloudBlobClient();
            var oldContainer = oldBlobClient.GetContainerReference("zboxfiles");



            var keyNew = Container.Resolve<IConfigurationKeys>().Storage;
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
