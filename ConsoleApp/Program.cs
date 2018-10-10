using Autofac;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Core.Request;
using Nethereum.Web3.Accounts;
using Nethereum.Web3;

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
            builder.RegisterType<PPP>().As<IDataProtect>();
            builder.RegisterSystemModules(
                Cloudents.Core.Enum.System.Console,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                //Assembly.Load("Cloudents.Infrastructure.Data"),
                Assembly.Load("Cloudents.Core"));
            _container = builder.Build();

            
            //var t = _container.Resolve<IBlockChainErc20Service>();
            //var spitballAddress = "0x0356a6cfcf3fd04ea88044a59458abb982aa9d96";
            //string metaMaskAddress = "0x27e739f9dF8135fD1946b0b5584BcE49E22000af";
            //string spitballServerAddress = "0xc416bd3bebe2a6b0fea5d5045adf9cb60e0ff906";

            //string SpitballPrivateKey = "428ac528cbc75b2832f4a46592143f46d3cb887c5822bed23c8bf39d027615a8";
            //string metaMaskPK = "10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4";

            

            //Account SpitballAccountt = new Account(SpitballPrivateKey);
            //var web3 = new Web3(SpitballAccountt);
            ////var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(t.GetAddress(SpitballPrivateKey));

            //for (int i = 0; i < 100; i++)
            //{
            //    var txCount = web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(t.GetAddress(SpitballPrivateKey));

            //    var TxHash = t.TransferPreSignedAsync(metaMaskPK, spitballServerAddress, 2, 1, default);
            //    //var approve = t.IncreaseApprovalPreSignedAsync(SpitballPrivateKey, spitballServerAddress, 1, 1, default);

            //    var b1 = t.GetBalanceAsync(metaMaskAddress, default);
            //    var b2 = t.GetBalanceAsync(spitballServerAddress, default);
            //    var b3 = t.GetBalanceAsync(spitballAddress, default);
                
            //    var a1 = t.GetAllowanceAsync(spitballAddress, spitballServerAddress, default);

            //    await Task.WhenAll(txCount, TxHash, b1, b2, b3/*, approve*/, a1).ConfigureAwait(false);

            //    Console.WriteLine($"nonce: {txCount.Result.Value}");
            //    Console.WriteLine($"metaMaskAddress Balance: {b1.Result}");
            //    Console.WriteLine($"spitballServerAddress Balance: {b2.Result}");
            //    Console.WriteLine($"spitballAddress Balance: {b3.Result}");
            //    Console.WriteLine("---------------------------------");
            //    Console.WriteLine($"allowance:{a1.Result}");
            //    Console.WriteLine("---------------------------------");

            //}

            /*Console.WriteLine(await t.GetAllowanceAsync(spitballAddress, spitballServerAddress, default));

            var y = await t.IncreaseApproval(spitballServerAddress, 100, default);
            Console.WriteLine(await t.GetAllowanceAsync(spitballAddress, spitballServerAddress, default));
            */
            /*Console.WriteLine($"Sender Balance: {await t.GetBalanceAsync(metaMaskAddress, default)}");
            Console.WriteLine($"To Balance: {await t.GetBalanceAsync(spitballServerAddress, default)}");
            Console.WriteLine($"Spender Balance: {await t.GetBalanceAsync(spitballAddress, default)}");
            
            Console.WriteLine("-------------------");

            var y = await t.ApprovePreSigned(metaMaskPK, spitballServerAddress, 5, 5, default);

            //var y = await t.TransferPreSigned(metaMaskPK, spitballServerAddress, 5, 5, default);

            Console.WriteLine($"Sender Balance: {await t.GetBalanceAsync(metaMaskAddress, default)}");
            Console.WriteLine($"To Balance: {await t.GetBalanceAsync(spitballServerAddress, default)}");
            Console.WriteLine($"Spender Balance: {await t.GetBalanceAsync(spitballAddress, default)}");
            */



            //var u = _container.Resolve<ISearchServiceWrite<University>>();
            //await u.CreateOrUpdateAsync(default);


            

            var b2 = _container.Resolve<IDocumentSearch>();
            var query = SearchQuery.Document(null, 920, null, null, 0);
           // var query = new CoursesQuery(2343);
            //var query = new QuestionsQuery(null, null, 0, null);
            var t = await b2.SearchDocumentsAsync(query, default);

           

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



            Console.WriteLine("Finish");
            Console.ReadLine();
        }

        private static string TTTT(string input,IEnumerable<string> BAD_WORDS)
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
            return await t.CreateNewTokens(address, 100, default);
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

    public class PPP : IDataProtect
    {
        public string Protect(string purpose, string plaintext, DateTimeOffset expiration)
        {
            throw new NotImplementedException();
        }

        public string Unprotect(string purpose, string protectedData)
        {
            throw new NotImplementedException();
        }
    }
}
