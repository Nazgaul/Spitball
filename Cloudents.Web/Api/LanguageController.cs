using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class LanguageController : ControllerBase
    {
        // GET
        [HttpGet]
        public Dictionary<string,string> Get()
        {
            var assembly = Assembly.GetAssembly(typeof(Cloudents.Web.Resources.Js.FaqBlock));

            var resources = assembly.GetManifestResourceNames();

            var dic = new Dictionary<string, string>();
            foreach (var rawResourceLocation in resources.Where(w => w.Contains("Cloudents.Web.Resources.Js")))
            {
                var resourceStr = rawResourceLocation.Substring(0, rawResourceLocation.LastIndexOf('.'));
                var resource = new ResourceManager(resourceStr, assembly);
                var resourceSet = resource.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

                var p = resourceSet.Cast<DictionaryEntry>()
                    .ToDictionary(x => x.Key.ToString(),
                        x => x.Value.ToString());


                var name = resourceStr.Replace("Cloudents.Web.Resources.Js.", string.Empty);
                foreach (var val in p)
                {
                    dic.Add($"{Char.ToLowerInvariant(name[0]) + name.Substring(1)}_{val.Key}", val.Value);

                }
            }
            return dic;
        }
    }
}