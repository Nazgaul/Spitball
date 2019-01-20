﻿using Autofac;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Query.Query.Admin;
using Cloudents.Query.Stuff;
using Cloudents.Search;
using Dapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Votes.Commands.AddVoteQuestion;
using Cloudents.Core.Enum;
using DocumentType = Cloudents.Core.Enum.DocumentType;

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
                Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                    ConfigurationManager.AppSettings["Redis"]),
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"], false),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionStringProd"],
                LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200),
                BlockChainNetwork = "http://localhost:8545",
                ServiceBus = ConfigurationManager.AppSettings["ServiceBus"]
            };
            

            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterAssemblyModules(Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Persistance"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Search"),
                Assembly.Load("Cloudents.Core"));
            //    Cloudents.Core.Enum.System.Console,
            //    Assembly.Load("Cloudents.Infrastructure.Framework"),
            //    Assembly.Load("Cloudents.Infrastructure.Storage"),
            //    Assembly.Load("Cloudents.Infrastructure"),
            //    Assembly.Load("Cloudents.Core"));
            builder.RegisterModule<ModuleFile>();
            //var module = new SearchModule(ConfigurationManager.AppSettings["AzureSearchServiceName"],
            //    ConfigurationManager.AppSettings["AzureSearchKey"], false);

            //builder.RegisterModule(module);




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

        public class Order
        {
            public int Id { get; set; }
        }

        public class OrderDto
        {

        }

        private static async Task RamMethod()
        {
           // var service1 = _container.Resolve<IUnitOfWork>();
            var service = _container.Resolve<IQueryBus>();
            var query = new GetDocumentPurchasedEmail(Guid.Parse("439B602A-421F-40F8-8A97-A9C60102D069"));
            var z = await service.QueryAsync(query, default);
            //var t = await service.GetEmailAsync(SystemEvent.DocumentPurchased, Language.English, default);
        }

       
        private static async Task ReduDocument()
        {
            var service = _container.Resolve<IStatelessSession>();
            var storage = _container.Resolve<ICloudStorageProvider>();
            var queueClient = storage.GetQueueClient();
            var queue = queueClient.GetQueueReference("generate-search-preview");


            var i = 0L;
            var cont = false;
            do
            {
                var i1 = i;
                var itemIds = await service.Query<Document>().Where(w => w.Id > i1)
                    .Take(100).OrderBy(o => o.Id).Select(s => s.Id).ToListAsync();

                var t = new List<Task>();
                foreach (var itemId in itemIds)
                {
                    var msg = new CloudQueueMessage(itemId.ToString());
                    t.Add(queue.AddMessageAsync(msg));
                    i = itemId;
                }

                await Task.WhenAll(t);
                cont = itemIds.Count > 0;

            } while (cont);


            //var service = _container.Resolve<SearchServiceWrite<Cloudents.Search.Entities.Document>>();
            //await service.CreateOrUpdateAsync(default);
        }

        private static async Task FixPoisonBackground(ICommandBus _commandBus)
        {
            var storage = _container.Resolve<ICloudStorageProvider>();
            var queueClient = storage.GetQueueClient();
            var queue = queueClient.GetQueueReference("background-poison");
            var newQueue = queueClient.GetQueueReference("background");

            do
            {
                var msg = queue.GetMessage();
                if (msg == null)
                {
                    break;
                }

                if (msg.AsString.Contains("Cloudents.Core.Message.System.AddUserTagMessage, Cloudents.Core"))
                {
                    try
                    {
                        var sMsg =
                            JsonConvert
                                .DeserializeObject<AddUserTagMessage>(msg.AsString);
                        if (!Tag.ValidateTag(sMsg.Tag))
                        {
                            queue.DeleteMessage(msg);
                            continue;
                        }

                        var command2 = new AddUserTagCommand(sMsg.UserId, sMsg.Tag);
                        await _commandBus.DispatchAsync(command2, token);
                        queue.DeleteMessage(msg);
                    }
                    catch (ArgumentException)
                    {
                        queue.DeleteMessage(msg);
                    }
                    catch (UserLockoutException)
                    {
                        queue.DeleteMessage(msg);
                    }
                }

                if (msg.AsString.Contains("Cloudents.Core.Message.System.UpdateUserBalanceMessage"))
                {
                    queue.DeleteMessage(msg);
                }
                else
                {
                    newQueue.AddMessage(new CloudQueueMessage(msg.AsString));
                    queue.DeleteMessage(msg);
                }
            } while (true);
        }



        private static async Task FixFilesAsync()
        {
            var d = _container.Resolve<DapperRepository>();



            IEnumerable<dynamic> result = await d.WithConnectionAsync(async f =>
            {
                return await f.QueryAsync(
                    @"select  id,blobname from  sb.document 
where left(blobName ,4) != 'file'");
            }, default);

            var _bus = _container.Resolve<ICloudStorageProvider>();
            var blobClient = _bus.GetBlobClient();
            var container = blobClient.GetContainerReference("spitball-files");
            var dir = container.GetDirectoryReference("files");
            foreach (var item in result)
            {
                CloudBlobDirectory itemDir = dir.GetDirectoryReference(item.id.ToString());
                CloudBlockBlob blob = itemDir.GetBlockBlobReference(item.blobname);
                var blobCopy = itemDir.GetBlockBlobReference($"file-{item.blobname}");

                await blobCopy.StartCopyAsync(blob);
                await blob.DeleteIfExistsAsync();
            }



        }

        private static async Task ReduWordProcessing()
        {



            var _bus = _container.Resolve<ICloudStorageProvider>();
            var blobClient = _bus.GetBlobClient();
            var queueClient = _bus.GetQueueClient();
            //var container = blobClient.GetContainerReference("spitball-files");
            //azure-webjobs-hosts/blobreceipts/spitball-function-migration-dev/Cloudents.Functions.BlobMigration.Run/

            //var dir = container.GetDirectoryReference(
            //    "blobreceipts/spitball-function-migration-dev/Cloudents.Functions.BlobMigration.Run/");


            var container = blobClient.GetContainerReference("spitball-files");
            var dir = container.GetDirectoryReference("files");
            var queue = queueClient.GetQueueReference("generate-blob-preview");
            var extensions = WordProcessor.Extensions;


            BlobContinuationToken blobToken = null;
            do
            {
                var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 5000, blobToken,
                    new BlobRequestOptions(),
                    new OperationContext(), default);

                Console.WriteLine("Receiving a new batch of blobs");
                foreach (IListBlobItem blob in result.Results)
                {

                    var fileNameWithoutDirectory = blob.Parent.Uri.MakeRelativeUri(blob.Uri).ToString();

                    if (fileNameWithoutDirectory.StartsWith("file-", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var extension in extensions)
                        {
                            if (Path.GetExtension(fileNameWithoutDirectory) == extension)
                            {
                                var id = blob.Uri.Segments[3].TrimEnd('/');
                                var msg = new CloudQueueMessage(id);
                                await queue.AddMessageAsync(msg);
                                // var blobToDelete = (CloudBlockBlob)blob;
                            }
                        }


                        //var blobToDelete = (CloudBlockBlob)blob;
                    }

                    if (fileNameWithoutDirectory.EndsWith("svg") && fileNameWithoutDirectory.StartsWith("preview"))
                    {
                        var blobToDelete = (CloudBlockBlob) blob;
                        await blobToDelete.DeleteAsync();
                    }
                }

                blobToken = result.ContinuationToken;
            } while (blobToken != null);




        }

        //private static async Task CheckSync()
        //{
        //    var str =
        //        "  {\"$type\":\"Cloudents.Core.Message.System.DocumentSearchMessage, Cloudents.Core\",\"Document\":{\"$type\":\"Cloudents.Core.Entities.Search.Document, Cloudents.Core\",\"Id\":\"52\",\"Name\":\"מבחן תורת הקבוצות א 003.jpg\",\"MetaContent\":null,\"Content\":null,\"Tags\":{\"$type\":\"System.String[], mscorlib\",\"$values\":[]},\"Course\":\"תורת הקבוצות א\",\"Country\":\"IL\",\"Language\":null,\"University\":\"6c19649e-1c27-4095-91e6-a99c008af82e\",\"DateTime\":\"2018-11-19T13:10:46.1653245Z\",\"Type\":0}}";


        //    var val = JsonConvert.DeserializeObject<Cloudents.Core.Message.System.DocumentSearchMessage>(str);
        //    val.Document.Type = DocumentType.Lecture;
        //    var elem = _container.Resolve<ISearchServiceWrite<Cloudents.Core.Entities.Search.Document>>();

        //    await elem.UpdateDataAsync(new[] { val.Document }, default);
        //}


        private static IEnumerable<string> SplitSentence(string input)
        {
            //TODO: Check environment newline
            return input.Split(new[] {"\r\n\r\n"}, StringSplitOptions.RemoveEmptyEntries);
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


        private static async Task HadarMethod()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ConfigureMapper>();
            });
            var queryBus = _container.Resolve<IQueryBus> ();
            
            var questions = await queryBus.QueryAsync<IEnumerable<DocumentFeedDto>>(new UserPurchaseDocumentByIdQuery(159907, 1), token);

            foreach (var item in questions)
            {
                Console.WriteLine(item.Id);
            }


            //await MigrateDelta();

            //await MigrateUniversity();
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



        /// <summary>
        /// This is dbi for update question language
        /// </summary>
        /// <returns></returns>
        //public static async Task UpdateLanguageAsync()
        //{
        //    var t = _container.Resolve<ITextAnalysis>();
        //    bool continueLoop = false;
        //    do
        //    {
        //        using (var child = _container.BeginLifetimeScope())
        //        {

        //            var sts = child.Resolve<IStatelessSession>();

        //            using (var unitOfWork = child.Resolve<IUnitOfWork>())
        //            {
        //                var repository = child.Resolve<IRepository<Answer>>();
        //                var questions = await sts.Query<Answer>().Where(w => w.Language == null).Take(100)
        //                    .OrderBy(o => o.Id)
        //                    .ToListAsync(); // repository.GetAllQuestionsAsync(i).ConfigureAwait(false);
        //                continueLoop = questions.Count > 0;
        //                if (!continueLoop)
        //                {
        //                    break;
        //                }

        //                var result = await t.DetectLanguageAsync(
        //                    questions.Where(w => w.Language == null)
        //                        .Select(s => new KeyValuePair<string, string>(s.Id.ToString(), s.Text)), default);

        //                foreach (var pair in result.Where(w => !w.Value.Equals(CultureInfo.InvariantCulture)))
        //                {
        //                    var q = await repository.LoadAsync(Guid.Parse(pair.Key), default);


        //                    q.SetLanguage(pair.Value);

        //                    await repository.UpdateAsync(q, default);
        //                }

        //                await unitOfWork.CommitAsync(default);
        //            }

        //        }

        //        //i++;
        //    } while (continueLoop);

        //}

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
                    and u.UniversityName not in (select name from sb.University)
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

      //  public static async Task TransferUsers()
      //  {
      //      var d = _container.Resolve<DapperRepository>();
      //      var erc = _container.Resolve<IBlockChainErc20Service>();


      //      do
      //      {
      //          var z = await d.WithConnectionAsync<IEnumerable<dynamic>>(async f =>
      //          {
      //              return await f.QueryAsync(
      //                  @"select top 200 UserId
		    //                ,ZU.Email
		    //                ,ZU.Culture
      //                from zbox.Users ZU
					 // join [Zbox].[University] U
						//on U.Id = ZU.UniversityId
      //                where LastAccessTime > DATEADD(YEAR,-2,GETDATE())  
	     //               and ZU.Email not in (select Email from sb.[User] where Email = ZU.Email)
	     //               and Email like '%@%'
	     //               and ZU.Email not like '%facebook.com'
						//and IsEmailVerified = 1;  
      //          ");
      //          }, default);

      //          if (z.Count() == 0)
      //          {
      //              break;
      //              //return;
      //          }

      //          using (var child = _container.BeginLifetimeScope())
      //          {
      //              try
      //              {
      //                  using (var unitOfWork = child.Resolve<IUnitOfWork>())
      //                  {
      //                      var repository = child.Resolve<IRegularUserRepository>();



      //                      foreach (var pair in z)
      //                      {
      //                          Console.WriteLine($"Processing id {pair.UserId}");
      //                          var name = pair.Email.Split(new[] {'.', '@'}, StringSplitOptions.RemoveEmptyEntries)[0];
      //                          var (privateKey, _) = erc.CreateAccount();

      //                          CultureInfo cultur = new CultureInfo(pair.Culture);

      //                          var user = new RegularUser(pair.Email, $"{name}.{random.Next(1000, 9999)}", privateKey,
      //                              cultur)
      //                          {
      //                              // EmailConfirmed = true,
      //                              LockoutEnabled = true,
      //                              NormalizedEmail = pair.Email.ToUpper(),
      //                              OldUser = true
      //                          };
      //                          user.NormalizedName = user.Name.ToUpper();
      //                          await repository.AddAsync(user, default);
      //                      }

      //                      await unitOfWork.CommitAsync(default).ConfigureAwait(false);
      //                      await Task.Delay(TimeSpan.FromSeconds(0.5));
      //                  }
      //              }
      //              catch (DuplicateRowException)
      //              {
      //                  //Do nothing
      //              }

      //          }
      //      } while (true);
      //  }


        //public static async Task ReCopyTransferFiles()
        //{
        //    var keyNew = _container.Resolve<IConfigurationKeys>().Storage;
        //    var storageAccount = CloudStorageAccount.Parse(keyNew);
        //    var blobClient = storageAccount.CreateCloudBlobClient();
        //    var container = blobClient.GetContainerReference("spitball-files");

        //    var baseDirectory = container.GetDirectoryReference("files");
        //    var queueClient = storageAccount.CreateCloudQueueClient();
        //    //var blob = _container.Resolve<IBlobProvider<Document>>();
        //    var v = _container.Resolve<IStatelessSession>();
        //    var oldIds = 0L;
        //    var cont = false;
        //    do
        //    {
        //        cont = false;
        //        var result = await v.Query<Document>()
        //            .Where(w => w.Id > oldIds && w.OldId != null)

        //            .Take(100).Select(s => new
        //            {
        //                s.Id,
        //                s.OldId
        //            }).OrderBy(o => o.Id).ToListAsync();

        //        foreach (var item in result)
        //        {
        //            cont = true;
        //            oldIds = item.Id;

        //            var dir = baseDirectory.GetDirectoryReference(item.Id.ToString());
        //            var blobs = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None,
        //                2, null, null, null, token).ConfigureAwait(false);
        //            if (blobs.Results.Count() == 0)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Red;
        //                Console.WriteLine("processing " + item.Id);
        //                Console.ResetColor();
        //                var sqlQuery = v.CreateSQLQuery("select BlobName from zbox.item where itemid = :id");
        //                sqlQuery.SetInt64("id", item.OldId.Value);
        //                var blobName = sqlQuery.UniqueResult<string>();
        //                var newBlobName = await CopyBlobFromOldContainerAsync(blobName, item.Id);

        //                var newBlob = dir.GetBlockBlobReference(newBlobName);
        //                var pendingBlob = baseDirectory.GetBlockBlobReference(newBlobName);
        //                await newBlob.StartCopyAsync(pendingBlob);
        //            }

        //            if (blobs.Results.Count() == 1)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Yellow;
        //                Console.WriteLine("processing " + item.Id);
        //                Console.ResetColor();
        //                var queue = queueClient.GetQueueReference("generate-blob-preview");
        //                await queue.AddMessageAsync(new CloudQueueMessage(item.Id.ToString()));
        //            }
        //        }
        //    } while (cont);



        //}

