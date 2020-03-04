using Cloudents.Core;
using Cloudents.Core.Extension;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    public class LocaleController : Controller
    {
        private static readonly ConcurrentDictionary<(CultureInfo, string), Dictionary<string, string>> CacheDictionary = new ConcurrentDictionary<(CultureInfo, string), Dictionary<string, string>>();

        private static readonly Regex DefaultRegex =
            new Regex(@"Cloudents.Web.Resources.Js.([A-Za-z0-9\-]+).resources", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private static Dictionary<string, string> ParseJsResource(string location)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            var dic = new Dictionary<string, string>();

            var regex = DefaultRegex;
            if (location != null)
            {
                regex = new Regex($@"Cloudents.Web.Resources.Js.{location}.([A-Za-z0-9\-]+).resources", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            }
            foreach (var rawResourceLocation in resources.Where(w => regex.Match(w).Success)) // w.Contains("Cloudents.Web.Resources.Js")))
            {
                var resourceStr = rawResourceLocation.Substring(0, rawResourceLocation.LastIndexOf('.'));
                var resourceManager = new ResourceManager(resourceStr, assembly);

                var resourceSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);


                var p = resourceSet.Cast<DictionaryEntry>()
                    .ToDictionary(x => x.Key.ToString(),
                        x => resourceManager.GetString(x.Key.ToString()));

                var index = resourceStr.LastIndexOf('.') + 1;
                var name = resourceStr.Remove(0, index);
                foreach (var (key, value) in p)
                {
                    dic.Add($"{name.ToCamelCase()}_{key}", value);
                }
            }

            return dic;
        }

        /// <summary>
        /// Get resource for js files 
        /// </summary>
        /// <param name="resource">The key to the resource</param>
        /// <remarks>Note please pass the version and also the locale(language) - since the browser caches the data</remarks>
        /// <returns>A key value dictionary</returns>
        [HttpGet]
        [ResponseCache(Duration = TimeConst.Week, VaryByQueryKeys = new[] { "*" })]
        public Dictionary<string, string> Get(string resource)
        {
            var dic = CacheDictionary.GetOrAdd((CultureInfo.CurrentUICulture, resource), _ => ParseJsResource(resource));
            return dic;
        }
    }
}