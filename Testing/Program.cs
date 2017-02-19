using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Autofac;
using Dapper;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using NHibernate;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Mail;

using System.Diagnostics;
using Zbang.Zbox.Infrastructure.Storage;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Cobisi.EmailVerify;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Url;
using File = System.IO.File;
using System.Collections.Generic;
using System.Threading;
using AlchemyAPIClient;
using AlchemyAPIClient.Requests;
using Autofac.Features.ResolveAnything;
using Autofac.Features.Variance;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;

namespace Testing
{
    class Program
    {

        static void GetXml()
        {
            using (var sr = File.Open(@"C:\Users\Ram\Desktop\jobs.xml", FileMode.Open))
            {
                var doc = XDocument.Load(sr);
                var y = doc.Descendants("job").Select(s =>
                {
                    var offer = string.Empty;
                    var offerElement = s.Element("offer");
                    if (offerElement != null)
                    {
                        offer = offerElement.Value;
                    }
                    return new
                    {
                        name = s.Attribute("name").Value,
                        location = s.Attribute("location").Value,
                        description = s.Element("description").Value,
                        offer
                    };
                });
            }
        }


        static async Task<string> GetTitle(string url)
        {

            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {

                var uri = new UriBuilder(url).Uri;
                var response = await client.GetAsync(uri);
                var str = await response.Content.ReadAsStringAsync();
                var html = string.Empty;

                var x = Regex.Match(str, "<meta.*?charset=([^\"']+)");
                var charset = x.Groups[1];
                if (string.IsNullOrEmpty(charset.Value))
                {
                    //var start = str.IndexOf("charset=");
                    //if (start == -1)
                    //{
                    html = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // var end = str.IndexOf('"', start);

                    //var encoding = str.Substring(start, end - start).Remove(0, 8);

                    //HtmlAgilityPack.HtmlWeb x = new HtmlAgilityPack.HtmlWeb();
                    //var v = x.Load(url);

                    //var html = await client.GetStringAsync(uri);

                    //var bytes = await client.GetByteArrayAsync(uri);
                    html = Encoding.GetEncoding(charset.Value).GetString(await response.Content.ReadAsByteArrayAsync());
                }
                //return responseString;

                string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                return title;
            }
        }

        static async Task DoAlchemyAsync(string text)
        {
            AlchemyClient client =
                new AlchemyClient("785ea0b610cc18cf9cb3815552d2bbd979133a5b"
                    // "https://gateway-a.watsonplatform.net/calls"
                    );
            var request = new AlchemyTextConceptsRequest(text, client)
            {
                KnowledgeGraph = true,
                LinkedData = true,
                ShowSourceText = true,
                MaxRetrieve = 30
            };
            var z = await request.GetResponse();

        }





