using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Cloudents.Core.Attributes;

namespace Cloudents.Core.Extension
{
    public static class ModuleRegistrationExtensions
    {
        public static void RegisterSystemModules(this ContainerBuilder builder,
            Core.Enum.System system, params Assembly[] assemblies)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            foreach (var assembly in assemblies)
            {
                try
                {
                    var modules = assembly.GetTypes()
                        .Where(w => typeof(IModule).IsAssignableFrom(w))
                        .Where(w => GetModuleAttribute(w, system) != null
                           // var info = w.GetTypeInfo().GetCustomAttributes<ModuleRegistrationAttribute>();
                            //return info.FirstOrDefault(f => f.System == system) != null;
                        ).OrderBy(o => GetModuleAttribute(o,system).Order);
                    foreach (var module in modules)
                    {
                        var p2 = Activator.CreateInstance(module);
                        if (p2 is IModule p)
                        {
                            builder.RegisterModule(p);
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var loaderExceptions = ex.LoaderExceptions;
                    throw loaderExceptions[0];
                }
            }
        }

        private static ModuleRegistrationAttribute GetModuleAttribute(Type type, Enum.System system)
        {
            var info = type.GetTypeInfo().GetCustomAttributes<ModuleRegistrationAttribute>();
            return info.FirstOrDefault(f => f.System == system);
        }
    }
}