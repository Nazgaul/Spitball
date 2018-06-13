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
        const string ServiceBusConnectionString = "Endpoint=sb://spitball-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CACOBTEeKVemCY7ScVBHYXBwDkClQcCKUW7QGq8dNfA=";
        const string TopicName = "topic1";
        static TopicClient topicClient;

        static async Task Main()
        {
            var builder = new ContainerBuilder();
            var keys = new ConfigurationKeys
            {
                Db = ConfigurationManager.ConnectionStrings["ZBoxProd"].ConnectionString,
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"]),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"],
                LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200),
                BlockChainNetwork = "http://spito5-dns-reg1.northeurope.cloudapp.azure.com:8545",
                ServiceBus = ConfigurationManager.AppSettings["ServiceBusProd"]
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
            var container = builder.Build();

           

            //topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            //var x = new ServiceClientCredentials()
            //var t = new ServiceBusManagementClient()

            
            // await SendMessagesAsync(10);

            
           // var z = ty.Split(new[] {'.', '@'}, StringSplitOptions.RemoveEmptyEntries)[0];
            var c = container.Resolve<IServiceBusProvider>();
            var a = container.Resolve<IUserRepository>();

            foreach (var user in await a.GetQueryable().ToListAsync())
            {
                //if (user.EmailConfirmed)
                //{
                    var x = new TalkJsUser(user.Id, user.Name)
                    {
                        Email = user.Email
                    };
                    await c.InsertMessageAsync(x, default);
                //}

            }
            //var t = await a.GetQuestionsAsync(new QuestionsQuery()
            //{
            //    Term = "files"
            //}, default);
            

            //var t = await a.GetQuestionDtoAsync(704, default);
            
            //var registerEmail = new RegistrationEmail("ram@cloudents.com","https://dev.spitball.co");
            //await a.InsertMessageAsync(registerEmail, default);

           // var z = a.GetAddress("2ee004d7bdc744205d8a3d31a1b2cd4816b8f860448427b570290c9a1cb571d6");
            //await a.SetInitialBalanceAsync(z, default).ConfigureAwait(false);
            //var c = a.TransferMoneyAsync("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", "0xAcfB119204a93BbDa781C972D27AeAB8671c63f4", 10, default);
            //var b = a.GetBalanceAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", default);

            //var c = a.TransferMoneyAsync("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", "0xAcfB119204a93BbDa781C972D27AeAB8671c63f4", 10, default);
            //var b = a.GetBalanceAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", default);
            //var b = await a.CreateNewTokens("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", 10, default);
            ////var (privateKey, publicAddress) = a.CreateAccount();
            //var b = a.GetBalanceAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", default);

            //var a = container.Resolve<IBlockChainCrowdSaleService>();
            //var b = await a.BuyTokensAsync("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", 10, default);
            //await a.Withdrawal(default);

            //Guid answer = Guid.NewGuid();
            // var c = container.Resolve<IBlockChainQAndAContract>();
            ////var command = new CreateQuestionCommand()
            ////{
            ////    Price = 0.5m,
            ////    SubjectId = 1,
            ////    Text = "123",
            ////    UserId = 11
            ////};
            ////await c.DispatchAsync(command, default);

            //await c.SubmitQuestionAsync(3, 1, "0x116CC5B77f994A4D375791A99DF12f19921138ea", default);
            //await c.SubmitAnswerAsync(3, answer, default);

            //await c.UpVoteAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", 3, answer, default);
            //await c.MarkAsCorrectAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", "0x27e739f9dF8135fD1946b0b5584BcE49E22000af", 3, answer, default);
            // var h = await c.UpVoteListAsync(3, answer, default);
            // Console.WriteLine(h[0]);
            //Guid.Parse("078d1202-834a-4634-9aec-1bdf1127368c")

            Console.WriteLine("Finish");
            Console.ReadLine();
            //await topicClient.CloseAsync();
        }

        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the topic.
                    string messageBody = $"Message {i}";
                    var m2 = new Message();
                    m2.UserProperties["t"] = "T";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                    message.Label = "2";
                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the topic.
                    await topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }






    }
   
}