        static void Main(string[] args)
        {
            //var t1 = DoAlchemyAsync();
            //t1.Wait();
            //var z = GuidEncoder.Encode("0114F1D3-85E3-40D6-B6FA-A5D7000465CA");
            //var v =  GuidEncoder.Decode(z);
            var unity = IocFactory.IocWrapper;
            //Console.WriteLine(Environment.MachineName);
            //Console.WriteLine("Hello\vWorld\n\n");
            //var tr = getAllInvalidLinks();
            //Task.WaitAll(tr);


            //Console.WriteLine("ended ");
            //k.Wait();

            //return;
            //Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            unity.ContainerBuilder.RegisterModule<WriteServiceModule>();
            unity.ContainerBuilder.RegisterModule<DataModule>();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            unity.ContainerBuilder.RegisterModule<CommandsModule>();

            Zbang.Zbox.Infrastructure.Mail.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.File.RegisterIoc.Register();
            unity.ContainerBuilder.RegisterModule<StorageModule>();
            // Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbang.Zbox.ReadServices.RegisterIoc.Register();
            unity.ContainerBuilder.RegisterModule<InfrastructureModule>();
            //unity.ContainerBuilder.RegisterType<SeachConnection>()
            //        .As<ISearchConnection>()
            //        .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
            //        .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
            //        .InstancePerLifetimeScope();

            unity.ContainerBuilder.RegisterModule<SearchModule>();
            unity.ContainerBuilder.RegisterModule<AiModule>();



            unity.ContainerBuilder.RegisterType<SendPush>()
            .As<ISendPush>()
            .WithParameter("connectionString", ConfigFetcher.Fetch("ServiceBusConnectionString"))
            .WithParameter("hubName", ConfigFetcher.Fetch("ServiceBusHubName"))
            .InstancePerLifetimeScope();
            unity.ContainerBuilder.RegisterSource(new ContravariantRegistrationSource());
            unity.Build();



            //var x = new Zbang.Zbox.Infrastructure.IdGenerator.IdGenerator();
            //var y = x.GetId();
            //var x = TestMediaServices();
            //Task.WaitAll(x);
            // return;
            //Emails();

            //var y =  GetCountries();


            // Get a list of invalid path characters. 
            //char[] invalidPathChars = Path.GetInvalidPathChars();

            //Console.WriteLine("The following characters are invalid in a path:");
            //ShowChars(invalidPathChars);
            //Console.WriteLine();

            //// Get a list of invalid file characters. 
            //char[] invalidFileChars = Path.GetInvalidFileNameChars();

            //Console.WriteLine("The following characters are invalid in a filename:");
            //ShowChars(invalidFileChars);
            //Calculation(); 
            //CastingPerformance();
            //log4net.Config.XmlConfigurator.Configure();

            var iocFactory = IocFactory.IocWrapper;
            var m_BlobProvider = iocFactory.Resolve<IBlobProvider2<FilesContainerName>>();
            var m_FileProcessorFactory = iocFactory.Resolve<IFileProcessorFactory>();
            var languageDetection = iocFactory.Resolve<IDetectLanguage>();
            var writeService = iocFactory.Resolve<IZboxWriteService>();
            var search = iocFactory.Resolve<IContentReadSearchProvider>();


            //var updateDate = new UpdateData(1028091, 8417, itemId: 606505);
            //var y = iocFactory.Resolve<IDomainProcess>(updateDate.ProcessResolver);
            //await y.ExecuteAsync(updateDate, cancellationToken);


            //var tSearch = search.SearchAsync(new SearchAllDocuments {Term = "Socrates"}, default(CancellationToken));
            //tSearch.Wait();

            //var ai = iocFactory.Resolve<IWitAi>();
            //var tAi = ai.GetUserIntentAsync("Give me all material on Socrates", default(CancellationToken));
            //tAi.Wait();
            //var write = iocFactory.Resolve<IContentWriteSearchProvider>();
            //var zzzzzz = write.UpdateDataAsync(null, null, default(CancellationToken));
            //zzzzzz.Wait();
            //var z = new AssignTagsToItemCommand(elem.Id, result, language);
            // var readService = iocFactory.Resolve<IZboxReadServiceWorkerRole>();
            //var z3 =  readService.GetFlashcardsDirtyUpdatesAsync(0, 1, 100, default(CancellationToken));
            // z3.Wait();

            // var uri = m_BlobProvider.GetBlobUrl("e3562f39-3e3b-41d2-9b82-5a17ae51a47d.pdf");
            // var processor = m_FileProcessorFactory.GetProcessor(uri);
            // var t = processor.ExtractContentAsync(uri);
            // t.Wait();
            // var cxz = languageDetection.DoWork(t.Result);
            // var z = DoAlchemyAsync(t.Result);
            // z.Wait();

            // var z = new AssignTagsToItemCommand(565902, new[] { "1234", "Moment of inertia" });
            // m_WriteService.AddItemTag(z);
            return;
            //var push = iocFactory.Resolve<ISendPush>();
            //var t = push.GetRegisteredUsersAsync();
            //t.Wait();
           
            var m_QueueRepository = iocFactory.Resolve<IQueueProvider>();
            var index = 0;
            IEnumerable<long> users;
           
            Console.WriteLine("Finish");
            Console.ReadLine();

        }

