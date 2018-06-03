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
using Cloudents.Core.Storage;
using Microsoft.Azure.ServiceBus;
using Nethereum.Web3.Accounts;

namespace ConsoleApp
{
    static class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://spitball-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CACOBTEeKVemCY7ScVBHYXBwDkClQcCKUW7QGq8dNfA=";
        const string TopicName = "topic1";
        static ITopicClient topicClient;

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
                BlockChainNetwork = "http://hopoea-dns-reg1.northeurope.cloudapp.azure.com:8545"
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterSystemModules(
                Cloudents.Core.Enum.System.Console,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"));
            //builder.RegisterType<TutorMeSearch>().AsSelf();
            var container = builder.Build();

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            await SendMessagesAsync(10);



            //var c = container.Resolve<IBlockChainQAndAContract>();
            //var d = await c.SubmitQuestionAsync(1, 1, "0x27e739f9dF8135fD1946b0b5584BcE49E22000af", default);

            Console.WriteLine("Finish");
            Console.ReadLine();
            await topicClient.CloseAsync();
        }

        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the topic.
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

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
