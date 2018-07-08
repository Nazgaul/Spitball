using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;

namespace ConsoleApp
{
    internal static class Program
    {
        private static IContainer container;

        static async Task Main()
        {
            var builder = new ContainerBuilder();
            var keys = new ConfigurationKeys
            {
                Db = ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"]),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"],
                LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200),
                BlockChainNetwork = "http://spito5-dns-reg1.northeurope.cloudapp.azure.com:8545",
                ServiceBus = ConfigurationManager.AppSettings["ServiceBus"]
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterSystemModules(
                Cloudents.Core.Enum.System.Console,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Infrastructure.Data"),
                Assembly.Load("Cloudents.Core"));
            container = builder.Build();

            // var _serviceBusProvider = container.Resolve<IServiceBusProvider>();

            // await _serviceBusProvider.InsertMessageAsync(new SupportRedeemEmail(100, 587), default);
            var bus = container.Resolve<IServiceBusProvider>();


            var message = new RegistrationEmail("ram@cloudents.com", "https://dev.spitball.co");
            await bus.InsertMessageAsync(message, default);
            await bus.InsertMessageAsync(new AnswerCorrectEmail
            ("ram@cloudents.com",
                "This is a question text which is very very long and i dont know why it is very very long. hi hi hi , yo yo yo",
                "This is a answer text which is very very long and i dont know why it is very very long. hi hi hi , yo yo yo",
                "https://dev.spitball.co", 100), default);
            // var r= await bus.QueryAsync<IEnumerable<BalanceDto>>(new UserDataByIdQuery(638), default);

            //var p = new TransactionPopulation(container);
            //await p.CreateTransactionOnExistingDataAsync();
            // await p.AddToUserMoney(100000, 660);




            //var q = new UserBalanceQuery(36);
            //var t = await queryBus.QueryAsync(q, default);
            //var sw = new Stopwatch();
            //sw.Start();
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedTicks);

            //var tt = container.Resolve<IQueryHandlerAsync<QuestionDetailQuery, QuestionDetailDto>>();
            //sw.Start();
            //var zz = await tt.GetAsync(new QuestionDetailQuery(1414), default);
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedTicks);


            Console.WriteLine("Finish");
            Console.ReadLine();
        }

        public static Task SendMoneyAsync()
        {
            var t = container.Resolve<IBlockChainErc20Service>();
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
    }
}
