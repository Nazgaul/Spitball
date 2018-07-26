using System;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using System.Collections.Generic;

namespace ConsoleApp
{
    internal static class Program
    {
        private static IContainer _container;

        static void Main()
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
            _container = builder.Build();

            String spreadsheetId = "1Mq1ec4dGp6ADuKmlrAa5rsFm-_-JvT8O0miC8wsd1T8";
            String range = "All!B:D";
            var subjectList = new List<CreateQuestionCommand>();
            subjectList = GoogleSheets.GetData(spreadsheetId, range);

           



            /* var _serviceBusProvider = _container.Resolve<ICommandBus>();

             var command = new UpdateQuestionTimeCommand();

             await _serviceBusProvider.DispatchAsync(command, default);


             Console.WriteLine("Finish");
             Console.ReadLine();
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
             }*/
         }
        }
}
