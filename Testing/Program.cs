using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Notification;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Mail;

using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Storage;
using System.Web.Security;
using System.Security.Cryptography;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using System.Globalization;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.ViewModel.Queries.User;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.File;
using System.Net.Http;
using Zbang.Zbox.Infrastructure.MediaServices;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Dapper;
using System.Text.RegularExpressions;

namespace Testing
{
    class Program
    {


        //static UnityContainer unityFactory = new UnityContainer();
        static IThumbnailProvider m_ThumbnailProvider;
        static IBlobProvider m_BlobProvider;
        static ITableProvider m_TableProvider;
        //static IEnumerable<Country> GetCountries()
        //{
        //    return from ri in
        //               from ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
        //               select new RegionInfo(ci.LCID)
        //           group ri by ri.TwoLetterISORegionName into g
        //           //where g.Key.Length == 2
        //           select new Country
        //           {
        //               CountryId = g.Key,
        //               Title = g.First().DisplayName
        //           };
        //}
        class Country
        {
            public string CountryId { get; set; }
            public string Title { get; set; }
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

            //var s = "https://www.cloudents.com/d/lzodJqaBYHu/pD0nrbAtHSq";
            var xzx = GetTitle("http://www.vivt.ru/").Result;
            var xzx2 = GetTitle("http://www.ynet.co.il/").Result;
            var xzx3 = GetTitle("https://www.google.com/").Result;
            var xzx4 = GetTitle("https://www.cloudents.com/").Result;
            //System.Net.WebClient wb = new WebClient();
            //var x = wb.DownloadData(s);

            //using (HttpClient client = new HttpClient())
            //{

            //    using (var sr = client.GetAsync(s).Result)
            //    {
            //        Console.WriteLine(sr.Headers);
            //    }

            //}
            //var y = WebUtility.UrlDecode(s);
            //WebRequest request = WebRequest.Create(u);
            //var v = request.GetResponse();

            //ProtobufSerializer<Country> x = new ProtobufSerializer<Country>();
            //var z = x.SerializeData(new Country { CountryId = "1", Title = "2" });

            //ProtobufSerializer<FileProcessData> y = new ProtobufSerializer<FileProcessData>();
            //var zz = y.SerializeData(new FileProcessData { BlobName = new Uri("http://www.google.com"), ItemId = 1 });

            //            using (var conn = DapperConnection.OpenConnection().Result)
            //            {
            //                var sql = @" select u.userid as Uid , 
            //    coalesce(AliasName,userName) as Name,
            //    u.userimage as Image,u.NeedCode as NeedCode,
            //    (select count(*) from zbox.users where universityid2 = u.userid) as MemberCount
            //    from zbox.users u 
            //    where u.usertype = 1 
            //    and u.Country = @country
            //    and (@prefix IS NULL OR (coalesce(u.AliasName,u.userName) 
            //      like '%' + @prefix + '%' or coalesce(u.AliasName,u.userName) like '%' + @prefixHeb + '%'))
            //    order by MemberCount desc";
            //                var retVal = conn.Query(sql, new { country = "IL" , prefix = "ר", prefixHeb="ר" });

            //            }

            //DownloadFromDropBox();
            //TestVerifyAccountKey();
            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();

            Zbang.Zbox.ReadServices.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Mail.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();

            var x = new Zbang.Zbox.Infrastructure.IdGenerator.IdGenerator();
            var y = x.GetId();
            //var x = TestMediaServices();
            //Task.WaitAll(x);
            // return;
            Emails();

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


            var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;
            m_TableProvider = iocFactory.Resolve<ITableProvider>();
            var words = m_TableProvider.GetFileterWored();

            // var m_FileProcessorFactory = iocFactory.Resolve<IFileProcessorFactory>();

            //var processor = m_FileProcessorFactory.GetProcessor("b6c49f14-f5d7-4733-9540-fac304ade3a8.ppt");

            //var retVal = processor.ConvertFileToWebSitePreview("b6c49f14-f5d7-4733-9540-fac304ade3a8.ppt", 0, 0, 3);
            //m_ThumbnailProvider = iocFactory.Resolve<IThumbnailProvider>();
            //m_BlobProvider = iocFactory.Resolve<IBlobProvider>();
            //TestImage();
            //var ShortCode = iocFactory.Resolve<IShortCodesCache>();
            //var boxid = ShortCode.LongToShortCode(10691, ShortCodesType.User);
            // IZboxWriteService writeService = iocFactory.Resolve<IZboxWriteService>();
            //var command = new UpdateUserProfileCommand(1, "ramy", null, null, "המרכז האקדמי פרס");

