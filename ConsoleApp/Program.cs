using Autofac;
using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.CommandHandler;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Storage;
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
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleApp
{
    internal static class Program
    {
        private static IContainer _container;
        private static CancellationToken token = CancellationToken.None;

        public static Random random = new Random();

        static async Task Main()
        {
            var builder = new ContainerBuilder();
            var keys = new ConfigurationKeys("https://www.spitball.co")
            {
                Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString, ConfigurationManager.AppSettings["Redis"]),
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"], true),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"],
                // ProdStorage = ConfigurationManager.AppSettings["OldStrageConnectionString"],
                LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200),
                BlockChainNetwork = "http://localhost:8545"
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();
            //builder.RegisterType<PPP>().As<IDataProtect>();
            builder.RegisterSystemModules(
                Cloudents.Core.Enum.System.Console,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                //Assembly.Load("Cloudents.Infrastructure.Data"),
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
            var _bus = _container.Resolve<ISearchServiceWrite<Cloudents.Core.Entities.Search.Document>>();
            await _bus.CreateOrUpdateAsync(default);
            // var sr = File.OpenRead(@"C:\Users\Ram\Downloads\file-148a9417-be00-431a-a6da-4e970fd0639c-625549.pdf");
            // var pdfProcessor = new PdfProcessor();
            //await pdfProcessor.ProcessFilesAsync(sr, (stream, s) => { return Task.CompletedTask; },
            //    s => { return Task.CompletedTask;}, i => { return Task.CompletedTask;}, token);
            //await ReduProcessing();
            //blobClient.ListBlobsSegmentedAsync("")
            //await _bus.UpdateNumberOfViews(1, default);
            //var z = _bus.PreviewFactory("dfjkhsfkjas.docx");

            //var ms = File.OpenRead(@"C:\Users\Ram\Downloads\file-b198fed1-4b9e-483e-b742-600d8f58ed84-601.docx");
            //ms.Seek(0, SeekOrigin.Begin);
            //await z.ProcessFilesAsync(ms, (stream, s) => Task.CompletedTask, sssssss =>
            //{
            //    Console.WriteLine(sssssss);
            //    return Task.CompletedTask;
            //}, i => Task.CompletedTask, token);

            //_bus.ProcessFilesAsync()
            //var query = new SyncAzureQuery(1, 0);

            //var (update, delete, version) =
            //    await _bus.QueryAsync<(IEnumerable<DocumentSearchDto> update, IEnumerable<string> delete, long version)>(query, token);


            // await _bus.QueryAsync(query, token);

            //(object update, object delete, object version) =
            //    await _bus.QueryAsync<(IEnumerable<QuestionSearchDto> update, IEnumerable<string> delete, long version)>(query, token);
        }

        private static async Task DoStuffToFiles(CloudBlobDirectory dir, Func<CloudBlockBlob, Task> func)
        {
            BlobContinuationToken blobToken = null;
            do
            {
                var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 5000, blobToken,
                    new BlobRequestOptions(),
                    new OperationContext(), default);

                Console.WriteLine("Receiving a new batch of blobs");
                foreach (IListBlobItem blob in result.Results)
                {
                    var blobToDelete = (CloudBlockBlob)blob;
                    await func(blobToDelete);

                    //foreach (var extension in WordProcessor.WordExtensions)
                    //{
                    //    //if (blob.Uri.AbsolutePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    //    //{
                    //    //    var blobToDelete = (CloudBlockBlob)blob;

                    //    //    await func(blobToDelete);
                    //    //    //Console.WriteLine("Deleting" + blobToDelete.Name);
                    //    //    //await blobToDelete.DeleteAsync();
                    //    //}
                    //}
                }

                blobToken = result.ContinuationToken;
            } while (blobToken != null);
        }

        private static async Task ReduProcessing()
        {
            var _bus = _container.Resolve<ICloudStorageProvider>();
            var blobClient = _bus.GetBlobClient();
            var queueClient = _bus.GetQueueClient();
            var container = blobClient.GetContainerReference("azure-webjobs-hosts");
            //azure-webjobs-hosts/blobreceipts/spitball-function-migration-dev/Cloudents.Functions.BlobMigration.Run/

            var dir = container.GetDirectoryReference(
                "blobreceipts/spitball-function-migration-dev/Cloudents.Functions.BlobMigration.Run/");

            //await DoStuffToFiles(dir, async blob =>
            //{
            //    foreach (var extension in WordProcessor.WordExtensions)
            //    {
            //        if (blob.Uri.AbsolutePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            //        {
            //            await blob.DeleteAsync();
            //        }
            //    }
            //});
            container = blobClient.GetContainerReference("spitball-files");
            dir = container.GetDirectoryReference("files");
            var queue = queueClient.GetQueueReference("generate-blob-preview");
            await DoStuffToFiles(dir, async blob =>
            {
                if (blob.Uri.Segments.Length != 5)
                {
                    return;
                }
                foreach (var extension in WordProcessor.WordExtensions)
                {
                    if (blob.Uri.AbsolutePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {

                        var id = blob.Uri.Segments[3].TrimEnd('/');

                        await queue.AddMessageAsync(new CloudQueueMessage(id));
                        Console.WriteLine($"Send queue message {id}");
                        //await blob.FetchAttributesAsync();
                        //int v = 0;
                        //if (blob.Metadata.TryGetValue("process", out var p) && int.TryParse(p, out v))
                        //{
                        //    v += 1;
                        //}

                        //blob.Metadata["process"] = v.ToString();
                        //await blob.SetMetadataAsync();
                        //await blob.DeleteAsync();
                    }
                }
            });


        }


        private static IEnumerable<string> SplitSentence(string input)
        {
            //TODO: Check environment newline
            return input.Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static readonly Regex SpaceReg = new Regex(@"\s+", RegexOptions.Compiled);


        static string StripUnwantedChars(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var z = Regex.Replace(input, "\\b(\\S)\\s+(?=\\S)", string.Empty);

            var eightOrNineDigitsId = new Regex(@"\b\d{8,9}\b", RegexOptions.Compiled);
            var result = SpaceReg.Replace(z, " ");
            result = eightOrNineDigitsId.Replace(result, string.Empty);
            result = new string(result.Where(w => char.IsLetterOrDigit(w) || char.IsWhiteSpace(w)).ToArray());
            //result = result.Replace("\0", string.Empty);
            result = result.Replace("בס\"ד", string.Empty);
            return result.Replace("find more resources at oneclass.com", string.Empty);
        }




        static IEnumerable<CultureInfo> FindCandidateCultures(RegionInfo region)
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Where(x => (new RegionInfo(x.Name)).GeoId == region.GeoId);
        }

        private static async Task HadarMethod()
        {

            await TransferUsers();
            //var t = _container.Resolve<IBlockChainErc20Service>();
            //string spitballServerAddress = "0xc416bd3bebe2a6b0fea5d5045adf9cb60e0ff906";


            //string spitballServerPK1 = "53d8ca027b5689e062c0e461b4aee444676fdf21d23ca73b8e1da1bc9b03bedf";
            //string spitballServerPK2 = "dc48f1874948869118757457827d7ca4efe8bce63730053f25b25c5b979efbca";
            //string spitballServerPK3 = "78cdd8076ccd67e1b64bb46eddad6bb7647da141bf3d870529a1f84169a4dd79";
            //string spitballServerPK4 = "ff755a496156cd42544d436f191f5842c3295d3a44623561c46a7fe983ae1f06";
            //string spitballServerPK5 = "5f271ce762fbcbe6a6e5472bf7c0ac12562f3e1fdb352be1f05b785eef8989a5";
            //string spitballServerPK6 = "d9f1ee276e00316679986b157dc66ce99f47400094157bea6381c822c88bdadb";
            //string spitballServerPK7 = "30ffb3e064f5aad2f602d9830dc838d12ceec5236700998566132de02866d512";
            //string spitballServerPK8 = "31281982d9965bae7597c59404b199a8f4d25b2e8070ef9a79dea8b4c01deb50";
            //string spitballServerPK9 = "9438658b938ab1767c3d1577a4999558cd0b34d640b978f4577659383f6c115e";
            //string spitballServerPK10 = "26633b6a2bc76ba070c9813edbf721d206da795482d0f26f4c63f7a4212ee456";
            //string spitballServerPK11 = "c4e6dfbb4ae587042b8f2311148b5b112173093a535ebc83f61cfd242d07c5bb";
            //string spitballServerPK12 = "3ee29eeae187be9ad7120896797e62035169905e27a65af6c3dfc95c3b6ac578";
            //string spitballServerPK13 = "a4312bacc19689368154aef36985218b676264bf0d9a9ba24479c28f7a6cde41";
            //string spitballServerPK14 = "7d2712441d99f21299162afde3c2515674725d4807baa23873b85926a315c6ed";
            //string spitballServerPK15 = "533f69ee5e39af47e3852d4851abfd43b0d162391c6b00124c4f4e0395a49a96";
            //string spitballServerPK16 = "84f5b526b4b82f9a8883d8b0a048925b936a9ce1487835a4d0cc58f325c17403";
            //string spitballServerPK17 = "a034ef0252030f6cf6af8941c03c0414a6766c2cb247e4c2580404ee9f34d620";
            //string spitballServerPK18 = "5be05be0da379bd16f62932a89eba920d7aec0ca5f178919fe08a4f830c079c0";
            //string spitballServerPK19 = "707fa6cf9416cf40bfc41bab94cd14b8002046f5c894373867097f4218f0710d";
            //string spitballServerPK20 = "58f3e1dcfa9c919c0d1f5aa41f31e96f72324b8adb819ffb500e0bc71cb3d17c";

            ///*
            //string metaMaskPK = "10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4";



            //var b1 = await t.GetBalanceAsync(metaMaskAddress, default);
            //var b2 = await t.GetBalanceAsync(spitballServerAddress, default);
            //Console.WriteLine($"metaMaskAddress Balance: {b1}");
            //Console.WriteLine($"spitballServerAddress Balance: {b2}");
            //*/

            ////Task<Task> TxHash;

            //var account = new Account(spitballServerPK1);
            //var web3 = new Nethereum.Web3.Web3(account);

            ///* await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd2, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd3, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd4, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd5, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd6, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd7, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd8, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd9, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd10, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd11, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd12, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd13, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd14, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd15, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd16, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd17, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd18, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd19, new HexBigInteger(10000000000000000000));
            // await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd20, new HexBigInteger(10000000000000000000));
            // */

            ///*var wl1 = await t.WhitelistUserForTransfers(SbAdd1, default);
            //var wl2 = await t.WhitelistUserForTransfers(SbAdd2, default);
            //var wl3 = await t.WhitelistUserForTransfers(SbAdd3, default);
            //var wl4 = await t.WhitelistUserForTransfers(SbAdd4, default);
            //var wl5 = await t.WhitelistUserForTransfers(SbAdd5, default);
            //var wl6 = await t.WhitelistUserForTransfers(SbAdd6, default);
            //var wl7 = await t.WhitelistUserForTransfers(SbAdd7, default);
            //var wl8 = await t.WhitelistUserForTransfers(SbAdd8, default);
            //var wl9 = await t.WhitelistUserForTransfers(SbAdd9, default);
            //var wl10 = await t.WhitelistUserForTransfers(SbAdd10, default);
            //var wl11 = await t.WhitelistUserForTransfers(SbAdd11, default);
            //var wl12 = await t.WhitelistUserForTransfers(SbAdd12, default);
            //var wl13 = await t.WhitelistUserForTransfers(SbAdd13, default);
            //var wl14 = await t.WhitelistUserForTransfers(SbAdd14, default);
            //var wl15 = await t.WhitelistUserForTransfers(SbAdd15, default);
            //var wl16 = await t.WhitelistUserForTransfers(SbAdd16, default);
            //var wl17 = await t.WhitelistUserForTransfers(SbAdd17, default);
            //var wl18 = await t.WhitelistUserForTransfers(SbAdd18, default);
            //var wl19 = await t.WhitelistUserForTransfers(SbAdd19, default);
            //var wl20 = await t.WhitelistUserForTransfers(SbAdd20, default);
            //*/

            //var d = await t.GetBalanceAsync(spitballServerAddress, default);
            //Console.WriteLine($"spitballServerAddress Balance: {d}");
            //ConcurrentQueue<string> staticQ = new ConcurrentQueue<string>();

            //staticQ.Enqueue(spitballServerPK1);
            //staticQ.Enqueue(spitballServerPK2);
            //staticQ.Enqueue(spitballServerPK3);
            //staticQ.Enqueue(spitballServerPK4);
            //staticQ.Enqueue(spitballServerPK5);
            //staticQ.Enqueue(spitballServerPK6);
            //staticQ.Enqueue(spitballServerPK7);
            //staticQ.Enqueue(spitballServerPK8);
            //staticQ.Enqueue(spitballServerPK9);
            //staticQ.Enqueue(spitballServerPK10);
            //staticQ.Enqueue(spitballServerPK11);
            //staticQ.Enqueue(spitballServerPK12);
            //staticQ.Enqueue(spitballServerPK13);
            //staticQ.Enqueue(spitballServerPK14);
            //staticQ.Enqueue(spitballServerPK15);
            //staticQ.Enqueue(spitballServerPK16);
            //staticQ.Enqueue(spitballServerPK17);
            //staticQ.Enqueue(spitballServerPK18);
            //staticQ.Enqueue(spitballServerPK19);
            //staticQ.Enqueue(spitballServerPK20);



            //for (int i = 0; i < 200; i++)
            //{


            //    ConcurrentQueue<string> blockQ = new ConcurrentQueue<string>(staticQ);

            //    string res;
            //    blockQ.TryDequeue(out res);
            //    var tx1 = t.TransferPreSignedAsync(res, spitballServerPK1, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx2 = t.TransferPreSignedAsync(res, spitballServerPK2, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx3 = t.TransferPreSignedAsync(res, spitballServerPK3, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx4 = t.TransferPreSignedAsync(res, spitballServerPK4, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx5 = t.TransferPreSignedAsync(res, spitballServerPK5, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx6 = t.TransferPreSignedAsync(res, spitballServerPK6, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx7 = t.TransferPreSignedAsync(res, spitballServerPK7, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx8 = t.TransferPreSignedAsync(res, spitballServerPK8, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx9 = t.TransferPreSignedAsync(res, spitballServerPK9, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx10 = t.TransferPreSignedAsync(res, spitballServerPK10, spitballServerAddress, 2, 1, default);

            //    blockQ.TryDequeue(out res);
            //    var tx11 = t.TransferPreSignedAsync(res, spitballServerPK11, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx12 = t.TransferPreSignedAsync(res, spitballServerPK12, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx13 = t.TransferPreSignedAsync(res, spitballServerPK13, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx14 = t.TransferPreSignedAsync(res, spitballServerPK14, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx15 = t.TransferPreSignedAsync(res, spitballServerPK15, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx16 = t.TransferPreSignedAsync(res, spitballServerPK16, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx17 = t.TransferPreSignedAsync(res, spitballServerPK17, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx18 = t.TransferPreSignedAsync(res, spitballServerPK18, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx19 = t.TransferPreSignedAsync(res, spitballServerPK19, spitballServerAddress, 2, 1, default);
            //    blockQ.TryDequeue(out res);
            //    var tx20 = t.TransferPreSignedAsync(res, spitballServerPK20, spitballServerAddress, 2, 1, default);


            //    await Task.WhenAll(tx1, tx2, tx3, tx4, tx5, tx6, tx7, tx8, tx9, tx10
            //                        , tx11, tx12, tx13, tx14, tx15, tx16, tx17, tx18, tx19, tx20).ConfigureAwait(false);

            //    d = await t.GetBalanceAsync(spitballServerAddress, default);
            //    Console.WriteLine($"spitballServerAddress Balance: {d}");
            //    Console.WriteLine(i);
            //    Console.WriteLine($"---------------------------------");
            //    Thread.Sleep(3000);
            //}
        }

        private static string TTTT(string input, IEnumerable<string> BAD_WORDS)
        {
            return string.Join(" ",
                input.Split(' ').Select(w => BAD_WORDS.Contains(w) ? "" : w));
        }
        public static Task SendMoneyAsync()
        {
            var t = _container.Resolve<IBlockChainErc20Service>();
            var pb = t.GetAddress("38d68c294410244dcd009346c756436a64530d7ddb0611e62fa79f9f721cebb0");
            return t.SetInitialBalanceAsync(pb, default);
        }

        public async static Task<string> MintTokens(string address, IBlockChainErc20Service t)
        {
            return await t.MintNewTokens(address, 100, default);
        }


        internal static bool TryParseAddress(string value)
        {
            // email = null;

            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            try
            {
                // MailAddress will auto-parse the name from a string like "testuser@test.com <Test User>"
                MailAddress mailAddress = new MailAddress(value);
                string displayName = string.IsNullOrEmpty(mailAddress.DisplayName) ? null : mailAddress.DisplayName;
                //email = new Email(mailAddress.Address, displayName);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        //private static async Task PopulateSheetOfQuestion()
        //{
        //    string spreadsheetId = "1A2O_jASZuWlI_jIX8a1eiZb61C5RDF9KQ2i7CQzGU30";
        //    string range = "All!B:D";


        //    //var subjectList = new List<CreateQuestionCommand>();
        //    var subjectList = GoogleSheets.GetData(spreadsheetId, range);
        //    foreach (var question in GoogleSheets.GetData(spreadsheetId, range))
        //    {
        //        var commandBus = _container.Resolve<ICommandBus>();
        //        // await commandBus.DispatchAsync(question, default);
        //    }
        //}


        /// <summary>
        /// This is dbi for update question language
        /// </summary>
        /// <returns></returns>
        public static async Task UpdateLanguageAsync()
        {
            var t = _container.Resolve<ITextAnalysis>();
            var i = 0;
            bool continueLoop = false;
            do
            {
                using (var child = _container.BeginLifetimeScope())
                {


                    using (var unitOfWork = child.Resolve<IUnitOfWork>())
                    {
                        var repository = child.Resolve<IQuestionRepository>();
                        var questions = await repository.GetAllQuestionsAsync(i).ConfigureAwait(false);
                        continueLoop = questions.Count > 0;
                        var result = await t.DetectLanguageAsync(
                            questions.Where(w => w.Language == null)
                                .Select(s => new KeyValuePair<long, string>(s.Id, s.Text)), default);

                        foreach (var pair in result.Where(w => !w.Value.Equals(CultureInfo.InvariantCulture)))
                        {
                            var q = await repository.LoadAsync(pair.Key, default);

                            q.SetLanguage(pair.Value);

                            await repository.UpdateAsync(q, default);
                        }

                        await unitOfWork.CommitAsync(default).ConfigureAwait(false);
                    }

                }

                i++;
            } while (continueLoop);

        }

        public static async Task TransferUniversities()
        {
            var d = _container.Resolve<DapperRepository>();
            var z = await d.WithConnectionAsync<IEnumerable<dynamic>>(async f =>
            {
                return await f.QueryAsync(
                    @"SELECT u.UniversityName,u.Country,u.Extra
                FROM(SELECT id, u.UniversityName, u.Country, u.Extra,
                ROW_NUMBER() OVER(PARTITION BY u.UniversityName, u.Country order by id) as cnt
                    FROM zbox.University u
                where isdeleted = 0
                    ) u
                WHERE cnt = 1");
            }, default);

            using (var child = _container.BeginLifetimeScope())
            {
                using (var unitOfWork = child.Resolve<IUnitOfWork>())
                {
                    var repository = child.Resolve<IUniversityRepository>();

                    foreach (var pair in z)
                    {
                        var university = new University(pair.UniversityName, pair.Country)
                        {
                            Extra = pair.Extra
                        };
                        await repository.AddAsync(university, default);
                    }

                    await unitOfWork.CommitAsync(default).ConfigureAwait(false);
                }

            }


        }

        public static async Task TransferUsers()
        {
            var d = _container.Resolve<DapperRepository>();


            var z = await d.WithConnectionAsync<IEnumerable<dynamic>>(async f =>
            {

                return await f.QueryAsync(
                    @"select top 500 UserId
		                    ,ZU.Email
		                    --,U.[UniversityName]
							--,u.[Country]
		                    ,ZU.Culture
                      from zbox.Users ZU
					  join [Zbox].[University] U
						on U.Id = ZU.UniversityId
                      where LastAccessTime > DATEADD(YEAR,-2,GETDATE())  
	                    and ZU.Email not in (select Email from sb.[User] where Email = ZU.Email)
	                    and Email like '%@%'
	                    and ZU.Email not like '%facebook.com'
						and IsEmailVerified = 1; 
                ");
            }, default);

            if (z.Count() == 0)
            { return; }

            using (var child = _container.BeginLifetimeScope())
            {
                using (var unitOfWork = child.Resolve<IUnitOfWork>())
                {
                    var repository = child.Resolve<IUserRepository>();
                    var erc = _container.Resolve<IBlockChainErc20Service>();


                    foreach (var pair in z)
                    {

                        var name = pair.Email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        var (privateKey, _) = erc.CreateAccount();

                        CultureInfo cultur = new CultureInfo(pair.Culture);

                        var user = new User(pair.Email, $"{name}.{random.Next(1000, 9999)}", privateKey, cultur)
                        {
                            // EmailConfirmed = true,
                            LockoutEnabled = true,
                            NormalizedEmail = pair.Email.ToUpper(),
                            OldUser = true
                        };
                        user.NormalizedName = user.Name.ToUpper();
                        await repository.AddAsync(user, default);
                    }

                    await unitOfWork.CommitAsync(default).ConfigureAwait(false);

                }

            }
            await TransferUsers();
        }

        public static async Task TransferDocumants()
        {
            var d = _container.Resolve<DapperRepository>();


            var key = ConfigurationManager.AppSettings["OldStrageConnectionString"];
            var productionOldstorageAccount = CloudStorageAccount.Parse(key);
            var oldBlobClient = productionOldstorageAccount.CreateCloudBlobClient();
            var oldContainer = oldBlobClient.GetContainerReference("zboxfiles");



            var keyNew = _container.Resolve<IConfigurationKeys>().Storage;
            var storageAccount = CloudStorageAccount.Parse(keyNew);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("spitball-files/files");

            //CloudBlobContainer directoryToPutFiles = container.Get .GetDirectoryReference("./files");

            Dictionary<int, DocumentType> DocType = new Dictionary<int, DocumentType>
            {
                { 1, DocumentType.Exam },
                { 2, DocumentType.Exam },
                { 7, DocumentType.Exam },
                { 8, DocumentType.Exam },
                { 9, DocumentType.Lecture },
                { 10, DocumentType.Lecture},
                { 5, DocumentType.Textbook }
            };

            string[] supportedFiles = { "doc", "docx", "xls", "xlsx", "PDF", "png", "jpg", "ppt", "ppt", "jpg", "png", "gif", "jpeg", "bmp" };


            var z = await d.WithConnectionAsync<IEnumerable<dynamic>>(async f =>
            {

                return await f.QueryAsync(
                    @"select top 1 I.ItemId, I.BlobName, I.Name,  B.BoxName, U.Id, B.ProfessorName, ISNULL(I.DocType,0) as DocType, I.NumberOfViews + I.NumberOfDownloads as [Views], I.CreationTime,
			            STRING_AGG((T.Name), ',') as Tags
                        FROM [Zbox].[Item] I
                        join zbox.Box B
	                        on I.BoxId = B.BoxId
                        join Zbox.Users ZU
	                        on I.UserId = ZU.UserId
                        join sb.[User] U
	                        on ZU.Email = U.Email
						left join zbox.ItemTag IT
							on IT.ItemId = I.ItemId
						left join zbox.Tag T
							on IT.TagId = T.Id and len(T.Name) >= 4
                        where I.Discriminator = 'File'
	                        and I.IsDeleted = 0 
							and I.ItemId not in (select D.OldId from sb.Document D where I.ItemId = D.OldId)
							and len(BoxName) <= 150
							and SUBSTRING (I.Name,CHARINDEX ('.',I.Name), 5) in ('.doc', '.docx', '.xls', '.xlsx', '.PDF', '.png', '.jpg', '.ppt', '.ppt', '.jpg', '.png', '.gif', '.jpeg', '.bmp' )
                        group by I.ItemId, I.BlobName, I.Name,  B.BoxName, U.Id, B.ProfessorName, ISNULL(I.DocType,0),I.NumberOfViews + I.NumberOfDownloads, I.CreationTime
                        order by I.ItemId desc;
                ");
            }, default);

            if (z.Count() == 0)
            { return; }


            using (var child = _container.BeginLifetimeScope())
            {

                var commandBus = child.Resolve<ICommandBus>();
                var session = child.Resolve<IStatelessSession>();


                foreach (var pair in z)
                {
                    string[] blobName = pair.BlobName.Split('.');
                    if (!supportedFiles.Contains(blobName[1], StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    CloudBlockBlob blobDestination = container.GetBlockBlobReference($"file-{blobName[0]}-{pair.ItemId}.{blobName[1]}");

                    CloudBlockBlob srcBlob = oldContainer.GetBlockBlobReference(pair.BlobName);

                    var sharedAccessUri = GetShareAccessUri(pair.BlobName, 360, oldContainer);

                    var blobUri = new Uri(sharedAccessUri);





                    await blobDestination.StartCopyAsync(blobUri).ConfigureAwait(false);
                    while (blobDestination.CopyState.Status != CopyStatus.Success)
                    {
                        Console.WriteLine(blobDestination.CopyState.Status);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        await blobDestination.ExistsAsync();
                    }


                    string[] words;
                    if (pair.Tags != null)
                    {
                        words = pair.Tags.Split(',');
                    }
                    else { words = null; }
                    DocumentType type;

                    if (DocType.ContainsKey(pair.DocType))
                    {
                        DocType.TryGetValue(pair.DocType, out type);
                    }
                    else { type = DocumentType.None; }



                    var command = new CreateDocumentCommand($"file-{blobName[0]}-{pair.ItemId}.{blobName[1]}",
                        pair.Name,
                    type, pair.BoxName, words, pair.Id, pair.ProfessorName);

                    await commandBus.DispatchAsync(command, default);

                    int views = pair.Views;
                    long? itemId = pair.ItemId;
                    DateTime updateTime = pair.CreationTime;

                    var doc = session.Query<Document>().Where(w => w.Id == command.Id)
                            .UpdateBuilder()
                            .Set(x => x.Views, x => views)
                            .Set(x => x.OldId, x => itemId)
                            .Set(x => x.TimeStamp.UpdateTime, x => updateTime)
                            .Update();

                }

            }
            await TransferDocumants();
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

        //private static int GenerateRandomNumber()
        //{
        //    var rdm = new Random();
        //    return rdm.Next(1000, 9999);
        //}

        public static async Task MigrateUniversity()
        {
            var d = _container.Resolve<DapperRepository>();

            var z = await d.WithConnectionAsync<IEnumerable<dynamic>>(async f =>
            {

                return await f.QueryAsync(
                    @"
                  select top 1000 U.Id, Un.UniversityName, Un.Country
                        from sb.[User] U
						join zbox.Users ZU
							on U.Email = ZU.Email
                        join [Zbox].[University] Un
                            on Un.Id = ZU.UniversityId
                        where LastAccessTime > DATEADD(YEAR,-2,GETDATE())  
                            and U.Email like '%@%'
                            and ZU.Email not like '%facebook.com'
                            and IsEmailVerified = 1 and U.UniversityId2 is null
                            and Un.isdeleted = 0; 
                  ");
            }, default);

            if (z.Count() == 0)
            { return; }

            using (var child = _container.BeginLifetimeScope())
            {
                var repository = child.Resolve<IUserRepository>();
                var uni = child.Resolve<IUniversityRepository>();
                using (var unitOfWork = child.Resolve<IUnitOfWork>())
                {
                    var ch = new AssignUniversityToUserCommandHandler(repository, uni);
                    List<Task> taskes = new List<Task>();
                    foreach (var pair in z)
                    {
                        var t = new AssignUniversityToUserCommand(pair.Id, pair.UniversityName, pair.Country);
                        taskes.Add(ch.ExecuteAsync(t, default));
                    }
                    await Task.WhenAll(taskes);
                    await unitOfWork.CommitAsync(default).ConfigureAwait(false);
                }
            }
            await MigrateUniversity();
        }


        //public class PPP : IDataProtect
        //{
        //    public string Protect(string purpose, string plaintext, DateTimeOffset expiration)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public string Unprotect(string purpose, string protectedData)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}




    }



}