//        public static async Task TransferDocuments()
//        {
//            var d = _container.Resolve<DapperRepository>();


//            var key = ConfigurationManager.AppSettings["StorageConnectionStringProd"];
//            var productionOldStorageAccount = CloudStorageAccount.Parse(key);
//            var oldBlobClient = productionOldStorageAccount.CreateCloudBlobClient();
//            var oldContainer = oldBlobClient.GetContainerReference("zboxfiles");



//            var keyNew = _container.Resolve<IConfigurationKeys>().Storage;
//            var storageAccount = CloudStorageAccount.Parse(keyNew);
//            var blobClient = storageAccount.CreateCloudBlobClient();
//            var container = blobClient.GetContainerReference("spitball-files/files");

//            //CloudBlobContainer directoryToPutFiles = container.Get .GetDirectoryReference("./files");

//            Dictionary<int, DocumentType> docType = new Dictionary<int, DocumentType>
//            {
//                {1, DocumentType.Exam},
//                {2, DocumentType.Exam},
//                {7, DocumentType.Exam},
//                {8, DocumentType.Exam},
//                {9, DocumentType.Lecture},
//                {10, DocumentType.Lecture},
//                {5, DocumentType.Textbook}
//            };

//            string[] supportedFiles =
//                {"doc", "docx", "xls", "xlsx", "PDF", "png", "jpg", "ppt", "pptx", "jpg", "png", "gif", "jpeg", "bmp"};

