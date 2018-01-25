﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.Metadata;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.DocumentDb;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;

namespace ConsoleApp
{
    static class Program
    {
        static async Task Main()
        {
            var builder = new ContainerBuilder();
            //var infrastructureModule = new InfrastructureModule(
            //    ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
            //    ConfigurationManager.AppSettings["AzureSearchServiceName"],
            //    ConfigurationManager.AppSettings["AzureSearchKey"],
            //    ConfigurationManager.AppSettings["Redis"],
            //    ConfigurationManager.AppSettings["StorageConnectionString"]);

            //  builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
            builder.RegisterModule(new ModuleWrite(new SearchServiceCredentials(
                ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"])
                    ,
                ConfigurationManager.AppSettings["Redis"],
                new LocalStorageData(Path.Combine(Directory.GetCurrentDirectory(), "Temp"), 500)));
            builder.RegisterModule<IocModule>();
            var container = builder.Build();

            var affiliate = container.ResolveKeyed<IUpdateAffiliate>(AffiliateProgram.Wyzant);
            await affiliate.ExecuteAsync(0, i =>
             {
                 Console.WriteLine(i);
                 return Task.CompletedTask;
             }, default);
            //210ec431-2d6d-45cb-bc01-04e3f687f0ed.docx
            Console.ReadLine();


            // var model = SearchQuery.Document(new [] {"microsoft"}, null, null, null, 0, SearchRequestSort.None, null);
        }
    }
}
