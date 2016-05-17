﻿using System;
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
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Zbang.Zbox.Domain.Commands;

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
           // var he = new CultureInfo("he");
           // var en = new CultureInfo("en");

            //GetXml();
            //HatavotWrite();
            //UniversitySearchProvider x = new UniversitySearchProvider();
            //x.BuildUniversityData();
            //            var x2 = @". עיקרי תוכנית בוורידג*. 
            //תכנית השמה לה ליעד להביא לחיסול המחסור בתקופה שלאחר המלחמה. המטרה האסטרטגית של התוכנית הייתה ליצור מערכת מקיפה של ביטחון סוציאלי, שתבטיח לכל אדם רמת מינימום של הכנסה. רמת המינימום נועדה להיות מספיקה לקיום, אך לשמש בתור רצפה בלבד, שאף אחד לא ייפול מתחתיה. על הרצפה הזו, יוכל כל אחד להוסיף לעצמו הכנסה מעבודה, או חסכון. היעד השני של התוכנית היה, שבנוסף לחופש ממחסור, אנגליה תבנה מדיניות חברתית מקיפה לקידום חברתי, שתכלול גם מאבק ב ענקיםהבאים: חולי, בערות, עוני, ואבטלה. התוכנית נועדה ליצור מערכת ביטחון סוציאלי, שתלווה כל אדם מהעריסה עד הקבר ותבטיח לו הכנסת מינימום.
            //האם תוכנית בוורידג' רלוונטית לימינו? – התבססות על מאמריהם של לאה אחדות ואברהם דורון:";

            //            x2 = x2.Substring(0, Math.Min(x2.Length, 400));
            //var s = "https://www.cloudents.com/d/lzodJqaBYHu/pD0nrbAtHSq";
            //var xzx = GetTitle("http://www.vivt.ru/").Result;
            //var xzx2 = GetTitle("http://www.ynet.co.il/").Result;
            //var xzx3 = GetTitle("https://www.google.com/").Result;
            //var xzx4 = GetTitle("https://www.cloudents.com/").Result;
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


            var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.IocWrapper;
            //var t = IndexItemSearch(iocFactory);
            //t.Wait();
            var m_WriteService = iocFactory.Resolve<IZboxWriteService>();
            //lucenewire.BuildUniversityData();
            var roomCommand = new ChatCreateRoomCommand(new[] { 1L, 7L }, Guid.NewGuid());
            var t1 = m_WriteService.AddChatRoomAsync(roomCommand);
            t1.Wait();
            //var luceneRead = iocFactory.Resolve<IUniversityReadSearchProvider>();
            //var x = luceneRead.SearchUniversity("פתוחה");
            //m_TableProvider = iocFactory.Resolve<ITableProvider>();
            //var words = m_TableProvider.GetFileterWored();

            //var m_FileProcessorFactory = iocFactory.Resolve<IFileProcessorFactory>();

            //var processor = m_FileProcessorFactory.GetProcessor(new Uri("http://127.0.0.1:10000/devstoreaccount1/zboxfiles/test.gif"));
            //var retVal2 = processor.PreProcessFile(new Uri("http://127.0.0.1:10000/devstoreaccount1/zboxfiles/test1.jpg")).Result;
            //var retVal = processor.PreProcessFile(new Uri("http://127.0.0.1:10000/devstoreaccount1/zboxfiles/test.gif")).Result;

            //m_ThumbnailProvider = iocFactory.Resolve<IThumbnailProvider>();

            //var m_BlobProvider = iocFactory.Resolve<IBlobProvider>();
            //var metaData = m_BlobProvider.FetechBlobMetaDataAsync("5c4b8f5a-87c0-40d8-8ab8-ffac3d676910.pdf").Result;
            //string content;
            //if (metaData.TryGetValue(StorageConsts.ContentMetaDataKey, out content))
            //{
            //    content = System.Net.WebUtility.UrlDecode(content);
            //}





            //var file = File.ReadAllBytes(@"C:\Users\Ram\Pictures\bug1.png");
            //var t = m_BlobProvider.UploadQuizImage(new MemoryStream(file), "image/png", 1, "bug1.png");
            //t.Wait();
            //var z = t.Result;
            //            var x = m_BlobProvider.GetThumbnailUrl("sometest");
            //            //TestImage();
            //            //var ShortCode = iocFactory.Resolve<IShortCodesCache>();
            //            //var boxid = ShortCode.LongToShortCode(10691, ShortCodesType.User);
            //var x = iocFactory.Resolve<IMailComponent>();
            //x.GenerateAndSendEmail(new[] { "ram@cloudents.com", "eidan@cloudents.com" },
            //         "failed connect to remove db ");
            //var t = x.DeleteUnsubscribe("yaari.ram@gmail.com");
            //t.Wait();
            //IZboxWorkerRoleService writeService = iocFactory.Resolve<IZboxWorkerRoleService>();
            //writeService.UpdateReputation(new UpdateReputationCommand(1));
            //writeService.AddNewUpdateAsync(new AddNewUpdatesCommand(
            //    21481,
            //   423791,
            //    Guid.Parse("645f4777-3583-4894-a9e7-a4c800a41ae9"),
            //   null,
            //    null,
            //   null
            //    )).Wait();
            //writeService.OneTimeDbi();
            //while (writeService.Dbi(0))
            //{

            //}

            //            //var command = new UpdateUserProfileCommand(1, "ramy", null, null, "המרכז האקדמי פרס");

            //            //writeService.Dbi();
            //            //writeService.Dbi();
            //            IZboxReadService readService = iocFactory.Resolve<IZboxReadService>();

            //            var tsuggestedUniversity = readService.GetUniversityListByFriendsIds(
            //                new List<long> {2415255, 
            //16410395, 
            //501556913, 
            //509546426, 
            //520131059, 
            //524093707, 
            //534377306, 
            //537311877, 
            //538061010, 
            //541778766, 
            //546252369, 
            //549554365, 
            //551208183, 
            //557197682, 
            //563025986, 
            //565339255, 
            //565579341, 
            //576042155, 
            //576136125, 
            //583308223, 
            //583605239, 
            //586077872, 
            //590640338, 
            //594610797, 
            //596713753, 
            //598935601, 
            //599847695, 
            //600242347, 
            //600427140, 
            //600538163, 
            //601726304, 
            //610363610, 
            //611024791, 
            //612147479, 
            //612709648, 
            //615190037, 
            //620825197, 
            //628237102, 
            //630468616, 
            //635359323, 
            //637528694, 
            //639580092, 
            //639861298, 
            //641043430, 
            //643866333, 
            //648792165, 
            //653359450, 
            //655606648, 
            //657031701, 
            //659152438, 
            //663424219, 
            //666088257, 
            //668221049, 
            //669981298, 
            //670858348, 
            //674916742, 
            //678716639, 
            //678727945, 
            //680907431, 
            //681382691, 
            //685626868, 
            //686575955, 
            //686708291, 
            //688865804, 
            //691436746, 
            //694788983, 
            //696380817, 
            //697562155, 
            //698973465, 
            //701038958, 
            //706243368, 
            //706448601, 
            //707844491, 
            //709058547, 
            //710755349, 
            //710983377, 
            //714895874, 
            //715805689, 
            //717571181, 
            //718387772, 
            //721588302, 
            //723218491, 
            //724339916, 
            //725508722, 
            //732516184, 
            //733617357, 
            //733643021, 
            //734794495, 
            //739787545, 
            //739833876, 
            //743574813, 
            //749459250, 
            //755817868, 
            //760718831, 
            //773152847, 
            //780158712, 
            //782180617, 
            //783230175, 
            //789739899, 
            //790414909, 
            //793169763, 
            //795829868, 
            //801288965, 
            //802773840, 
            //818367309, 
            //824939419, 
            //826602732, 
            //831953087, 
            //832312312, 
            //839612359, 
            //849274572, 
            //854584311, 
            //869790480, 
            //872095356, 
            //880320234, 
            //1004814080,
            //1007152252,
            //1014750275,
            //1024254015,
            //1039731016,
            //1041220808,
            //1049146080,
            //1066332232,
            //1068019067,
            //1068825252,
            //1087266539,
            //1121138404,
            //1149818070,
            //1176055037,
            //1223550129,
            //1228141127,
            //1271008083,
            //1297039630,
            //1320758730,
            //1349704417,
            //1427130900,
            //1447592555,
            //1470826181,
            //1490108754,
            //1529379320,
            //1579031784,
            //1593034569,
            //1634232277,
            //100000224363960,
            //100000236080092,
            //100000273987985,
            //100000274814266,
            //100000296529344,
            //100000343394828,
            //100000392745843,
            //100000494453685,
            //100000504883470,
            //100000542740567,
            //100000563240719,
            //100000658780812,
            //100000801106267,
            //100000892986329,
            //100000913998579,
            //100001042491048,
            //100001134337878,
            //100001231668431,
            //100001482960771,
            //100001489932429,
            //100001859106744,
            //100004013425764,
            //100004121160990});
            //            tsuggestedUniversity.Wait();

            //            var suggestedUniversity = tsuggestedUniversity.Result;
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

        private static async Task IndexItemSearch(Zbang.Zbox.Infrastructure.Ioc.IocFactory iocFactory)
        {
            IZboxReadServiceWorkerRole m_ZboxReadService = iocFactory.Resolve<IZboxReadServiceWorkerRole>();
            IItemWriteSearchProvider m_ItemSearchProvider = iocFactory.Resolve<IItemWriteSearchProvider>();
            var updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync(1, 1, 1);
            if (updates.ItemsToUpdate.Any() || updates.ItemsToDelete.Any())
            {
                await m_ItemSearchProvider.UpdateDataAsync(updates.ItemsToUpdate, updates.ItemsToDelete);
            }
        }

        private async static Task<string> TestMediaServices()
        {
            IFileProcessorFactory service = Zbang.Zbox.Infrastructure.Ioc.IocFactory.IocWrapper.Resolve<IFileProcessorFactory>();
            var processor = service.GetProcessor(new Uri("http://127.0.0.1:10000/devstoreaccount1/zboxfiles/7c8aaf28-e398-49f3-85ef-b9e9b04848a3.jpg"));
            var x = await processor.PreProcessFileAsync(new Uri("http://127.0.0.1:10000/devstoreaccount1/zboxfiles/7c8aaf28-e398-49f3-85ef-b9e9b04848a3.jpg"));
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



            var mail = new MailManager2();
           var x =  mail.GetUnsubscribesAsync(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),0);
            x.Wait();


            var t  = mail.GenerateAndSendEmailAsync("irena@cloudents.com",
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
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new InvitationToCloudentsMailParams("Eidan", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/401fe59e-1005-42a9-a97b-dc72f20abed4.jpg", new CultureInfo("he-IL"), "yaari.ram@gmail.com", null));

            //mail.GenerateAndSendEmail("noatseitlin@facebook.com", new WelcomeMailParams("ram", new CultureInfo("ru-RU")));
            // mail.GenerateAndSendEmailAsync("yaari.ram@gmail.com", new WelcomeMailParams("ram2", new CultureInfo("en-Us"))).Wait();
            //
            // mail.GenerateAndSendEmailAsync("yaari.ram@gmail.com", new ForgotPasswordMailParams2("hfgkjsdhf##askjd", "https://www.spitball.co", "ram", new CultureInfo("en-Us"))).Wait();
            // ////mail.GenerateAndSendEmail("yaari.ram@gmail.com", new ForgotPasswordMailParams2("h$$$sdhf##askjd", new CultureInfo("en-Us")));
            //
            // mail.GenerateAndSendEmailAsync("yaari.ram@gmail.com", new InviteMailParams("some user name", "some box name", "some box url", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg", new CultureInfo("he-IL"))).Wait();
            // //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new InviteMailParams("some user name", "some box name", "some box url", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg", new CultureInfo("en-Us")));
            //
            // mail.GenerateAndSendEmailAsync("yaari.ram@gmail.com", new MessageMailParams("some message", "some user name", new CultureInfo("he-IL"), "ram.y@outlook.com", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg")).Wait();
            ////mail.GenerateAndSendEmail("itsik.bitran@facebook.com", new MessageMailParams("some message", "some user name", new CultureInfo("en-Us"), "ram.y@outlook.com", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg"));


            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new MessageMailParams("some message", "some user name", new CultureInfo("he-IL"), "yaari_r@yahoo.com", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg"));
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new MessageMailParams("some message", "some user name", new CultureInfo("en-Us"), "yaari_r@yahoo.com", "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg"));

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
            //var t = mail.GenerateAndSendEmailAsync("yaari.ram@gmail.com", new UpdateMailParams(updates, new CultureInfo("ru-RU")));
            //t.Wait();
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new ChangeEmailMailParams("7656", new CultureInfo("ru-RU")));
            //mail.GenerateAndSendEmail("yaari.ram@gmail.com", new ChangeEmailMailParams("7656", new CultureInfo("en-Us")));

            //left new msg, update

            //var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;

            //IQueueProvider m_QueueProvider = iocFactory.Resolve<IQueueProvider>();
            //m_QueueProvider.InsertMessageToMailNew(new Zbang.Zbox.Infrastructure.Transport.WelcomeMailData("yaari.ram@gmail.com", "Ram Yaari", "en-Us"));

            //IMailManager mail = iocFactory.Resolve<IMailManager>();
            //IZboxReadService zboxService = iocFactory.Resolve<IZboxReadService>();
            //const string recipient = "yaari.ram@gmail.com";

            //var query1 = new GetBoxInviteDataQuery(11);
            //var boxData = zboxService.GetBoxDateForInvite(query1);

            //var parameters = new ForgotPassword("123qwe");
            //mail.SendEmail(parameters, recipient);
            //var parameters2 = new EmailVerification("VERY LONG HASSSSSSSSH", "Ram Yaaer");
            //mail.SendEmail(parameters2, recipient);
            //var parameters3 = new ChangeEmail(5000);
            //mail.SendEmail(parameters3, recipient);
            //var parameters4 = new ItemDeleted("This is the box", "This is an item name", "this is the user");
            //mail.SendEmail(parameters4, recipient);
            //var parameters5 = new Subscription("this is box", "lovely user");
            //mail.SendEmail(parameters5, recipient);
            //var parameters6 = new InviteToBox("Ram", "JOIN NOW!!!", boxData.Name,
            //                                 new Uri("https://ram.multimicloud.com/Box?BoxUid=lzodJqaBLkm"),
            //                                 boxData.UpdateTime, boxData.NumOfItems,
            //                                 boxData.Image);
            //mail.SendEmail(parameters6, recipient);

            //var parameters10 = new Updates("Ken", "ram", new Uri("https://ram.multimicloud.com/Box?BoxUid=lzodJqaBLkm"),
            //    new List<CommentDetails> { new CommentDetails("ram", TimeSpan.FromHours(5), "This is a cool image",
            //        "https://zboxstorage.blob.core.windows.net/zboxprofilepic/fd1de52d-859d-4815-a063-211fba152998")},
            //        new List<FileDetails> { new FileDetails("ram",TimeSpan.FromHours(5),
            //            "https://zboxstorage.blob.core.windows.net/zboxprofilepic/fd1de52d-859d-4815-a063-211fba152998",
            //            "test.png",
            //            "https://zboxstorage.blob.core.windows.net/zboxthumbnail/863fa055-1e48-4c0d-b99d-9879b8d61e7d.thumbnail.jpg")});
            //mail.SendEmail(parameters10, recipient);
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
            //mail.SendEmail(parameters7, recipient);
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
