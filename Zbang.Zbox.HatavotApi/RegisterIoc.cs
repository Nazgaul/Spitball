using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Store.Services;

namespace Zbang.Zbox.Store
{
   public  static class RegisterIoc
    {
       public static void Register()
       {
           var ioc = IocFactory.Unity;

           ioc.RegisterType<IReadService, ReadService>();
           ioc.RegisterType<IWriteService, WriteService>();
       }
    }
}
