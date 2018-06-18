// ReSharper disable All
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Mail;
using Cloudents.Infrastructure.Search.Tutor;
using Cloudents.Infrastructure.BlockChain;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Rest;
using Nethereum.Web3.Accounts;
using NHibernate.Linq;

namespace ConsoleApp
{
    static class Program
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
            //builder.RegisterType<TutorMeSearch>().AsSelf();
            container = builder.Build();

            //await SendMoneyAsync();

           // var t = container.Resolve<IUserRepository>();
           // var z = await t.GetUserProfileAsync(551, default);

            //var t = container.Resolve<IQuestionRepository>();
            //var z = await t.GetQuestionsAsync(new QuestionsQuery()
            //{
            //    Term = "Irena's question"
            //}, default);


            var a = container.Resolve<IBlockChainQAndAContract>();
            var b = container.Resolve<IBlockChainErc20Service>();
            //var b = await a.CashIn("74c79f73068ffe5e3c9b3485a5b5f57d8966cc9d1c15f850603c2c2e559b329f", 10);
            //var c = await a.CashIOut("74c79f73068ffe5e3c9b3485a5b5f57d8966cc9d1c15f850603c2c2e559b329f", 10);

            //Testing events

            //TransferTokens
            Guid answer = Guid.NewGuid();
            Guid answer2 = Guid.NewGuid();
            var FromAccount = new Account("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4");
            var Toaccount = new Account("74c79f73068ffe5e3c9b3485a5b5f57d8966cc9d1c15f850603c2c2e559b329f");
            //Console.WriteLine(a.GetBalanceAsync(Toaccount.Address, default).Result);
            //var c = await a.TransferMoneyAsync("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", Toaccount.Address, 1000, default);
            //Console.WriteLine(a.GetBalanceAsync(Toaccount.Address, default).Result);


            BlockChainSubmitQuestion Submittest = new BlockChainSubmitQuestion(1, 10, FromAccount.Address);
            BlockChainSubmitAnswer answertest1 = new BlockChainSubmitAnswer(1, answer, Toaccount.Address);
            BlockChainSubmitAnswer answertest2 = new BlockChainSubmitAnswer(1, answer2, Toaccount.Address);
            BlockChainMarkQuestionAsCorrect correct = new BlockChainMarkQuestionAsCorrect(FromAccount.Address, Toaccount.Address, 1, answer);
            BlockChainUpVote UV1 = new BlockChainUpVote(FromAccount.Address, 1, answer);
            BlockChainUpVote UV2 = new BlockChainUpVote(FromAccount.Address, 1, answer2);

            Console.WriteLine(b.GetBalanceAsync(FromAccount.Address, default).Result);
            await a.SubmitAsync(Submittest, default);
            Console.WriteLine(b.GetBalanceAsync(FromAccount.Address, default).Result);
            await a.SubmitAsync(answertest1, default);
            Console.WriteLine("SubmitAnswer1");
            await a.SubmitAsync(answertest2, default);
            Console.WriteLine("SubmitAnswer2");
            await a.SubmitAsync(UV1, default);
            Console.WriteLine("UpVote1");
            await a.SubmitAsync(UV2, default);
            Console.WriteLine("UpVote2");
            Console.WriteLine(b.GetBalanceAsync(FromAccount.Address, default).Result);
            await a.SubmitAsync(correct, default);
            Console.WriteLine(b.GetBalanceAsync(FromAccount.Address, default).Result);


            Console.WriteLine("Finish");
            Console.ReadLine();
            //await topicClient.CloseAsync();
        }

        public static Task SendMoneyAsync()
        {
            var t = container.Resolve<IBlockChainErc20Service>();
            var pb = t.GetAddress("38d68c294410244dcd009346c756436a64530d7ddb0611e62fa79f9f721cebb0");
            return t.SetInitialBalanceAsync(pb, default);
            //
        }

    }
   
}
