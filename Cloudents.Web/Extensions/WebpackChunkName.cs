using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cloudents.Web.Extensions
{
    public class WebPackChunkName
    {
        private readonly IFileProvider _provider;
        private readonly ConcurrentDictionary<string, WebPackBundle> _tags = new ConcurrentDictionary<string, WebPackBundle>();


        public WebPackChunkName(IFileProvider provider)
        {
            _provider = provider;
        }

        public WebPackBundle GetTag(string chunk)
        {
            if (_tags.TryGetValue(chunk, out var webPack))
            {
                return webPack;
            }
            var t = _provider.GetFileInfo($"wwwroot/dist/{chunk}.json");
            if (!t.Exists)
            {
                return new WebPackBundle();
            }
            using (var stream = t.CreateReadStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    using (var reader = new JsonTextReader(sr))
                    {
                        var obj = JObject.Load(reader);
                        
                        var webPackBundle = new WebPackBundle();
                        var files = obj[chunk].Values<string>();
                        foreach (var z in obj.Values())
                        {
                            
                        }
                        foreach (var file in files)
                        {
                            if (file.EndsWith("js", StringComparison.InvariantCultureIgnoreCase))
                            {
                                webPackBundle.Js = file;
                            }
                            if (file.EndsWith("css", StringComparison.InvariantCultureIgnoreCase))
                            {
                                webPackBundle.Css = file;
                            }
                        }
                        _tags.AddOrUpdate(chunk, webPackBundle, (key, existingValue) => existingValue);
                        //_tags.Add(chunk, webPackBundle);
                        return webPackBundle;
                    }
                }
            }
        }
    }

    public class WebPackBundle
    {
        public string Css { get; set; }
        public string Js { get; set; }
    }
}