            //writeService.Dbi();
            //writeService.Dbi();
            //IZboxReadService readService = iocFactory.Resolve<IZboxReadService>();
            // readService.GetUserDetailsByMembershipId(new GetUserByMembershipQuery(Guid.Parse("29D55B10-7840-48F6-945E-A57080D03229")));
            //var result = readService.GetUserDetailsByMembershipId(new GetUserByMembershipQuery(Guid.Parse("29D55B10-7840-48F6-945E-A57080D03229")));

            //var result = readService.GetLibraryNode(new Zbang.Zbox.ViewModel.Queries.Library.GetLibraryNodeQuery(322, Guid.Parse("d96e364f-0c0e-45a6-8dc9-8dc784beab95"), 1, 0, OrderBy.LastModified));
            //var result = readService.GetBox2(new GetBoxQuery(39, 1));

            // writeService.UpdateUserUniversity(new UpdateUserUniversityCommand("עזריאלי - מכללה אקדמית להנדסה", 1));

            //var command = new DeleteNodeFromLibraryCommand(Guid.Parse("40f213a7-4fc7-45af-a461-10389ca19596"), 14);

            //writeService.DeleteNodeLibrary(command);

            //Guid id = Guid.NewGuid();
            //var command = new AddNodeToLibraryCommand("t222", id, userId, Guid.Parse("48bb9ce3-7b31-47b8-aa19-0c56f552529a"));
            //writeService.AddNodeToLibrary(command);

            //var command = new UpdateUserProfileCommand(1, "ramvv", null, null, "מכללת רם");
            //writeService.UpdateUserProfile(command);
            //var x = readService.GetLibraryChildren(new Zbang.Zbox.ViewModel.Queries.Library.GetLibraryNodeQuery(1, null));

            //var x = readService.GetLibraryChildren(new Zbang.Zbox.ViewModel.Queries.Library.GetLibraryNodeQuery(1, null));

            // var x = readService.GetAcademicBoxNameByPrefix(new GetBoxByNameQuery("b",1));


            //var query = new GetBoxQuery(24,3);
            //var result = readService.GetBox(query);
            //readService.GetDashBoard(new GetDashBoardQuery(3));

            //var x = APIreadService.GetBoxItems(new GetBoxItemsPagedQuery(boxid, 6));
            //writeService.AddBoxComment(new Zbang.Zbox.Domain.Commands.AddBoxCommentCommand(3, 10, "this is a nice test"));
            //var x = writeService.AddFileToBox(new Zbang.Zbox.Domain.Commands.AddFileToBoxCommand(1, 781, "xxx4", "xxx4", 0, "test"));
            //writeService.AddTagToBox(new Zbang.Zbox.Domain.Commands.AddTagToBoxCommand(Guid.NewGuid(), 12, "test2", 3));
            //writeService.DeleteTagFromBox(new Zbang.Zbox.Domain.Commands.DeleteTagFromBoxCommand("test2", 185, 3));
            // var v = readService.GetBoxItem(new GetItemQuery(3, 1879, 12));
            //var v = readService.GetTags(new GetTagsQuery(3, ""));

            //var x = readService.GetBoxDateForInvite(new GetBoxInviteDataQuery(10));
            //var x = readService.GetUserDetailsByMembershipId(new GetUserByMembershipQuery(Guid.NewGuid()));
            //writeService.CreateUser(new Zbang.Zbox.Domain.Commands.CreateMembershipUserCommand(Guid.NewGuid(),"ram.y@outlook.com","ramyoutlook"));
            //IZboxService zboxService = unityFactory.Resolve<IZboxService>();
            //var x = zboxService.ChangeBoxFavorite(new Zbang.Zbox.Domain.Commands.ChangeBoxFavoriteCommand(false, 116, 3));
            //zboxService.AddBoxComment(new Zbang.Zbox.Domain.Commands.AddBoxCommentCommand(3, 116, "this is a test"));
            //Emails();
            //IThumbnailProvider provider = unityFactory.Resolve<IThumbnailProvider>();
            //var bytes = File.ReadAllBytes(@"C:\Users\ram.ZBANG-LOCAL\Desktop\nopreview_image.jpg");

            //var ImageResizer = new ImageResizer();
            //var ms = ImageResizer.ResizeImage(new MemoryStream(bytes), 1331, 1331);
            //File.WriteAllBytes(@"C:\Users\ram.ZBANG-LOCAL\Desktop\nopreview_image2.jpg", ms.ToArray());

