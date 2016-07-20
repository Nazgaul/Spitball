using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Storage;

namespace Management_Application
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var unity = IocFactory.IocWrapper;
            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();

            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Mail.RegisterIoc.Register();
            //Zbang.Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            //Zbang.Zbox.Infrastructure.Search.RegisterIoc.Register();

            //unity.ContainerBuilder.RegisterType<SendPush>()
            //.As<ISendPush>()
            //.WithParameter("connectionString", ConfigFetcher.Fetch("ServiceBusConnectionString"))
            //.WithParameter("hubName", ConfigFetcher.Fetch("ServiceBusHubName"))
            //.InstancePerLifetimeScope();

            unity.Build();

            Application.Run(new Form1());
        }
    }
}