        private static void emailsVerify()
        {
            LicensingManager.SetLicenseKey("FHbrz2C/8XTEPkmsVzPzPuYvZ2XNoMDfMEdJKJdvGlwPpkAgNwQMVT+Ae1ZSY8QbQpm+7g==");
            var verifier = new VerificationEngine();
            var settings = new VerificationSettings
            {
                AllowDomainLiterals = false,
                AllowComments = true,
                AllowQuotedStrings = true,
                LocalHostFqdn = "mg.spitball.co",
                LocalSenderAddress = "michael@spitball.co",
                LocalEndPoint = new IPEndPoint(IPAddress.Parse("184.173.153.166"), 25)
            };

            // The component will use just the provided DNS server for its lookups

            settings.DnsServers.Clear();
            settings.DnsServers.Add(IPAddress.Parse("8.8.8.8"));
            // settings.SmtpConnectionTimeout = TimeSpan.FromMinutes(5);
            var x = verifier.Run("MHW0008@auburn.edu", VerificationLevel.CatchAll, settings).Result;
            var result = verifier.Run("LEW0019@auburn.edu", VerificationLevel.CatchAll, settings).Result;


        }
        //private static string[] emails()
        //{
        //    return new[]
        //    {
        //        "yaari.ram@gmail.com",
        //        "emo0002@auburn.edu",
        //        "mab0076@auburn.edu",
        //        "blb0037@auburn.edu",
        //        "wsb0012@auburn.edu",
        //        "anb0033@auburn.edu",
        //        "rsb0013@auburn.edu",
        //        "cjb0043@auburn.edu",
        //        "hrb0009@auburn.edu",
        //        "azb0059@auburn.edu",
        //        "keb0067@auburn.edu",
        //        "tlb0024@auburn.edu",
        //        "wzb0019@auburn.edu",
        //        "mbb0023@auburn.edu",
        //        "aab0029@auburn.edu",
        //        "jwb0032@auburn.edu",
        //        "rzb0033@auburn.edu",
        //        "wgb0010@auburn.edu",
        //        "nmb0005@auburn.edu",
        //        "azb0039@auburn.edu",
        //        "gvb0002@auburn.edu",
        //        "mab0045@auburn.edu",
        //        "jzb0032@auburn.edu",
        //        "krb0031@auburn.edu",
        //        "deb0017@auburn.edu",
        //        "jlb0073@auburn.edu",
        //        "spa0001@auburn.edu",
        //        "eab0046@auburn.edu",
        //        "mtb0021@auburn.edu",
        //        "bailegm@auburn.edu",
        //        "grb0009@auburn.edu",
        //        "fmb0005@auburn.edu",
        //        "kga0004@auburn.edu",
        //        "maa0043@auburn.edu",
        //        "sra0016@auburn.edu",
        //        "dza0016@auburn.edu",
        //        "sla0018@auburn.edu",
        //        "tla0006@auburn.edu",
        //        "wta0007@auburn.edu",
        //        "kma0018@auburn.edu",
        //        "dba0005@auburn.edu",
        //        "dma0008@auburn.edu",
        //        "mca0003@auburn.edu",
        //        "apa0003@auburn.edu",
        //        "hra0005@auburn.edu",
        //        "aja0019@auburn.edu",
        //        "cwa0001@auburn.edu",
        //        "ana0014@auburn.edu",
        //        "bda0005@auburn.edu",
        //        "gza0011@auburn.edu",
        //        "mma0020@auburn.edu",
        //        "cga0006@auburn.edu"
        //    };
        //}

        //private static async Task GetValue()
        //{
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    var httpClient = new HttpClient();
        //    var iocFactory = IocFactory.IocWrapper;
        //    var blobProvider = iocFactory.Resolve<ICloudBlockProvider>();
        //    using (var conn = DapperConnection.OpenConnection())
        //    {
        //        var data = conn.Query("select Id, LargeImage from zbox.University where LargeImage not like 'https://az%' ");
        //        foreach (var singleRow in data)
        //        {
        //            string extension = Path.GetExtension(singleRow.LargeImage);
        //            try
        //            {
        //                using (var sr = await httpClient.GetAsync(singleRow.LargeImage))
        //                {

