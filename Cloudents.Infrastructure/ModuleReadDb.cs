using System.Reflection;
using Autofac;
using Cloudents.Application.Attributes;
using Cloudents.Application.Interfaces;
using Cloudents.Infrastructure.Data;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Application.Enum.System.Web)]
    [ModuleRegistration(Application.Enum.System.MailGun)]
    [ModuleRegistration(Application.Enum.System.Console)]
    public class ModuleReadDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            builder.RegisterType<DapperRepository>().AsSelf();
            builder.RegisterAssemblyTypes(currentAssembly).AsClosedTypesOf(typeof(IReadRepositoryAsync<,>));
            builder.RegisterAssemblyTypes(currentAssembly).AsClosedTypesOf(typeof(IReadRepositoryAsync<>));
        }
    }
}