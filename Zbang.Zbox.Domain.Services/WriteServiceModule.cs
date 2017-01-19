
using Autofac;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Domain.Services
{
    //public static class RegisterIoc
    //{
    //    public static void Register()
    //    {
    //        var ioc = IocFactory.IocWrapper;
    //        DataAccess.RegisterIoc.Register();

    //        ioc.RegisterType<Common.IZboxWriteService, ZboxWriteService>();
    //        ioc.RegisterType<Common.IZboxWorkerRoleService, ZboxWorkerRoleService>();
    //        ioc.RegisterType<Common.IZboxServiceBootStrapper, ZboxWriteService>();
    //    }
    //}
    public class WriteServiceModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            //TODO: add unit of work to autofac
            builder.RegisterType<ZboxWriteService>().As<Common.IZboxWriteService>()
                .As<Common.IZboxServiceBootStrapper>().AutoActivate().OnActivated(e => e.Instance.BootStrapper());
            builder.RegisterType<ZboxWorkerRoleService>().As<Common.IZboxWorkerRoleService>();
            builder.RegisterModule<RepositoryModule>();
        }
    }
}