//            var cacheUsers = new ConcurrentDictionary<string, long?>();
//            List<dynamic> z;
//            long itemId = 631767;
//            do
//            {
//                z = await d.WithConnectionAsync(async f =>
//                {
//                    return (await f.QueryAsync(
//                        @" select top 10 I.ItemId, I.BlobName, I.Name,  B.BoxName, ZU.Email,ZUni.UniversityName, ZUNI.Country,  B.ProfessorName, 
//ISNULL(I.DocType,0) as DocType, I.NumberOfViews + I.NumberOfDownloads as [Views], I.CreationTime,
//			            STRING_AGG((T.Name), ',') as Tags
//                        FROM [Zbox].[Item] I
//                        join zbox.Box B
//	                        on I.BoxId = B.BoxId and b.discriminator in (2,3)
//                        join Zbox.Users ZU
//	                        on I.UserId = ZU.UserId
//                        join Zbox.University ZUNI on B.University = ZUNI.Id
//						left join zbox.ItemTag IT
//							on IT.ItemId = I.ItemId
//						left join zbox.Tag T
//							on IT.TagId = T.Id and len(T.Name) >= 4
//                        where I.Discriminator = 'File'
//						and i.itemid > @itemId
//	                        and I.IsDeleted = 0 
//							and I.ItemId not in (select D.OldId from sb.Document D where I.ItemId = D.OldId)
//							and (
//                                RIGHT(I.Name, 5) in ('.doc', '.docx', '.xls', '.xlsx', '.PDF', '.png', '.jpg', '.ppt', '.pptx', '.jpg', '.png', '.gif', '.jpeg', '.bmp' )
//                                or RIGHT(I.Name, 4) in ('.doc', '.docx', '.xls', '.xlsx', '.PDF', '.png', '.jpg', '.ppt', '.pptx', '.jpg', '.png', '.gif', '.jpeg', '.bmp' )
//                                )
//                        group by I.ItemId, I.BlobName, I.Name,  B.BoxName, ZU.Email,ZUni.UniversityName,ZUNI.Country, B.ProfessorName,
//						 ISNULL(I.DocType,0),I.NumberOfViews + I.NumberOfDownloads, I.CreationTime
//						 order by i.itemid
//                ", new {itemId = itemId})).ToList();
//                }, default);

