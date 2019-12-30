using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace Cloudents.Web.Services
{
    public class WebPackChunkName
    {
        private readonly IFileProvider _provider;
        private readonly IWebHostEnvironment _environment;
        private readonly ConcurrentDictionary<string, WebPackBundle> _tags = new ConcurrentDictionary<string, WebPackBundle>();

        public WebPackChunkName(IFileProvider provider, IWebHostEnvironment environment)
        {
            _provider = provider;
            _environment = environment;
        }


        public WebPackBundle GetVendorTag()
        {
            if (!_environment.IsDevelopment() && _tags.TryGetValue("vendor", out var webPack))
            {
                return webPack;
            }

            var content = _provider.GetDirectoryContents("wwwroot/dist");

            var files = content.Where(w => w.Name.StartsWith("vendor")).OrderByDescending(o => o.LastModified).ToList();


            var webPackBundle = new WebPackBundle();
            webPackBundle.Js = files.FirstOrDefault(f => f.Name.EndsWith("js"))?.Name;
            webPackBundle.Css = files.FirstOrDefault(f => f.Name.EndsWith("css") && !f.Name.EndsWith("rtl.css"))?.Name;
            webPackBundle.RtlCss = files.FirstOrDefault(f => f.Name.EndsWith("rtl.css"))?.Name;

            _tags.AddOrUpdate("vendor", webPackBundle, (_, existingValue) => existingValue);
            return webPackBundle;
            //var t = _provider.GetFileInfo($"wwwroot/dist/{chunk}.json");
            //if (!t.Exists)
            //{
            //    return new WebPackBundle();
            //}
            //using (var stream = t.CreateReadStream())
            //{
            //    using (var sr = new StreamReader(stream))
            //    {
            //        using (var reader = new JsonTextReader(sr))
            //        {
            //            var obj = JObject.Load(reader);

            //            var webPackBundle = new WebPackBundle();
            //            var chunk2 = obj[chunk];
            //            if (chunk2.Type == JTokenType.Array)
            //            {

            //            }

            //            foreach (var file in chunk2.Values<string>())
            //            {
            //                if (file.EndsWith("js", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    webPackBundle.Js = file;
            //                    continue;
            //                }

            //                if (file.EndsWith("rtl.css", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    webPackBundle.RtlCss = file;
            //                    continue;

            //                }
            //                if (file.EndsWith("css", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    webPackBundle.Css = file;
            //                }
            //            }
            //            _tags.AddOrUpdate(chunk, webPackBundle, (_, existingValue) => existingValue);
            //            return webPackBundle;
            //        }
            //    }
            //}
        }


        public WebPackBundle GetClientTag(bool isRtl)
        {

            const string chunk = "main";

            if (!_environment.IsDevelopment() && _tags.TryGetValue($"{chunk}{isRtl}", out var webPack))
            {
                return webPack;
            }

            //var prefixDirectory = isRtl ? "rtl" : "ltr";
            var content = _provider.GetDirectoryContents($"wwwroot/dist/");

            var files = content.Where(w => w.Name.StartsWith(chunk)).OrderByDescending(o => o.LastModified).ToList();


            var webPackBundle = new WebPackBundle();
            webPackBundle.Js = $"{files.First(f => f.Name.EndsWith("js"))?.Name}";
            //webPackBundle.Css = files.FirstOrDefault(f => f.Name.EndsWith("css") && !f.Name.EndsWith("rtl.css"))?.Name; 
            //webPackBundle.RtlCss = files.FirstOrDefault(f => f.Name.EndsWith("rtl.css"))?.Name; ;

            _tags.AddOrUpdate($"{chunk}{isRtl}", webPackBundle, (_, existingValue) => existingValue);
            return webPackBundle;
            //var t = _provider.GetFileInfo($"wwwroot/dist/{chunk}.json");
            //if (!t.Exists)
            //{
            //    return new WebPackBundle();
            //}
            //using (var stream = t.CreateReadStream())
            //{
            //    using (var sr = new StreamReader(stream))
            //    {
            //        using (var reader = new JsonTextReader(sr))
            //        {
            //            var obj = JObject.Load(reader);

            //            var webPackBundle = new WebPackBundle();
            //            var chunk2 = obj[chunk];
            //            if (chunk2.Type == JTokenType.Array)
            //            {

            //            }

            //            foreach (var file in chunk2.Values<string>())
            //            {
            //                if (file.EndsWith("js", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    webPackBundle.Js = file;
            //                    continue;
            //                }

            //                if (file.EndsWith("rtl.css", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    webPackBundle.RtlCss = file;
            //                    continue;

            //                }
            //                if (file.EndsWith("css", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    webPackBundle.Css = file;
            //                }
            //            }
            //            _tags.AddOrUpdate(chunk, webPackBundle, (_, existingValue) => existingValue);
            //            return webPackBundle;
            //        }
            //    }
            //}
        }
    }

    public class WebPackBundle
    {
        public string Css { get; set; }

        public string RtlCss { get; set; }
        public string Js { get; set; }


        public string GetCss()
        {
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                return RtlCss;
            }

            return Css;
        }
    }
}
