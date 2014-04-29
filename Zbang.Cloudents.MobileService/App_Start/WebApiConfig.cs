using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.MobileService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions
            {
                PushAuthorization = Microsoft.WindowsAzure.Mobile.Service.Security.AuthorizationLevel.User
            };
            

            // Use this class to set WebAPI configuration options
            //HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options, (configuration, builder) =>
            {
                builder.RegisterType<FacebookAuthenticationService>().As<IFacebookAuthenticationService>();
            }));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
           // Database.SetInitializer(new MobileServiceInitializer());
        }
    }

    //public class MobileServiceInitializer : System.Data.Entity.NullDatabaseInitializer<MobileServiceContext>// DropCreateDatabaseIfModelChanges<MobileServiceContext>
    //{
    //    //protected override void Seed(MobileServiceContext context)
    //    //{
    //    //    List<TodoItem> todoItems = new List<TodoItem>
    //    //    {
    //    //        new TodoItem { Id = "1", Text = "First item", Complete = false },
    //    //        new TodoItem { Id = "2", Text = "Second item", Complete = false },
    //    //    };

    //    //    foreach (TodoItem todoItem in todoItems)
    //    //    {
    //    //        context.Set<TodoItem>().Add(todoItem);
    //    //    }

    //    //    base.Seed(context);
    //    //}
    //}
}

