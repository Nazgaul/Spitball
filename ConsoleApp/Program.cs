using Autofac;
using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts;

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
                Db = new DbConnectionString(ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString, ConfigurationManager.AppSettings["Redis"]),
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"], true),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"],
                LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200),
                BlockChainNetwork = "http://13.69.54.132:8545",
                ServiceBus = ConfigurationManager.AppSettings["ServiceBus"]
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
            _container = builder.Build();



            Console.WriteLine(Environment.UserName);
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


            //var u = _container.Resolve<ISearchServiceWrite<University>>();
            //await u.CreateOrUpdateAsync(default);




            // var b2 = _container.Resolve<IDocumentSearch>();
            // var query = SearchQuery.Document(null, 920, null, null, 0);
            // var query = new CoursesQuery(2343);
            //var query = new QuestionsQuery(null, null, 0, null);
            //var t = await b2.SearchDocumentsAsync(query, default);



            //var d = await b2.SearchAsync(query, default);
            ////var result = await b2.SearchAsync(null, new[] { TutorRequestFilter.InPerson }, TutorRequestSort.Relevance, 
            //    new GeoPoint(-74.006f, 40.7128f)
            //    , 0, false, default);



            // QuestionRepository c = new QuestionRepository(b);
            // Console.WriteLine(c.GetOldQuestionsAsync(default));



            //await UpdateCreationTimeProductionAsync();
            //IEventMessage z = new AnswerCreatedEvent(null);
            //var query = new UserDataByIdQuery(1642);
            //var query = new FictiveUsersQuestionsWithoutCorrectAnswerQuery();
            //var t = await bus.QueryAsync< ProfileDto>(query, default);
            //var query = new FictiveUsersQuestionsWithoutCorrectAnswerQuery();
            //var t = await bus.QueryAsync(query, default);
            //  var bus = _container.Resolve<IQueryBus>();
            // var z = new NextQuestionQuery(68, 11);
            // var x = await bus.QueryAsync(z, default);




        }

        private static async Task RamMethod()
        {
            var bus = _container.Resolve<ITutorSearch>();
            var z = await bus.SearchAsync(null, new TutorRequestFilter[] {TutorRequestFilter.InPerson,TutorRequestFilter.Online },
                TutorRequestSort.Relevance,new GeoPoint(34.8516f, 31.0461f), 0, false, default);
        }

        private static async Task HadarMethod()
        {
            var t = _container.Resolve<IBlockChainErc20Service>();
            string spitballServerAddress = "0xc416bd3bebe2a6b0fea5d5045adf9cb60e0ff906";


            string spitballServerPK1 = "53d8ca027b5689e062c0e461b4aee444676fdf21d23ca73b8e1da1bc9b03bedf";
            string spitballServerPK2 = "dc48f1874948869118757457827d7ca4efe8bce63730053f25b25c5b979efbca";
            string spitballServerPK3 = "78cdd8076ccd67e1b64bb46eddad6bb7647da141bf3d870529a1f84169a4dd79";
            string spitballServerPK4 = "ff755a496156cd42544d436f191f5842c3295d3a44623561c46a7fe983ae1f06";
            string spitballServerPK5 = "5f271ce762fbcbe6a6e5472bf7c0ac12562f3e1fdb352be1f05b785eef8989a5";
            string spitballServerPK6 = "d9f1ee276e00316679986b157dc66ce99f47400094157bea6381c822c88bdadb";
            string spitballServerPK7 = "30ffb3e064f5aad2f602d9830dc838d12ceec5236700998566132de02866d512";
            string spitballServerPK8 = "31281982d9965bae7597c59404b199a8f4d25b2e8070ef9a79dea8b4c01deb50";
            string spitballServerPK9 = "9438658b938ab1767c3d1577a4999558cd0b34d640b978f4577659383f6c115e";
            string spitballServerPK10 = "26633b6a2bc76ba070c9813edbf721d206da795482d0f26f4c63f7a4212ee456";

            string SbAdd1 = "0xc416bd3bebe2a6b0fea5d5045adf9cb60e0ff906";
            string SbAdd2 = "0xc7fe8b32b435aa2a4a624f8dd84cb39dfafef83d";
            string SbAdd3 = "0xabe6778e3091496628ff39ab206dd84a7fd09141";
            string SbAdd4 = "0xb47b4a77dbfaad4d0b0bdfe6bb7070d27538ff52";
            string SbAdd5 = "0x71597bff6c07d5d1061182b4bec820a45f6cb900";
            string SbAdd6 = "0x8f333898c9b0c52d3a1ce020120bd4d80dd585b7";
            string SbAdd7 = "0x611a77566067e18d7eb59b9ba9e4d99d0bac4c2b";
            string SbAdd8 = "0xbf74d27e2667ca48d3a82fb525f10001a2fe2b7d";
            string SbAdd9 = "0x68e29eab0092aa2272701f6971ab311ac4618822";
            string SbAdd10 = "0xa130fec27ccc8296a7dcf193a52c31e6e62b886c";


            /*
            string metaMaskPK = "10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4";



            var b1 = await t.GetBalanceAsync(metaMaskAddress, default);
            var b2 = await t.GetBalanceAsync(spitballServerAddress, default);
            Console.WriteLine($"metaMaskAddress Balance: {b1}");
            Console.WriteLine($"spitballServerAddress Balance: {b2}");
            */

            //Task<Task> TxHash;

             var account = new Account(spitballServerPK1);
             var web3 = new Nethereum.Web3.Web3(account);
            
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd2, new HexBigInteger(10000000000000000000));
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd3, new HexBigInteger(10000000000000000000));
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd4, new HexBigInteger(10000000000000000000));
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd5, new HexBigInteger(10000000000000000000));
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd6, new HexBigInteger(10000000000000000000));
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd7, new HexBigInteger(10000000000000000000));
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd8, new HexBigInteger(10000000000000000000));
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd9, new HexBigInteger(10000000000000000000));
            await web3.TransactionManager.SendTransactionAsync(spitballServerAddress, SbAdd10, new HexBigInteger(10000000000000000000));
            
            /*
                       var wl1 = await t.WhitelistUserForTransfers(SbAdd1, default);
                       var wl2 = await t.WhitelistUserForTransfers(SbAdd2, default);
                       var wl3 = await t.WhitelistUserForTransfers(SbAdd3, default);
                       var wl4 = await t.WhitelistUserForTransfers(SbAdd4, default);
                       var wl5 = await t.WhitelistUserForTransfers(SbAdd5, default);
                       var wl6 = await t.WhitelistUserForTransfers(SbAdd6, default);
                       var wl7 = await t.WhitelistUserForTransfers(SbAdd7, default);
                       var wl8 = await t.WhitelistUserForTransfers(SbAdd8, default);
                       var wl9 = await t.WhitelistUserForTransfers(SbAdd9, default);
                       var wl10 = await t.WhitelistUserForTransfers(SbAdd10, default);
           */
            var d = await t.GetBalanceAsync(spitballServerAddress, default);
            Console.WriteLine($"spitballServerAddress Balance: {d}");
            ConcurrentQueue<string> staticQ = new ConcurrentQueue<string>();


            staticQ.Enqueue(spitballServerPK1);
            staticQ.Enqueue(spitballServerPK2);
            staticQ.Enqueue(spitballServerPK3);
            staticQ.Enqueue(spitballServerPK4);
            staticQ.Enqueue(spitballServerPK5);
            staticQ.Enqueue(spitballServerPK6);
            staticQ.Enqueue(spitballServerPK7);
            staticQ.Enqueue(spitballServerPK8);
            staticQ.Enqueue(spitballServerPK9);
            staticQ.Enqueue(spitballServerPK10);

            for (int i = 0; i < 200; i++)
            {
               

                ConcurrentQueue<string> blockQ = new ConcurrentQueue<string>(staticQ);

                string res;
                blockQ.TryDequeue(out res);
                var tx1 = t.TransferPreSignedAsync(res, spitballServerPK1, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx2 = t.TransferPreSignedAsync(res, spitballServerPK2, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx3 = t.TransferPreSignedAsync(res, spitballServerPK3, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx4 = t.TransferPreSignedAsync(res, spitballServerPK4, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx5 = t.TransferPreSignedAsync(res, spitballServerPK5, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx6 = t.TransferPreSignedAsync(res, spitballServerPK6, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx7 = t.TransferPreSignedAsync(res, spitballServerPK7, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx8 = t.TransferPreSignedAsync(res, spitballServerPK8, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx9 = t.TransferPreSignedAsync(res, spitballServerPK9, spitballServerAddress, 2, 1, default);
                blockQ.TryDequeue(out res);
                var tx10 = t.TransferPreSignedAsync(res, spitballServerPK10, spitballServerAddress, 2, 1, default);

                await Task.WhenAll(tx1, tx2, tx3, tx4, tx5, tx6, tx7, tx8, tx9, tx10).ConfigureAwait(false);

                d = await t.GetBalanceAsync(spitballServerAddress, default);
                Console.WriteLine($"spitballServerAddress Balance: {d}");
                Console.WriteLine(i);
                Console.WriteLine($"---------------------------------");
            }
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

        private static async Task PopulateSheetOfQuestion()
        {
            string spreadsheetId = "1A2O_jASZuWlI_jIX8a1eiZb61C5RDF9KQ2i7CQzGU30";
            string range = "All!B:D";


            //var subjectList = new List<CreateQuestionCommand>();
            var subjectList = GoogleSheets.GetData(spreadsheetId, range);
            foreach (var question in GoogleSheets.GetData(spreadsheetId, range))
            {
                var commandBus = _container.Resolve<ICommandBus>();
                // await commandBus.DispatchAsync(question, default);
            }
        }


        public static async Task UpdateCreationTimeProductionAsync()
        {
            using (var child = _container.BeginLifetimeScope())
            {
                using (var unitOfWork = child.Resolve<IUnitOfWork>())
                {
                    var repository = child.Resolve<IQuestionRepository>();
                    var questions = await repository.GetAllQuestionsAsync().ConfigureAwait(false);
                    var random = new Random();
                    foreach (var question in questions)
                    {
                        if (question.CorrectAnswer == null && question.User.Fictive)
                        {
                            // question.Created = DateTimeHelpers.NextRandomDate(2, random);
                            Console.WriteLine(question.Created);
                            await repository.UpdateAsync(question, default);
                        }
                        //user1.UserCreateTransaction();
                        //await t.UpdateAsync(user1, default).ConfigureAwait(false);
                    }
                    await unitOfWork.CommitAsync(default).ConfigureAwait(false);
                }
            }

        }
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
