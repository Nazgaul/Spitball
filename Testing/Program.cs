using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Autofac;
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





        static void Main(string[] args)
        {
            emailsVerify();
            var unity = IocFactory.IocWrapper;
            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();

            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Mail.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbang.Zbox.ReadServices.RegisterIoc.Register();
            unity.ContainerBuilder.RegisterType<SeachConnection>()
                    .As<ISearchConnection>()
                    .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
                    .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
                    .WithParameter("isDevelop", false) // this do nothing its only read only
                    .InstancePerLifetimeScope();
            Zbang.Zbox.Infrastructure.Search.RegisterIoc.Register();

            unity.ContainerBuilder.RegisterType<SendPush>()
            .As<ISendPush>()
            .WithParameter("connectionString", ConfigFetcher.Fetch("ServiceBusConnectionString"))
            .WithParameter("hubName", ConfigFetcher.Fetch("ServiceBusHubName"))
            .InstancePerLifetimeScope();

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
            var service = iocFactory.Resolve<IMailComponent>();
            //var t = service.CheckEmailValidateAsync("exn5038@psu.edu");
            //var t = service.SendSpanGunEmailAsync("ariel@cloudents.com", "1",
            //    "Spitball, the free social studying app, has over 250,000 students across the world using our product to access class documents (including past exams, study guides, lecture notes, etc).You can also chat with your classmates, create quizzes, and more, all for free!    We’re live at Auburn this semester, and we have over 1,900 documents exclusively for Auburn students!    Sign up for free today, it takes less than a minute. Check us out!",
            //    "Spitball has launched at Auburn!", "Cecily", "auburn_6272_s1", "https://www.spitball.co/auburn/");
            //t.Wait();
            //foreach (var email in emails())
            //{
            //    var result = service.VerifyEmail(email);
            //    Console.WriteLine($"{email} : {result}");
            //}

            //var clusterflunkFilesCopy = new ClusterflunkFilesCopy(iocFactory);
            //var t1 = clusterflunkFilesCopy.BuildBoxesAsync();
            //t1.Wait();
            //var t = clusterflunkFilesCopy.BuildFilesAsync();
            //t.Wait();
            //Application.Run(new Form1());
            //Console.WriteLine("Finish");
            //Console.ReadLine();

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
                LocalEndPoint = new IPEndPoint(IPAddress.Parse("184.173.153.166"),25)
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


        public static void ShowChars(char[] charArray)
        {
            Console.WriteLine("Char\tHex Value");
            // Display each invalid character to the console. 
            foreach (char someChar in charArray)
            {
                if (Char.IsWhiteSpace(someChar))
                {
                    Console.WriteLine(",\t{0:X4}", (int)someChar);
                }
                else
                {
                    Console.WriteLine("{0:c},\t{1:X4}", someChar, (int)someChar);
                }
            }
        }
        private static void Emails()
        {



            var mail = new MailManager2();
            var x = mail.GetUnsubscribesAsync(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), 0);
            x.Wait();


            var t = mail.GenerateAndSendEmailAsync("irena@cloudents.com",
                new WelcomeMailParams("אירנה", new CultureInfo("he")));
            t.Wait();

            t = mail.GenerateAndSendEmailAsync("irena@cloudents.com",
               new WelcomeMailParams("Irena", new CultureInfo("en")));
            t.Wait();

            t = mail.GenerateAndSendEmailAsync("shlomi@cloudents.com",
                new WelcomeMailParams("אירנה", new CultureInfo("he")));
            t.Wait();

            t = mail.GenerateAndSendEmailAsync("shlomi@cloudents.com",
               new WelcomeMailParams("Irena", new CultureInfo("en")));
            t.Wait();

            t = mail.GenerateAndSendEmailAsync("irena@cloudents.com",
                new DepartmentRequestAccessMailParams(new CultureInfo("en"), "Ire", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg", "woop"));
            t.Wait();

            t = mail.GenerateAndSendEmailAsync("irena@cloudents.com",
                new DepartmentRequestAccessMailParams(new CultureInfo("he"), "Ire", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg", "woop"));
            t.Wait();

            t = mail.GenerateAndSendEmailAsync("irena@cloudents.com",
               new DepartmentApprovedMailParams(new CultureInfo("he"), "woop"));
            t.Wait();

            t = mail.GenerateAndSendEmailAsync("irena@cloudents.com",
               new DepartmentApprovedMailParams(new CultureInfo("en"), "woop"));
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
