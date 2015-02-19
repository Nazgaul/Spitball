using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;
            
            ioc.RegisterType<ApplicationDbContext>();
            ioc.RegisterType<ApplicationUserManager>();

           

           
        }
    }
}
