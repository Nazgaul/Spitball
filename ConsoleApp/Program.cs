﻿// ReSharper disable All
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
using Nethereum.Web3.Accounts;

namespace ConsoleApp
{
    static class Program
    {
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


            //var t = container.Resolve<ICrowdsaleService>();
            //var z = await t.BuyTokens("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", 1, default);

            //var a = container.Resolve<IBlockChainErc20Service>();
            //var b = await a.GetBalanceAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", default);


            Guid answer = Guid.NewGuid();
            var c = container.Resolve<IBlockChainQAndAContract>();
           await c.SubmitQuestionAsync("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", 3, 1, default);
           await c.SubmitAnswerAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", 3, answer, default);
           
            await c.UpVoteAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", 3, answer, 1, default);
            await c.MarkAsCorrectAsync("0x27e739f9dF8135fD1946b0b5584BcE49E22000af", 3, default);
            var h = await c.UpVoteListAsync(3, answer, default);
            Console.WriteLine(h[0]);
            //Guid.Parse("078d1202-834a-4634-9aec-1bdf1127368c")

            Console.WriteLine("Finish");
            Console.ReadLine();
        }



      


    }
    [SerializableAttribute]
public class A
    {
        public int a { get; set; }
    }

[SerializableAttribute]
public class B : A
    {
        public int aa { get; set; }
    }
}