            //zboxService.GetBoxUpdate(new GetBoxUpdatesQuery(156, 3, DateTime.Now.AddDays(-1)));
            //var result = zboxService.GetBox(new GetBoxQuery(156,3));
            //var x = zboxService.GetBoxComments(new GetBoxCommentsQuery(156,3));
        }

        private async static Task<string> TestMediaServices()
        {
            IFileProcessorFactory service = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IFileProcessorFactory>();
            var processor = service.GetProcessor(new Uri("http://127.0.0.1:10000/devstoreaccount1/zboxfiles/7c8aaf28-e398-49f3-85ef-b9e9b04848a3.jpg"));
            var x = await processor.PreProcessFile(new Uri("http://127.0.0.1:10000/devstoreaccount1/zboxfiles/7c8aaf28-e398-49f3-85ef-b9e9b04848a3.jpg"));
            // var id = await service.EncodeVideo(new Uri("https://zboxstorage.blob.core.windows.net/zboxfiles/f6a1b2b6-7c1c-44fc-961c-b90492c8552a.wmv"));
            //var id = await service.EncodeVideo(new Uri("https://zboxstorage.blob.core.windows.net/zboxfiles/test.wmv"));

            //Console.WriteLine(id);
            return await Task.FromResult<string>("s");
        }

        private async static void DownloadFromDropBox()
        {
            //https://dl.dropboxusercontent.com/1/view/2pdvchldh2fn3o0/ccc/%D7%9E%D7%95%D7%A0%D7%99%D7%9D.docx
            using (HttpClient client = new HttpClient())
            {
                var sr = await client.GetAsync("https://dl.dropboxusercontent.com/1/view/2pdvchldh2fn3o0/ccc/%D7%9E%D7%95%D7%A0%D7%99%D7%9D.docx");
                Console.Write("here");
                // File.WriteAllBytes(@"d:\test.doc", sr);
            }
        }

        private static void TestVerifyAccountKey()
        {

            var plaintextBytes = Encoding.UTF8.GetBytes("TsKl7pMWvW43552543535423453245");
            var hashId = MachineKey.Protect(plaintextBytes, "test");
            var g = Convert.ToBase64String(hashId, Base64FormattingOptions.None);
            Console.WriteLine(g);
            Console.WriteLine(g.Length);

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < hashId.Length; i++)
            {
                sBuilder.Append(hashId[i].ToString("x2"));
            }
            Console.WriteLine(sBuilder.ToString());
            Console.WriteLine(sBuilder.Length);
            //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //rng.
        }