        //                    if (!sr.IsSuccessStatusCode)
        //                    {
        //                        conn.Execute("update zbox.university set isdirty = 1, largeImage=null where id=@id", new
        //                        {
        //                            //image = $"https://az32006.vo.msecnd.net/universities/{singleRow.Id}{extension}",
        //                            id = singleRow.Id
        //                        });
        //                        continue;
        //                        //throw new UnauthorizedAccessException("Cannot access dropbox");
        //                    }
        //                    ////sr.Content.Headers.ContentType.
        //                    CloudBlockBlob blob = blobProvider.GetFile(singleRow.Id.ToString() + extension,
        //                        "universities");
        //                    using (var stream = await blob.OpenWriteAsync())
        //                    {
        //                        await sr.Content.CopyToAsync(stream);

        //                    }
        //                    blob.Properties.ContentType = sr.Content.Headers.ContentType.MediaType;
        //                    blob.Properties.CacheControl = "public  max-age=" + TimeConst.Week;
        //                    await blob.SetPropertiesAsync();
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                conn.Execute("update zbox.university set isdirty = 1, largeImage=null where id=@id", new
        //                {
        //                    //image = $"https://az32006.vo.msecnd.net/universities/{singleRow.Id}{extension}",
        //                    id = singleRow.Id
        //                });
        //                continue;
        //            }
        //            conn.Execute("update zbox.university set isdirty = 1, largeImage=@image where id=@id", new
        //            {
        //                image = $"https://az32006.vo.msecnd.net/universities/{singleRow.Id}{extension}",
        //                id = singleRow.Id
        //            });
        //        }
        //    }
        //}

        /**
        * This function return the number of not deleted links
        * */
        private static async Task<int> LinksNum()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return conn.QuerySingle<int>(@"select count(*) from zbox.Item 
                where  Discriminator = 'Link'
                and isdeleted = 0");
            }
        }

        /**
         * This function write to csv file all invalid links (URL,ids, reason)
         * **/
        public static async Task<bool> getAllInvalidLinks()
        {
            var validLinks = new List<String>();
            var invalidLinks = new List<LinkItemStatus>();
            var numOfRows = await LinksNum();

            //Calc the Max index that return results
            var maxIndex = Math.Ceiling(numOfRows / (double)100);
            var tasks = new List<Task>();

            //Go over the indexes and add the invalidLinks action to the task list
            for (var index = 0; index < maxIndex; index++)
            {
                Console.WriteLine("The index is:" + index + "\\" + maxIndex);
                tasks.Add(GetLinkItems(invalidLinks, validLinks, index, 100));
            }

            //Wait untill all lines will be called
            await Task.WhenAll(tasks);

            //Write to the csv file the invalid links and reason
            using (var sw = new StreamWriter(@"C:\Users\USER\Desktop\InvalidLinksList.csv"))
            {
                foreach (var obj in invalidLinks)
                    sw.WriteLine(String.Format("{0},{1},{2}", obj.BlobName, string.Join(" ", obj.ItemIds), obj.Code));
            }
            return true;
        }
        /**
         * This function check the validity of links item according to the index and offset parameters
         * add the invalid links to the InvalidLinks Parameter and the valid links to the validList parameter
         * */
        public static async Task GetLinkItems(List<LinkItemStatus> invalidLinks, List<string> validLinks, int index = 0, int RowSize = 100)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var VALID_STATUS = HttpStatusCode.OK.ToString();
                var httpClient = new HttpClient();
                var list = conn.Query(@"select itemid, blobname from zbox.Item 
                where  Discriminator = 'Link'
                and isdeleted = 0
                order by itemid
                OFFSET @Offset ROWS
                FETCH NEXT @RowSize ROWS ONLY"
                , new { Offset = index * 100, RowSize });

