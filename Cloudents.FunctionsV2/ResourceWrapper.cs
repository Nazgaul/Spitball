using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using Cloudents.FunctionsV2.Resources;

namespace Cloudents.FunctionsV2
{
    //https://github.com/Azure/Azure-Functions/issues/581
    public static class ResourceWrapper
    {
        private static readonly ConcurrentDictionary<CultureInfo, ResourceSet> ResourceSets = new ConcurrentDictionary<CultureInfo, ResourceSet>();
        private static readonly ResourceManager ProjectManager;// = new ResourceManager();
        static ResourceWrapper()
        {

            LoadLocalizationAssemblies();

            ProjectManager = new ResourceManager(typeof(App));
           
        }

        private static void LoadLocalizationAssemblies()
        {
            // var binDir = Path.Combine(Environment.CurrentDirectory);
            var regEx = new Regex("^[A-Za-z]{1,8}(-[A-Za-z0-9]{1,8})*$");
            var dir = Path.GetDirectoryName(typeof(ResourceWrapper).Assembly.Location);
            var parent = dir.Remove(dir.IndexOf(@"\bin"));
            var assemblyFiles = Directory.EnumerateFiles(parent, "*.resources.dll", SearchOption.AllDirectories);
            foreach (var file in assemblyFiles.Distinct())
            {

                try
                {
                    var asm = Assembly.LoadFrom(file);
                    var loadedAssembly = asm.GetName().Name;
                    var currentAssembly = typeof(App).Assembly.GetName().Name;
                    if (loadedAssembly.StartsWith(currentAssembly))
                    {
                        var v = file.Split('\\').FirstOrDefault(w => regEx.IsMatch(w) && w.Split('-')[0].Length == 2);

                        var manifestResource = asm.GetManifestResourceNames();

                        var x = new ResourceSet(asm.GetManifestResourceStream(manifestResource.First()));
                        ResourceSets.AddOrUpdate(new CultureInfo(v), x, (_, x3) => x3);
                    }
                }
                catch (FileLoadException e)
                {
                    //Happen in unit test.
                }
            }
        }

     

        public static string GetString(string key)
        {
            
            var culture = CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.CurrentCulture;
            if (ResourceSets.TryGetValue(culture, out var v))
            {
                return v.GetString(key);
            }
            return ProjectManager.GetString(key);
        }

        //private static string GetStringRecursion(string key, CultureInfo info)
        //{
        //    while (info != null)
        //    {
        //        if (ResourceSets.TryGetValue(info, out var v))
        //        {
        //            var result =  v.GetString(key);
        //            if (result != null)
        //            {
        //                return result;
        //            }
        //        }

        //        //if (Templates.TryGetValue(info, out var template))
        //        //{
        //        //    return template;
        //        //}

        //        if (Equals(info, info.Parent))
        //        {
        //            break;
        //        }
        //        info = info.Parent;
        //    }

        //    return null;

        //}


    }
}
