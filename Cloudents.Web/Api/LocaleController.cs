using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Cloudents.Core.Extension;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class LocaleController : ControllerBase
    {
        private static readonly ConcurrentDictionary<CultureInfo, Dictionary<string, string>> CacheDictionary = new ConcurrentDictionary<CultureInfo, Dictionary<string, string>>();

        //public Task<IViewComponentResult> InvokeAsync()
        //{
        //    var jsonString = CacheDictionary.GetOrAdd(CultureInfo.CurrentUICulture, _ => ParseJsResource());
        //    return Task.FromResult<IViewComponentResult>(View("Default", jsonString));
        //}

        private static Dictionary<string, string> ParseJsResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            var dic = new Dictionary<string, string>();
            foreach (var rawResourceLocation in resources.Where(w => w.Contains("Cloudents.Web.Resources.Js")))
            {
                var resourceStr = rawResourceLocation.Substring(0, rawResourceLocation.LastIndexOf('.'));
                var resource = new ResourceManager(resourceStr, assembly);

                var resourceSet = resource.GetResourceSet(CultureInfo.InvariantCulture, true, true);


                var p = resourceSet.Cast<DictionaryEntry>()
                    .ToDictionary(x => x.Key.ToString(),
                        x => resource.GetString(x.Key.ToString()));

                var name = resourceStr.Replace("Cloudents.Web.Resources.Js.", string.Empty);
                foreach (var val in p)
                {
                    dic.Add($"{name.ToCamelCase()}_{val.Key}", val.Value);
                }
            }

            return dic;
        }

        // GET
        [HttpGet]
        public Dictionary<string, string> Get(string locale)
        {
            var dic = CacheDictionary.GetOrAdd(CultureInfo.CurrentUICulture, _ => ParseJsResource());
            return dic;
        }
    }
}