//                //if (z.Count() == 0)
//                //{
//                //    return;
//                //}


//                using (var child = _container.BeginLifetimeScope())
//                {

//                    var commandBus = child.Resolve<ICommandBus>();
//                    var session = child.Resolve<IStatelessSession>();


//                    foreach (var pair in z)
//                    {
//                        itemId = pair.ItemId;
//                        Console.WriteLine($"processing {itemId}");

//                        string[] blobName = pair.BlobName.Split('.');
//                        if (!supportedFiles.Contains(blobName[1], StringComparer.OrdinalIgnoreCase))
//                        {
//                            Console.ForegroundColor = ConsoleColor.Red;
//                            Console.WriteLine($"{pair.ItemId} not blob support");
//                            Console.ResetColor();
//                            //itemsAlreadyProcessed.Add(pair.ItemId);
//                            continue;
//                        }

//                        string country = pair.Country, email = pair.Email;

//                        var userId = cacheUsers.GetOrAdd(email, x =>
//                        {
//                            long? id = GetUserId(x, country);
//                            return id;
//                        });
//                        if (userId == null)
//                        {
//                            Console.ForegroundColor = ConsoleColor.Red;
//                            Console.WriteLine($"{pair.ItemId} doesn't have userid to assign country {country}");
//                            Console.ResetColor();
//                            continue;
//                        }

