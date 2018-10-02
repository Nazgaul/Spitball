using Autofac;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Core.Models;


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

            var address = "0x0356a6cfcf3fd04ea88044a59458abb982aa9d96";
            var t = _container.Resolve<IBlockChainErc20Service>();
            var balance = await t.GetBalanceAsync(address, default);

            Console.WriteLine(balance);
           // var h = await MintTokens(address, t);
            //Console.WriteLine(await t.GetBalanceAsync(address, default));
            
            string metaMaskAddress = "0x27e739f9dF8135fD1946b0b5584BcE49E22000af";
            string spitballAddress = "0xc416bd3bebe2a6b0fea5d5045adf9cb60e0ff906";

            //var test = await t.TransferMoneyAsync("428ac528cbc75b2832f4a46592143f46d3cb887c5822bed23c8bf39d027615a8", metaMaskAddress, 80, default);
            await t.SetInitialBalanceAsync(metaMaskAddress, default);
            
            Console.WriteLine(await t.GetBalanceAsync(address, default));
            balance = await t.GetBalanceAsync(metaMaskAddress, default);
            Console.WriteLine(balance);

            //var b = _container.Resolve<FluentQueryBuilder>();
            //var x = await b.QueryAsync(new SyncAzureQuery(56123, 0), default);
            //    .AddJoin<Question,User>(q=>q.User,u=>u.Id)
            //    .AddSelect<User,Cloudents.Core.Entities.Search.Question>( q => q.Id,t2=>t2.UserId)
            //    .AddSelect<Question>( q => q.Text, "b")
            //    .AddSelect($"(select count(*) from {b.Table<Answer>()} where {b.Column<Answer>(x=>x.Question)} = {b.ColumnAlias<Question>(x=>x.Id)}) AnswerCount");

            //string t = b;
            //Console.WriteLine(t);
            /* var b2 = _container.Resolve<ITutorSearch>();
             var result = await b2.SearchAsync(null, new[] { TutorRequestFilter.InPerson }, TutorRequestSort.Relevance, 
                 new GeoPoint(-74.006f, 40.7128f)
                 , 0, false, default);*/



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

            //TODO: TALK TO RAM. THIS IS NOT THE WAY WE DO STUFF NOW.

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
