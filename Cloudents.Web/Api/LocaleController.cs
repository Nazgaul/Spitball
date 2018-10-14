﻿using Cloudents.Core;
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
    [Route("api/[controller]"), ApiController]
    public class LocaleController : ControllerBase
    {
        private static readonly ConcurrentDictionary<(CultureInfo, string), Dictionary<string, string>> CacheDictionary = new ConcurrentDictionary<(CultureInfo, string), Dictionary<string, string>>();

        private static readonly Regex DefaultRegex =
            new Regex(@"Cloudents.Web.Resources.Js.([A-Za-z0-9\-]+).resources", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        //public Task<IViewComponentResult> InvokeAsync()
        //{
        //    var jsonString = CacheDictionary.GetOrAdd(CultureInfo.CurrentUICulture, _ => ParseJsResource());
        //    return Task.FromResult<IViewComponentResult>(View("Default", jsonString));
        //}

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
        [ResponseCache(Duration = TimeConst.Week, VaryByQueryKeys = new[] { "*" })]
        public Dictionary<string, string> Get(string locale, string resource)
        {
            var dic = CacheDictionary.GetOrAdd((CultureInfo.CurrentUICulture, resource), _ => ParseJsResource(resource));
            return dic;
        }
    }
}