//                        Guid? uniId = GetUniversityId(pair.UniversityName, country);
//                        if (uniId == null)
//                        {

//                            Console.ForegroundColor = ConsoleColor.Red;
//                            Console.WriteLine($"{pair.ItemId} doesn't have uniId to assign");
//                            Console.ResetColor();
//                            continue;
//                        }

//                        var newBlobName = await CopyBlobFromOldContainerAsync(pair.BlobName, itemId);


//                        string[] words = null;
//                        if (pair.Tags != null)
//                        {
//                            words = pair.Tags.Split(',');
//                        }

//                        DocumentType type = DocumentType.None;

//                        if (docType.ContainsKey(pair.DocType))
//                        {
//                            docType.TryGetValue(pair.DocType, out type);
//                        }

//                        string courseName = pair.BoxName;
//                        while (courseName.Length < Course.MinLength)
//                        {
//                            courseName += "-";
//                        }


//                        string itemName = pair.Name;
//                        CreateDocumentCommand command =
//                            CreateDocumentCommand.DbiOnly(newBlobName,
//                                itemName.Truncate(150),
//                                type, courseName, words?.Where(Tag.ValidateTag),
//                                userId.Value, pair.ProfessorName, uniId.Value);

//                        await commandBus.DispatchAsync(command, default);

