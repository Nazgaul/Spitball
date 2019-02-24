using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Storage;
using Cloudents.Query;
using Dapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using SimMetricsMetricUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ConsoleApp
{
    internal static class Program
    {
        private static IContainer _container;




        static async Task Main()
        {

            var builder = new ContainerBuilder();
            var keys = new ConfigurationKeys("https://www.spitball.co")
            {
                Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBoxProd"].ConnectionString,
                    ConfigurationManager.AppSettings["Redis"]),
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"], true),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionStringProd"],
                LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200),
                BlockChainNetwork = "http://localhost:8545",
                ServiceBus = ConfigurationManager.AppSettings["ServiceBus"],
                PayPal = new PayPalCredentials(
                    "AcaET-3DaTqu01QZ0Ad7-5C52pMZ5s4nx59TmbCqdn8gZpfJoM3UPLYCnZmDELZfc-22N_yhmaGEjS3e",
                    "EPBamUk7w8Ibrld_eNRV18FYp1zqcYBqx8gCpBBUU9_W5h4tBf8_PhqYS9rzyBBjXJhZ0elFoXoLvdk8",
                    true)
            };


            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterAssemblyModules(Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Persistance"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Search"),
                Assembly.Load("Cloudents.Core"));

            builder.RegisterModule<ModuleFile>();


            _container = builder.Build();

            if (Environment.UserName == "Ram")
            {
                await HadarMethod();
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
            var textTranslator = _container.Resolve<ITextTranslator>();
            //var result2 = await textTranslator.TranslateAsync("hello text", "he", default);

            var dapper = _container.Resolve<DapperRepository>();
            var blobStorage = _container.Resolve<IBlobProvider<DocumentContainer>>();
            var textAnalysis = _container.Resolve<ITextAnalysis>();
            var textClassifier = _container.Resolve<ITextClassifier>();
            var englishCulture = new CultureInfo("en");

            var list = new List<string>();

            IEnumerable<(long, string)> result;
            using (var db = dapper.OpenConnection())
            {
                result = await db.QueryAsync<(long, string)>(@"Select id,name from sb.Document
                where CourseName = N'ליניארית'
                and state = 'Ok'");
            }

            var FileName = @"C:\Users\Ram\Documents\keyphrases-Liniar2.txt";
            foreach (var documentId in result.Take(50))
            {
                Console.WriteLine($"Processing document {documentId.Item1}");
                var text = await blobStorage.DownloadTextAsync("text.txt", documentId.Item1.ToString(), default);
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                var v = await textAnalysis.DetectLanguageAsync(text, default);
                if (!v.Equals(englishCulture))
                {
                    text = await textTranslator.TranslateAsync(text, "en", default);
                }

                var keyPhrases = await textClassifier.KeyPhraseAsync(text, default);

                if (!v.Equals(englishCulture))
                {
                    text = string.Join(" , ", keyPhrases);
                    text = await textTranslator.TranslateAsync(text, v.TwoLetterISOLanguageName.ToLowerInvariant(), default);

                    keyPhrases = text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                }

                using (var fileStream = new FileStream(FileName, FileMode.Append))
                using (var sw = new StreamWriter(fileStream))
                {
                    await sw.WriteLineAsync($"Document Id: {documentId.Item1} documentName {documentId.Item2}");
                    await sw.WriteLineAsync($"===============================================================");
                    foreach (var keyPhrase in keyPhrases)
                    {
                        list.Add(keyPhrase);
                        await sw.WriteLineAsync($"{keyPhrase}");
                    }

                    await sw.WriteLineAsync($"---------------------------------------------------------------");
                    await sw.WriteLineAsync();

                }


            }
            using (var fileStream = new FileStream(FileName, FileMode.Append))
            using (var sw = new StreamWriter(fileStream))
            {
                await sw.WriteLineAsync($"For course data");

                var numberGroups = list.GroupBy(i => i);
                foreach (var grp in numberGroups.OrderByDescending(o => o.Count()))
                {
                    await sw.WriteLineAsync($"Key phrase: {grp.Key} count { grp.Count()}");
                }
            }

            Console.WriteLine("done");

        }



        private static async Task ReduWordProcessing()
        {



            var _bus = _container.Resolve<ICloudStorageProvider>();
            var blobClient = _bus.GetBlobClient();
            var queueClient = _bus.GetQueueClient();


            var container = blobClient.GetContainerReference("spitball-files");
            var dir = container.GetDirectoryReference("files");
            var queue = queueClient.GetQueueReference("generate-blob-preview");


            BlobContinuationToken blobToken = null;
            do
            {
                var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 5000, blobToken,
                    new BlobRequestOptions(),
                    new OperationContext(), default);

                var list = new HashSet<string>();
                Console.WriteLine("Receiving a new batch of blobs");
                foreach (IListBlobItem blob in result.Results)
                {

                    var fileNameWithoutDirectory = blob.Parent.Uri.MakeRelativeUri(blob.Uri).ToString();
                    var id = blob.Uri.Segments[3].TrimEnd('/');
                    if (!list.Add(id))
                    {
                        continue;
                    }
                    var fileDir = container.GetDirectoryReference($"files/{id}");
                    var blobs = fileDir.ListBlobs().ToList();

                    if (!blobs.Any(a => a.Uri.AbsoluteUri.Contains("preview")))
                    {
                        var msg = new CloudQueueMessage(id);
                        await queue.AddMessageAsync(msg);
                        using (var file =
           new StreamWriter(@"C:\Users\Ram\Documents\regular.txt", true))
                        {

                            file.WriteLine(id);

                        }
                        Console.WriteLine("Processing regular " + id);
                        continue;
                    }
                    if (!blobs.Any(a => a.Uri.AbsoluteUri.Contains("blur")))
                    {
                        var queue2 = queueClient.GetQueueReference("generate-blob-preview-blur");
                        var msg = new CloudQueueMessage(id);
                        await queue2.AddMessageAsync(msg);

                        using (var file =
         new StreamWriter(@"C:\Users\Ram\Documents\blur.txt", true))
                        {

                            file.WriteLine(id);

                        }
                        Console.WriteLine("Processing blur " + id);
                    }

                }

                blobToken = result.ContinuationToken;
            } while (blobToken != null);




        }

        private static readonly Regex SpaceReg = new Regex(@"\s+", RegexOptions.Compiled);


        private static List<(Guid, string, string, Guid, string, string)> FindSimilarStringsUniversity(
            (Guid, string, string) t, List<(Guid, string, string)> pageTexts)
        {
            var jaroWinkler = new JaroWinkler();
            List<(Guid, string, string, Guid, string, string)> res =
                    new List<(Guid, string, string, Guid, string, string)>();
            foreach (var r in pageTexts.Where(w => w != t))
            {
                var result = jaroWinkler.GetSimilarity(r.Item2, t.Item2);

                if (result > 0.9)
                { res.Add((t.Item1, t.Item2, t.Item3, r.Item1, r.Item2, r.Item3)); }
            }
            return res;

        }

        private static List<(string, string)> FindSimilarStringsCourse(
           string t, List<string> pageTexts)
        {
            var jaroWinkler = new JaroWinkler();
            List<(string, string)> res =
                    new List<(string, string)>();
            foreach (var r in pageTexts.Where(w => w != t))
            {
                var result = jaroWinkler.GetSimilarity(r, t);

                if (result > 0.95)
                { res.Add((t, r)); }
            }
            return res;
            /*if (result.Any(w => w > 0.95))
            {
                res.Add(t, );
            }
            return res;*/
        }

        private static async Task UniversitiesWithSimilarNames()
        {
            var d = _container.Resolve<DapperRepository>();

            var universities = await d.WithConnectionAsync(async f =>
            {
                return await f.QueryAsync<(Guid, string, string)>(
                    @"Select Id,[Name], Country from sb.University order by Name");
            }, default);
            StringBuilder sb = new StringBuilder();
            string filePath = @"C:\Users\Charlie even\university.csv";
            //List<(string,string)> output = new List<(string, string)>();
            foreach (var universityName in universities)
            {
                var res = FindSimilarStringsUniversity(universityName, universities.ToList());

                foreach (var university in res)
                {
                    File.AppendAllLines(filePath, new List<string>() { $@"{university.Item1}, {university.Item2}, {university.Item3}, {university.Item4}, {university.Item5}, {university.Item6}" });
                }
            }
        }

        private static async Task CoursesWithSimilarNames()
        {
            var d = _container.Resolve<DapperRepository>();

            var courses = await d.WithConnectionAsync(async f =>
            {
                return await f.QueryAsync<string>(
                    @"Select [Name] from sb.Course where name like N'%[א-ת]%' order by Name");
            }, default);
            StringBuilder sb = new StringBuilder();
            string filePath = @"C:\Users\Charlie even\course.csv";
            //List<(string,string)> output = new List<(string, string)>();
            foreach (var courseName in courses)
            {
                var res = FindSimilarStringsCourse(courseName, courses.ToList());

                foreach (var tuple in res)
                {
                    File.AppendAllLines(filePath, new List<string>() { $@"{tuple.Item1}, {tuple.Item2}" });
                }
            }
        }

        private static async Task HadarMethod()
        {
            await FixStorageAsync();
           /* var commandBus = _container.Resolve<ICommandBus>();

            var command = new CreateQuestionCommand(QuestionSubject.Accounting, "EmailTest", 5,
                    160259, new List<string> { }, "econ 101");
            await commandBus.DispatchAsync(command, token);*/
            //await CoursesWithSimilarNames();
            //await FunctionsExtensions.MergeCourses(_container);

            /*var d = _container.Resolve<DapperRepository>();


            var res = await d.WithConnectionAsync(async f =>
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

                /* int length = output.Count();

                 for (int index = 0; index < length; index++)
                 {
                     sb.AppendLine(string.Join(delimiter, output[index]));
                 }*/

            //}

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

        private static async Task FixStorageAsync()
        {
            var key = ConfigurationManager.AppSettings["StorageConnectionStringProd"];
            var productionOldStorageAccount = CloudStorageAccount.Parse(key);
            var blobClient = productionOldStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("spitball-files");
            var dir = container.GetDirectoryReference("files");


            BlobContinuationToken blobToken = null;
            do
            {
                var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 5000, blobToken,
                    new BlobRequestOptions(),
                    new OperationContext(), default);

                var list = new HashSet<string>();
                Console.WriteLine("Receiving a new batch of blobs");
                foreach (IListBlobItem blob in result.Results)
                {
                    if (blob.Uri.Segments.Contains("220643"))
                    {
                        Console.WriteLine(blob.Uri);
                    }
                    
                    Console.WriteLine(blob.Uri);
                    
                    var blobToCheckStr = blob.Uri.Segments[4];
                    var test = blob.Uri.Segments.Length;
                    if (test > 5)
                    {
                        var dirToRemove = blob.Parent;
                        string dirToRemoveStr = dirToRemove.Uri.ToString().Split('/')[dirToRemove.Uri.ToString().Split('/').Length - 2];
                        var blobToMoveList = dirToRemove.ListBlobs();


                        string blobToMoveStr = blobToMoveList.First().Uri.ToString().Split('/')[blobToMoveList.First().Uri.ToString().Split('/').Length - 1];
                        var blobToMove = dirToRemove.GetBlockBlobReference(blobToMoveStr);

                        var name = blob.Uri.ToString().Split('/')[blob.Uri.ToString().Split('/').Length - 2].Replace('/', '-');
                        var ect = Path.GetExtension(blob.Uri.ToString().Split('/')[blob.Uri.ToString().Split('/').Length - 1]).TrimStart('.');
                        var id = blob.Uri.Segments[blob.Uri.Segments.Length - 3].Trim('/');

                        CloudBlockBlob blobDestination = dir.GetBlockBlobReference(
                            $"{id}/{name}.{ect}");


                        var sharedAccessUri = GetShareAccessUri(blobToMoveStr, 360, dirToRemove);

                        var blobUri = new Uri(sharedAccessUri);

                        await blobDestination.StartCopyAsync(blobUri).ConfigureAwait(false);
                        while (blobDestination.CopyState.Status != CopyStatus.Success)
                        {
                            Console.WriteLine(blobDestination.CopyState.Status);
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            await blobDestination.ExistsAsync();
                        }


                     ((CloudBlob)blobToMove).DeleteIfExists();
                    }
                }

                blobToken = result.ContinuationToken;
            } while (blobToken != null);

        }

    }
}
