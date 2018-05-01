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
                throw new ArgumentNullException(nameof(builder));
            }

            foreach (var assembly in assemblies)
            {
                try
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
                catch (ReflectionTypeLoadException ex)
                {
                    var loaderExceptions = ex.LoaderExceptions;
                    throw;
                }

            }
        }
    }
}