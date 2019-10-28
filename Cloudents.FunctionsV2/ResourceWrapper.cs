using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private static ResourceManager projectManager;// = new ResourceManager();
        static ResourceWrapper()
        {

            LoadLocalizationAssemblies();

            projectManager = new ResourceManager(typeof(App));

            //return x.GetString(val.ResourceName, CultureInfo.CurrentUICulture);
            //resourceManager = new ResourceManager()
            //  ResourceSets.Add(new CultureInfo("en-IN"), Load("en-IN"));
            //            ResourceSets.Add(new CultureInfo("he"), Load("he"));
            //ResourceSets.Add(new CultureInfo("en"), Load("en"));
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

                var asm = Assembly.LoadFrom(file);
                var loadedAssembly = asm.GetName().Name;
                var currentAssembly = typeof(App).Assembly.GetName().Name;
                if (loadedAssembly.StartsWith(currentAssembly))
                {
                    var v = file.Split('\\').FirstOrDefault(w => regEx.IsMatch(w) && w.Split('-')[0].Length == 2);

                    var manifestResource = asm.GetManifestResourceNames();

                    var x = new ResourceSet(asm.GetManifestResourceStream(manifestResource.First()));

                    // _tags.AddOrUpdate("vendor", webPackBundle, (_, existingValue) => existingValue);
                    ResourceSets.AddOrUpdate(new CultureInfo(v),x, (_,x3) => x3);
                    //ResourceSets.Add(new CultureInfo(v), x);
                }

                // var x = new ResourceManager(assmebly);
            }


        }

        //private static ResourceSet Load(string lang)
        //{

        //    //var asm = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "bin", lang, "Function.App.resources.dll"));
        //    //var resourceName = $"Function.App.Resources.Emails.{lang}.resources";
        //    //var tt = asm.GetManifestResourceNames();
        //    //return new ResourceSet(asm.GetManifestResourceStream(resourceName));
        //    var dir = Path.GetDirectoryName(typeof(ResourceWrapper).Assembly.Location);
        //    var parent = dir.Remove(dir.IndexOf(@"\bin"));
        //    var combine = Path.Combine(parent, lang, "Cloudents.FunctionsV2.resources.dll");
        //    var asm = Assembly.LoadFrom(combine);
        //    var resourceName = $"Cloudents.FunctionsV2.Resources.App.{lang}.resources";
        //    return new ResourceSet(asm.GetManifestResourceStream(resourceName));
        //}

        public static string GetString(string key)
        {
            if (ResourceSets.TryGetValue(CultureInfo.DefaultThreadCurrentUICulture, out var v))
            {
                return v.GetString(key);
            }
            return projectManager.GetString(key);
            //return null;
            // return ResourceSets[CultureInfo.CurrentUICulture].GetString(key);
        }


    }
}
