using System;
using System.Collections.Concurrent;
using System.IO;
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
                        foreach (var file in obj[chunk].Values<string>())
                        {
                            if (file.EndsWith("js", StringComparison.OrdinalIgnoreCase))
                            {
                                webPackBundle.Js = file;
                            }
                            if (file.EndsWith("css", StringComparison.OrdinalIgnoreCase))
                            {
                                webPackBundle.Css = file;
                            }
                        }
                        _tags.AddOrUpdate(chunk, webPackBundle, (key, existingValue) => existingValue);
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
