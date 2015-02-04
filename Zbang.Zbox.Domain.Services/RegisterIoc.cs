﻿
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Domain.Services
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;
            DataAccess.RegisterIoc.Register();

            ioc.RegisterType<Common.IZboxWriteService, ZboxWriteService>();
            ioc.RegisterType<Common.IZboxServiceBootStrapper, ZboxWriteService>();
        }
    }
}
