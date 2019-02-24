using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Storage;
using Cloudents.Query;
using Cloudents.Search.Document;
using Cloudents.Search.Question;
using Dapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using SimMetricsMetricUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ConsoleApp
{
    internal static class Program
    {
        private static IContainer _container;
        //private static CancellationToken token = CancellationToken.None;

        //private static readonly log4net.ILog log =
        //    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static async Task Main()
        {
            
            var builder = new ContainerBuilder();
            var keys = new ConfigurationKeys("https://www.spitball.co")
            {
                Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                    ConfigurationManager.AppSettings["Redis"]),
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"], true),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"],
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

            //await CoursesWithSimilarNames();
            //await FunctionsExtensions.MergeCourses(_container);

            var d = _container.Resolve<DapperRepository>();
            

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

            }

        }

        

       
       
    }
}
