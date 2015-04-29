
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Domain.Services
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;
            DataAccess.RegisterIoc.Register();

            ioc.RegisterType<Common.IZboxWriteService, ZboxWriteService>();
            ioc.RegisterType<Common.IZboxWorkerRoleService, ZboxWorkerRoleService>();
            ioc.RegisterType<Common.IZboxServiceBootStrapper, ZboxWriteService>();
        }
    }
}