        private static void TestImage()
        {

            //ImageResizer x = new ImageResizer();
            //var sr = File.Open(@"C:\Users\ram.ZBANG-LOCAL\Downloads\1081833.gif", FileMode.Open);
            //var sr2 = m_BlobProvider.DownloadFile("a1a148cd-d0af-43b6-894a-4f5468eb1eb8.jpg");

            //var sr3 = m_BlobProvider.DownloadFileToBytes("bf72a097-9141-4aae-95c3-1f61ceeaaeb3.gif");
            //var bytes = new byte[sr2.Length];
            //sr2.Read(bytes, 0, (int)sr2.Length);
            //var ms = new MemoryStream(bytes, true);
            //File.WriteAllBytes(@"C:\Users\ram.ZBANG-LOCAL\Downloads\1081833(3).gif", bytes);

            //const int BYTES_TO_READ = sizeof(Int64);
            //byte[] one = new byte[BYTES_TO_READ];
            //byte[] two = new byte[BYTES_TO_READ];
            //int iterations = (int)Math.Ceiling((double)sr.Length / BYTES_TO_READ);
            //for (int i = 0; i < iterations; i++)
            //{
            //    //sr.Read(one, 0, BYTES_TO_READ);
            //    sr2.Read(two, 0, BYTES_TO_READ);
            //    sw.Write(two);
            //    //if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
            //        //Console.WriteLine("not equal");
            //}
            //sw.Flush();
            //sw.Dispose();
            //var thumbnailUrl = m_ThumbnailProvider.GenerateThumbnail("9bfc91b0-269f-4afb-86e9-c5ff329b0b07.pdf");
            //using (StreamReader sr = new StreamReader(@"C:\Users\ram.ZBANG-LOCAL\Downloads\asset(1).PNG"))
            //{
            //sr.Position = 50;
            //var v = x.ResizeImageAndSave(sr2, 150, 150, true);
            //File.WriteAllBytes(@"C:\Users\ram.ZBANG-LOCAL\Downloads\1081833(1).jpg", v.ToArray());
            //}
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



            var mail = new Zbang.Zbox.Infrastructure.Mail.MailManager2();
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new InvitationToCloudentsMailParams("Eidan", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/401fe59e-1005-42a9-a97b-dc72f20abed4.jpg", new CultureInfo("en-Us")));
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new InvitationToCloudentsMailParams("Eidan", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/401fe59e-1005-42a9-a97b-dc72f20abed4.jpg", new CultureInfo("he-IL")));

            //mail.GenerateAndSendEmail("noatseitlin@facebook.com", new WelcomeMailParams("ram", new CultureInfo("ru-RU")));
            mail.GenerateAndSendEmail("yaari.ram@gmail.com", new WelcomeMailParams("ram2", new CultureInfo("en-Us")));

            mail.GenerateAndSendEmail("yaari.ram@gmail.com", new ForgotPasswordMailParams2("hfgkjsdhf##askjd", "https://www.cloudents.com", "ram", new CultureInfo("en-Us")));
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new ForgotPasswordMailParams("h$$$sdhf##askjd", new CultureInfo("en-Us")));

            // mail.GenerateAndSendEmail("yaari.ram@gmail.com", new InviteMailParams("some user name", "some box name", "some box url", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg", new CultureInfo("he-IL")));
            ////mail.GenerateAndSendEmail("yaari.ram@gmail.com", new InviteMailParams("some user name", "some box name", "some box url", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg", new CultureInfo("en-Us")));

            //mail.GenerateAndSendEmail("itsik.bitran@facebook.com", new MessageMailParams("some message", "some user name", new CultureInfo("he-IL"), "ram.y@outlook.com", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg"));
            ////mail.GenerateAndSendEmail("itsik.bitran@facebook.com", new MessageMailParams("some message", "some user name", new CultureInfo("en-Us"), "ram.y@outlook.com", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg"));


            //mail.GenerateAndSendEmail("itsik.bitran@facebook.com", new MessageMailParams("some message", "some user name", new CultureInfo("he-IL"), "yaari_r@yahoo.com", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg"));
            //mail.GenerateAndSendEmail("itsik.bitran@facebook.com", new MessageMailParams("some message", "some user name", new CultureInfo("en-Us"), "yaari_r@yahoo.com", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg"));

            //var updates = new List<Zbang.Zbox.Infrastructure.Mail.UpdateMailParams.BoxUpdate> 
            //{
            //    new UpdateMailParams.BoxUpdate("some box",
            //        new List<UpdateMailParams.BoxUpdateDetails> {
            //            new UpdateMailParams.BoxUpdateDetails(20,"some user", "some nice item name",EmailAction.AddedItem,"fakeUrl"),
            //            new UpdateMailParams.BoxUpdateDetails(21,"some user", string.Empty,EmailAction.AskedQuestion,"fakeUrl"),
            //            new UpdateMailParams.BoxUpdateDetails(22,"some user", "some nice item name",EmailAction.AskedQuestion,"fakeUrl"),
            //            new UpdateMailParams.BoxUpdateDetails(23,"some user", "some box name",EmailAction.Join,"fakeUrl")
            //        }),

            //    new UpdateMailParams.BoxUpdate("some box2",
            //        new List<UpdateMailParams.BoxUpdateDetails> {
            //            new UpdateMailParams.BoxUpdateDetails(24,"some user", "some nice item name",EmailAction.AddedItem,"fakeUrl"),
            //            new UpdateMailParams.BoxUpdateDetails(25,"some user", string.Empty,EmailAction.AskedQuestion,"fakeUrl"),
            //            new UpdateMailParams.BoxUpdateDetails(26,"some user", "some nice item name",EmailAction.AskedQuestion,"fakeUrl"),
            //            new UpdateMailParams.BoxUpdateDetails(27,"some user", "some box name",EmailAction.Join,"fakeUrl")
            //        })

            //};
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new UpdateMailParams(updates, new CultureInfo("ru-RU")));
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new ChangeEmailMailParams("7656", new CultureInfo("ru-RU")));
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new ChangeEmailMailParams("7656", new CultureInfo("en-Us")));

            //left new msg, update

            //var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;

            //IQueueProvider m_QueueProvider = iocFactory.Resolve<IQueueProvider>();
            //m_QueueProvider.InsertMessageToMailNew(new Zbang.Zbox.Infrastructure.Transport.WelcomeMailData("yaari.ram@gmail.com", "Ram Yaari", "en-Us"));

            //IMailManager mail = iocFactory.Resolve<IMailManager>();
            //IZboxReadService zboxService = iocFactory.Resolve<IZboxReadService>();
            //const string recepient = "yaari.ram@gmail.com";

            //var query1 = new GetBoxInviteDataQuery(11);
            //var boxData = zboxService.GetBoxDateForInvite(query1);

            //var parameters = new ForgotPassword("123qwe");
            //mail.SendEmail(parameters, recepient);
            //var parameters2 = new EmailVerification("VERY LONG HASSSSSSSSH", "Ram Yaaer");
            //mail.SendEmail(parameters2, recepient);
            //var parameters3 = new ChangeEmail(5000);
            //mail.SendEmail(parameters3, recepient);
            //var parameters4 = new ItemDeleted("This is the box", "This is an item name", "this is the user");
            //mail.SendEmail(parameters4, recepient);
            //var parameters5 = new Subscription("this is box", "lovely user");
            //mail.SendEmail(parameters5, recepient);
            //var parameters6 = new InviteToBox("Ram", "JOIN NOW!!!", boxData.Name,
            //                                 new Uri("https://ram.multimicloud.com/Box?BoxUid=lzodJqaBLkm"),
            //                                 boxData.UpdateTime, boxData.NumOfItems,
            //                                 boxData.Image);
            //mail.SendEmail(parameters6, recepient);

            //var parameters10 = new Updates("Ken", "ram", new Uri("https://ram.multimicloud.com/Box?BoxUid=lzodJqaBLkm"),
            //    new List<CommentDetails> { new CommentDetails("ram", TimeSpan.FromHours(5), "This is a cool image",
            //        "https://zboxstorage.blob.core.windows.net/zboxprofilepic/fd1de52d-859d-4815-a063-211fba152998")},
            //        new List<FileDetails> { new FileDetails("ram",TimeSpan.FromHours(5),
            //            "https://zboxstorage.blob.core.windows.net/zboxprofilepic/fd1de52d-859d-4815-a063-211fba152998",
            //            "test.png",
            //            "https://zboxstorage.blob.core.windows.net/zboxthumbnail/863fa055-1e48-4c0d-b99d-9879b8d61e7d.thumbnail.jpg")});
            //mail.SendEmail(parameters10, recepient);
            ////var d = zboxService.GetBoxDataForItemAdd(new GetBoxDataForImmediateEmailQuery(12, 1808));

            //var userid = 3;
            //var query = new GetBoxesDigestQuery(NotificationSettings.OnceAWeek, userid);
            //var result = zboxService.GetBoxIdList(query);
            //List<Updates> boxesUpdates = new List<Updates>();
            //foreach (var boxid in result)
            //{

            //    var boxQuery = new GetBoxDataForDigestEmailQuery(NotificationSettings.OnceAWeek, boxid.Id);
            //    var boxdata = zboxService.GetBoxDataForDigestEmail(boxQuery);
            //    var update = new Updates(boxdata.Name, boxdata.Owner,
            //                             new Uri("https://ram.multimicloud.com/Box?BoxUid=lzodJqaBLkm"),
            //                             boxdata.Comments.Select(s => new CommentDetails(s.AuthorName, DateTime.UtcNow - s.UpdateTime,
            //                                 s.CommentText, s.UserImage)).ToList(),
            //                             boxdata.Files.Select(
            //                                 s => new FileDetails(s.UploaderName, DateTime.UtcNow - s.CreationTime,
            //                                                      s.UploaderImage, s.Name, s.ThumbnailBlobUrl)).ToList());
            //    boxesUpdates.Add(update);

            //    //List<FileDetails> FileDataForEmail = boxdata.Files.Select(c => new FileDetails(c.Uid, c.Name, c.ThumbnailBlobUrl, c.BoxUid)).ToList();
            //    //List<CommentDetails> CommentDataForEmail = boxdata.Comments.Select(s => new CommentDetails { Comment = s.CommentText, Time = (DateTime.UtcNow - s.CreationTime), UserName = s.AuthorName }).ToList();


            //    //boxesUpdates.Add(new Updates(m_ShortToLongCode.LongToShortCode(boxdata.Id), boxdata.Name, FileDataForEmail, CommentDataForEmail, boxdata.Owner));

            //}
            //var parameters7 = new Digest(boxesUpdates);
            //mail.SendEmail(parameters7, recepient);
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