//                        int views = pair.Views;
//                        itemId = pair.ItemId;
//                        DateTime updateTime = pair.CreationTime;

//                        var doc = session.Query<Document>().Where(w => w.Id == command.Id)
//                            .UpdateBuilder()
//                            .Set(x => x.Views, x => views)
//                            .Set(x => x.OldId, x => itemId)
//                            .Set(x => x.TimeStamp.UpdateTime, x => updateTime)
//                            .Update();
//                        await Task.Delay(TimeSpan.FromSeconds(0.5));
//                    }

//                }
//            } while (z.Count > 0);

//            //await TransferDocumants();
//        }

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

        private static long? GetUserId(string email, string country)
        {
            var d = _container.Resolve<DapperRepository>();
            return d.WithConnection<long?>(connection =>
            {
                const string sql = @"select id from sb.[user] where email = @email;
select top 1 id from sb.[user] where Fictive = 1 and country = @country order by newid()";
                using (var multi = connection.QueryMultiple(sql, new {email = email, country = country}))
                {
                    var val = multi.ReadFirstOrDefault<long?>();
                    if (val.HasValue)
                    {
                        return val.Value;
                    }

                    val = multi.ReadFirstOrDefault<long?>();
                    if (val.HasValue)
                    {
                        return val.Value;
                    }

                    return null;
                }



            });
        }

        private static Guid? GetUniversityId(string name, string country)
        {
            //select id from sb.University where Name = N'המכללה האקדמית בית ברל' and country = 'IL'
            var d = _container.Resolve<DapperRepository>();
            return d.WithConnection<Guid?>(connection =>
            {
                const string sql = @"select id from sb.University where Name = @Name and country = @country";
                using (var multi = connection.QueryMultiple(sql, new {Name = name, country = country}))
                {
                    var val = multi.ReadFirstOrDefault<Guid?>();
                    if (val.HasValue)
                    {
                        return val.Value;
                    }

                    return null;
                }
            });
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



        //  public static async Task MigrateUniversity()
        //  {
        //      var d = _container.Resolve<DapperRepository>();

        //      var z = await d.WithConnectionAsync<IEnumerable<dynamic>>(async f =>
        //      {

        //          return await f.QueryAsync(
        //              @"
        //            select top 100 U.Id, Un.UniversityName, Un.Country
        //                  from sb.[User] U
        //join zbox.Users ZU
        //	on U.Email = ZU.Email
        //                  join [Zbox].[University] Un
        //                      on Un.Id = ZU.UniversityId
        //                    where u.OldUser = 1
        //                      and IsEmailVerified = 1 
        //	and U.UniversityId2 is null
        //                      and Un.isdeleted = 0; 
        //            ");
        //      }, default);

        //      if (z.Count() == 0)
        //      { return; }

        //      using (var child = _container.BeginLifetimeScope())
        //      {
        //          var repository = child.Resolve<IRegularUserRepository>();
        //          var uni = child.Resolve<IUniversityRepository>();
        //       //   var transaction = child.Resolve<TransactionRepository>();
        //          using (var unitOfWork = child.Resolve<IUnitOfWork>())
        //          {
        //              var ch = new AssignUniversityToUserCommandHandler(repository, uni);
        //              // List<Task> taskes = new List<Task>();
        //              foreach (var pair in z)
        //              {
        //                  var command = new AssignUniversityToUserCommand(pair.Id, pair.UniversityName, pair.Country);
        //                  await ch.ExecuteAsync(command, default);
        //                  // taskes.Add(ch.ExecuteAsync(t, default));
        //              }
        //              //await Task.WhenAll(taskes);
        //              await unitOfWork.CommitAsync(default).ConfigureAwait(false);
        //          }
        //      }
        //      await MigrateUniversity();
        //  }

        public static async Task MigrateDelta()
        {
            var sessin = _container.Resolve<IStatelessSession>();

            //await TransferUsers();
            await TransferUniversities();
            //await TransferDocuments();


            //populate courses tags
            await sessin.CreateSQLQuery(@"insert into sb.Course(Name,Count)
                                    select distinct BoxName,0 from zbox.box
                                    where isdeleted = 0
                                    and discriminator in (2,3)
                                    except
                                    select Name,0 from sb.Course;
                                    insert into sb.Course(Name,Count)
                                    select distinct CourseCode,0 from zbox.box
                                    where isdeleted = 0
                                    and CourseCode is not null
                                    and discriminator in (2,3)
                                    except
                                    select Name,0 from sb.Course;").ExecuteUpdateAsync();

            //Transfer tags
            await sessin.CreateSQLQuery(@"insert into sb.Tag
                                    select DISTINCT [Name], 0 
                                    from Zbox.Tag
                                    where len(Name) >= 4
                                    and [Name] not in (select [Name] from sb.Tag)").ExecuteUpdateAsync();

            //await MigrateUniversity();
            //update country to user
            await sessin.CreateSQLQuery(@"update sb.[user]
                                    set country = (select country from  sb.University u2 where universityid2 = u2.Id)
                                    where OldUser = 1 and Cou").ExecuteUpdateAsync();

            //Users - Courses migration
            await sessin.CreateSQLQuery(@"insert into [sb].[UsersCourses]
                                    select U.Id, B.BoxName
                                    from sb.[User] U
                                    join Zbox.Users ZU
	                                    on U.Email = ZU.Email
                                    join [Zbox].[UserBoxRel] UB
	                                    on ZU.UserId = UB.UserId
                                    join Zbox.Box B
	                                    on UB.BoxId = B.BoxId
                                    where isdeleted = 0
                                    and CourseCode is not null
                                    and discriminator in (2,3)
                                    union
                                    select U.Id, B.CourseCode
                                    from sb.[User] U
                                    join Zbox.Users ZU
	                                    on U.Email = ZU.Email
                                    join [Zbox].[UserBoxRel] UB
	                                    on ZU.UserId = UB.UserId
                                    join Zbox.Box B
	                                    on UB.BoxId = B.BoxId
                                    where isdeleted = 0
                                    and CourseCode is not null
                                    and discriminator in (2,3)
                                    except
									select * 
									from [sb].[UsersCourses]").ExecuteUpdateAsync();

            await sessin.CreateSQLQuery(@"begin tran
                                    declare @tmp table (CourseId nvarchar(300), userCounter int)
                                    insert into @tmp 
                                    select CourseId, count(1)
                                    from sb.[UsersCourses]
                                    group by CourseId

                                    update [sb].[Course]
                                    set [Count] = t.userCounter
                                    from sb.[Course] c
                                    join  @tmp t
	                                    on c.[Name] = t.CourseId
                                    commit").ExecuteUpdateAsync();
        }

        private static async Task RemoveBlobs()
        {
            var _bus = _container.Resolve<ICloudStorageProvider>();
            var blobClient = _bus.GetBlobClient();
            var container = blobClient.GetContainerReference("spitball-files");
            var dir = container.GetDirectoryReference("files");

            var sessin = _container.Resolve<IStatelessSession>();
            BlobContinuationToken blobToken = null;
            do
            {
                var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 5000, blobToken,
                    new BlobRequestOptions(),
                    new OperationContext(), default);

                Console.WriteLine("Receiving a new batch of blobs");
                foreach (IListBlobItem blob in result.Results)
                {
                    var prefix = blob.Parent.Prefix.Split('/');

                    var t = sessin.Query<Document>()
                        .Where(w => w.Id == System.Convert.ToInt64(prefix[1]))
                        .FirstOrDefault();

                    if (t == null)
                    {
                        var blobToDelete = (CloudBlockBlob) blob;
                        Console.WriteLine(blobToDelete.Name);
                        await blobToDelete.DeleteAsync();
                    }

                }

                blobToken = result.ContinuationToken;
            } while (blobToken != null);
        }


        private static async Task RemoveBlobs2()
        {
            var _bus = _container.Resolve<ICloudStorageProvider>();
            var blobClient = _bus.GetBlobClient();
            var container = blobClient.GetContainerReference("spitball-files");
            var dir = container.GetDirectoryReference("files");
            BlobContinuationToken blobToken = null;
            do
            {
                var result = await dir.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 5000, blobToken,
                    new BlobRequestOptions(),
                    new OperationContext(), default);
                foreach (IListBlobItem blob in result.Results)
                {

                    var blobToDelete = (CloudBlockBlob)blob;
                    if (blobToDelete.Name.Contains("preview-blur-"))
                    {
                        blobToDelete.Delete();
                    }
                }

                blobToken = result.ContinuationToken;
            } while (blobToken != null);
        }
    }
}
