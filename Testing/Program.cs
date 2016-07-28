using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Autofac;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Mail;

using System.Diagnostics;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Storage;
using System.Web.Security;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Repositories;
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



            var unity = IocFactory.IocWrapper;
            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();

            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Mail.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
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
            var writeService = iocFactory.Resolve<IZboxWorkerRoleService>();
            writeService.OneTimeDbi();
            //writeService.DeleteNodeLibrary(new DeleteNodeFromLibraryCommand(Guid.Parse("6FB8A861-F1FC-4D14-B838-A56D011BFCAA"), 166100));

        }

        private static async Task GetValue()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpClient = new HttpClient();
            var iocFactory = IocFactory.IocWrapper;
            var blobProvider = iocFactory.Resolve<ICloudBlockProvider>();
            using (var conn = DapperConnection.OpenConnection())
            {
                var data = conn.Query("select Id, LargeImage from zbox.University where LargeImage not like 'https://az%' ");
                foreach (var singleRow in data)
                {
                    string extension = Path.GetExtension(singleRow.LargeImage);
                    try
                    {
                        using (var sr = await httpClient.GetAsync(singleRow.LargeImage))
                        {

                            if (!sr.IsSuccessStatusCode)
                            {
                                conn.Execute("update zbox.university set isdirty = 1, largeImage=null where id=@id", new
                                {
                                    //image = $"https://az32006.vo.msecnd.net/universities/{singleRow.Id}{extension}",
                                    id = singleRow.Id
                                });
                                continue;
                                //throw new UnauthorizedAccessException("Cannot access dropbox");
                            }
                            ////sr.Content.Headers.ContentType.
                            CloudBlockBlob blob = blobProvider.GetFile(singleRow.Id.ToString() + extension,
                                "universities");
                            using (var stream = await blob.OpenWriteAsync())
                            {
                                await sr.Content.CopyToAsync(stream);

                            }
                            blob.Properties.ContentType = sr.Content.Headers.ContentType.MediaType;
                            blob.Properties.CacheControl = "public  max-age=" + TimeConst.Week;
                            await blob.SetPropertiesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Execute("update zbox.university set isdirty = 1, largeImage=null where id=@id", new
                        {
                            //image = $"https://az32006.vo.msecnd.net/universities/{singleRow.Id}{extension}",
                            id = singleRow.Id
                        });
                        continue;
                    }
                    conn.Execute("update zbox.university set isdirty = 1, largeImage=@image where id=@id", new
                    {
                        image = $"https://az32006.vo.msecnd.net/universities/{singleRow.Id}{extension}",
                        id = singleRow.Id
                    });
                }
            }
        }


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
