using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Cloudents.FunctionsV2
{
    //https://github.com/Azure/Azure-Functions/issues/581
    public static class ResourceWrapper
    {
        private static Dictionary<string, ResourceSet> _resourceSets = new Dictionary<string, ResourceSet>();
        static ResourceWrapper()
        {
            _resourceSets.Add("he", Load("he"));
            _resourceSets.Add("en", Load("en"));
        }

        private static ResourceSet Load(string lang)
        {
            var asm = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "bin", lang, "Cloudents.FunctionsV2.resources.dll"));
            var resourceName = $"Cloudents.FunctionsV2.Resources.App.{lang}.resources";
           //var tt = asm.GetManifestResourceNames();
            return new ResourceSet(asm.GetManifestResourceStream(resourceName));
        }

        public static string GetString(string lang, string key)
        {
            return _resourceSets[lang].GetString(key);
        }
    }
}
