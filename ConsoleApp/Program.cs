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
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Models;
using Cloudents.Core.Query;


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
                BlockChainNetwork = "http://spito5-dns-reg1.northeurope.cloudapp.azure.com:8545",
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





            //var u = _container.Resolve<ISearchServiceWrite<University>>();
            //await u.CreateOrUpdateAsync(default);


            var stopWordsList = new[]{ "university",
                "of",
                "college",
                "school",
                "the",
                "a",
                "המכללה","אוניברסיטת","מכללת","האוניברסיטה"
            };
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
            var cleanName = "University of Kent".RemoveWords(stopWordsList);

            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            sw.Restart();
            for (int i = 0; i < 100000; i++)
            {
                var cleanName = TTTT("University of Kent", stopWordsList);

            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            //var b2 = _container.Resolve<IQueryBus>();
            //var query = new SyncAzureQuery(1,0);
            //var d = await b2.QueryAsync< (IEnumerable<UniversitySearchDto> update, IEnumerable<long> delete, long version)>(query, default);
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
