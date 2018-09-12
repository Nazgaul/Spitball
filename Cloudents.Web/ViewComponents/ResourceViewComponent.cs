using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace Cloudents.Web.ViewComponents
{
    public class ResourceViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();

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
                    dic.Add($"{char.ToLowerInvariant(name[0]) + name.Substring(1)}_{val.Key}", val.Value);

                }
            }
            var jsonString = JsonConvert.SerializeObject(dic);
            return Task.FromResult<IViewComponentResult>(View("Default", jsonString));
        }
    }

}

