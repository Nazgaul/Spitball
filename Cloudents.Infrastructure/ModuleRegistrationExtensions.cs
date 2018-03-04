using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Cloudents.Core.Attributes;

namespace Cloudents.Infrastructure
{
    public static class ModuleRegistrationExtensions
    {
        public static void RegisterSystemModules(this ContainerBuilder builder,
            Core.Enum.System system, params Assembly[] assemblies)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            foreach (var assembly in assemblies)
            {
                var modules = assembly.GetTypes()
                    .Where(w => typeof(IModule).IsAssignableFrom(w))
                    .Where(w =>
                    {
                        var info = w.GetTypeInfo().GetCustomAttributes<ModuleRegistrationAttribute>();
                        return info?.FirstOrDefault(f => f.System == system) != null;
                        //return info?.System == system;
                    });
                foreach (var module in modules)
                {
                    var p2 = Activator.CreateInstance(module);
                    if (p2 is IModule p)
                    {
                        builder.RegisterModule(p);
                    }

                }
            }

            //var modules = builder.RegisterAssemblyTypes(assemblies).Where(w =>
            // {
            //     var info = w.GetTypeInfo().GetCustomAttribute<ModuleRegistrationAttribute>();
            //     return info?.System == system;
            // }).As<IModule>();
            
            //ContainerBuilder expr_39 = new ContainerBuilder();
            //using (IContainer container = expr_39.Build(ContainerBuildOptions.None))
            //{
            //    foreach (IModule current in container.Resolve<IEnumerable<IModule>>())
            //    {
            //        registrar.RegisterModule(current);
            //    }
            //}
            //foreach (var module in modules)
            //{
            //    builder.RegisterModule(module);
            //}

            // return new ModuleRegistrar(builder).RegisterAssemblyModules(assemblies);
        }
    }
}