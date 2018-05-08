// ReSharper disable All
using System;
using System.Collections.Generic;
using System.Configuration;
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
using Cloudents.Core.Read;
using Cloudents.Core.Request;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Mail;
using Cloudents.Infrastructure.Search.Tutor;

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
                LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200)
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

            var t = container.Resolve<IGoogleAuth>();
            var z = await t.LogInAsync(
                            "eyJhbGciOiJSUzI1NiIsImtpZCI6IjNmM2VmOWM3ODAzY2QwYjhkNzUyNDdlZTBkMzFmZGQ1YzJjZjM4MTIifQ.eyJhenAiOiI5OTc4MjMzODQwNDYtZGRocnBoaWd1MGhzZ2trMWRnbGFqYWlmY2cycmdnYm0uYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI5OTc4MjMzODQwNDYtZGRocnBoaWd1MGhzZ2trMWRnbGFqYWlmY2cycmdnYm0uYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTE5Njg5NTkyNTg0Mjg1ODE2OTQiLCJlbWFpbCI6InlhYXJpLnJhbUBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiYXRfaGFzaCI6Ilplc2o1bXBKUWRGYnYyT3RUUkUxMkEiLCJleHAiOjE1MjU3NzY5NDksImlzcyI6ImFjY291bnRzLmdvb2dsZS5jb20iLCJqdGkiOiI5ZjJiODE1M2MwMjE0MDUyNzI2ZDU1MTQzNGYyMGVkM2IyNGZiMjdmIiwiaWF0IjoxNTI1NzczMzQ5LCJuYW1lIjoiUmFtIFlhYXJpIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS8tUktRWEdWdXJxODAvQUFBQUFBQUFBQUkvQUFBQUFBQUFDSzAvdVY0VE1aNDdxMjgvczk2LWMvcGhvdG8uanBnIiwiZ2l2ZW5fbmFtZSI6IlJhbSIsImZhbWlseV9uYW1lIjoiWWFhcmkiLCJsb2NhbGUiOiJlbiJ9.SfLbhq_eg_b73RexoPefyoVVkrxwZ_8NZBpOPP3PXdFkO7nz4RJd-Wanz7enlViHqoavjnz-W3Cc5zFHF8sK4utU63DBUMtjgiKYIhs9Zp8QpfMqSq6SJGO4Xs1hv4gINkdUPBXE-HOx6ss_06Hes083GXy1RC1pGoOL6AHcg9ZtLZSeR1UP0Qa3TVzeDp52xwoO1A72bzPyk2Z1tZDqsAIZdTGhzugzbQ9QQ_HN2GGf8gRfzD66JDZag67vIj4f5nHCjveXhg2t6v9PIvNRhPr_Ku0miRoA7Sys1e2y96WZnVBi-QtdqIB3I8tLyP6pCRrYOKdlEIVdS4lo3S5QuQ",
                            default);

            //var resolve2 = container
            //    .Resolve<IReadRepositoryAsync<(IEnumerable<CourseSearchWriteDto> update, IEnumerable<SearchWriteBaseDto>
            //        delete, long version), SyncAzureQuery>>();

            //var t2 = await resolve2.GetAsync(new SyncAzureQuery(0, 0), default);



            Console.WriteLine("Finish");
            Console.ReadLine();
        }





    }
}