                Console.WriteLine($"Start index {index}");
                //Run on the Query result rows
                foreach (var singleRow in list)
                {
                    var currentUrl = singleRow.blobname;
                    //Check if currentUrl exist in one of the given lists(valid or invalid)
                    var dictVal = invalidLinks.Find(r => r.BlobName == currentUrl);
                    //CurrentUrl is already known as valid
                    if (validLinks.IndexOf(currentUrl) > -1) { continue; }
                    //CurrentUrl known as invalid
                    if (dictVal != null)
                    {
                        //Add the current id to the invalidItem ids list
                        var idsList = dictVal.ItemIds.AsList();
                        idsList.Add(singleRow.itemid);
                        dictVal.ItemIds = idsList.ToArray();
                        Console.WriteLine(string.Format("id :{0} val {1}", singleRow.itemid, dictVal.Code));
                    }
                    else
                    {
                        var status = "";
                        try
                        {
                            //Start http process
                            using (var sr = await httpClient.GetAsync(singleRow.blobname))
                            {
                                //The response Ok add the item to the valid List and update the Status
                                if (sr.IsSuccessStatusCode)
                                {
                                    validLinks.Add(currentUrl);
                                    status = VALID_STATUS;
                                }
                                else
                                {
                                    status = ($"{((HttpResponseMessage)sr).StatusCode}({((int)((HttpResponseMessage)sr).StatusCode)})");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            status = e.Message;
                            Console.WriteLine(string.Format("issue id :{0} val {1} error:{2}", singleRow.itemid, singleRow.blobname, e.Message));
                        }
                        finally
                        {
                            if (status != VALID_STATUS)
                                invalidLinks.Add(new LinkItemStatus(singleRow.blobname, singleRow.itemid, status));
                        }


                    }
                }
                Console.WriteLine($"end index {index}");
            }
        }
        /**
         * This Class Used for Invalid Items 
         * */
        public class LinkItemStatus
        {
            public LinkItemStatus(string blobName, long id, string code)
            {
                BlobName = blobName;
                ItemIds = new[] { id };
                Code = code;
            }
            public long[] ItemIds { get; set; }
            public string BlobName { get; set; }
            public string Code { get; set; }
        }
        //public static void ShowChars(char[] charArray)
        //{
        //    Console.WriteLine("Char\tHex Value");
        //    // Display each invalid character to the console. 
        //    foreach (char someChar in charArray)
        //    {
        //        if (Char.IsWhiteSpace(someChar))
        //        {
        //            Console.WriteLine(",\t{0:X4}", (int)someChar);
        //        }
        //        else
        //        {
        //            Console.WriteLine("{0:c},\t{1:X4}", someChar, (int)someChar);
        //        }
        //    }
        //}
        private static void Emails()
        {



            var mail = new MailManager2();
            var x = mail.GetUnsubscribesAsync(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), 0);
            x.Wait();

            var z = new[] { new LikeData
                {
                    OnLikeText = "טקסט",
                    UserName = "שם משתמש",
                    Type = Zbang.Zbox.Infrastructure.Enums.LikeType.Comment
                } };
            var t = mail.GenerateAndSendEmailAsync("ariel@cloudents.com",
                new LikesMailParams(new CultureInfo("he"), "אריאל", z));
            t.Wait();

            t = mail.GenerateAndSendEmailAsync("ariel@cloudents.com",
               new ReplyToCommentMailParams(new CultureInfo("he"), "אריאל", "משתמש מסויים", "שם הקופסא", "/"));
            t.Wait();

            mail.GenerateAndSendEmailAsync("yaari.ram@gmail.com",
                new FlagItemMailParams("דגכחלדיג", "חדיגחלכדיכ", "גלחכךדגחכך", "asdas", "asda")).Wait();
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new StoreOrder("ram y", "הליכון משגע", 12341234));


            mail.GenerateAndSendEmailAsync("yaari.ram@gmail.com", new InvitationToCloudentsMailParams("Eidan",
                "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/401fe59e-1005-42a9-a97b-dc72f20abed4.jpg",
                new CultureInfo("en-Us"), "yaari.ram@gmail.com"
                , "https://develop.cloudents.com/account/")).Wait();

        }

        private static void CastingPerformance()
        {
            object x = "xxx";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string s = (string)x;
            sw.Stop();
            Console.WriteLine(sw.ElapsedTicks);
            sw.Start();
            string v = x as string;
            sw.Stop();
            Console.WriteLine(sw.ElapsedTicks);
        }
    }
}
