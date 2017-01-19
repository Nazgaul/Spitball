using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
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

            var builder = IocFactory.IocWrapper;

            builder.ContainerBuilder.RegisterModule<InfrastructureModule>();
            builder.ContainerBuilder.RegisterModule<DataModule>();
           // Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            builder.ContainerBuilder.RegisterModule<WriteServiceModule>();

            builder.ContainerBuilder.RegisterModule<CommandsModule>();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Mail.RegisterIoc.Register();
            builder.ContainerBuilder.RegisterModule<StorageModule>();
            builder.Build();

            Application.Run(new Form1());
        }
    }
}
