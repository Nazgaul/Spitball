using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using SquishIt.Framework.JavaScript;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class BundleConfig
    {
        private static readonly Dictionary<string, string> CssBundles = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> JsBundles = new Dictionary<string, string>();

        private static readonly string CdnLocation = GetValueFromCloudConfig();

        public static string CssLink(string key)
        {
            string retVal;
            //we except that some values won't exists
            CssBundles.TryGetValue(key, out retVal);

            return retVal;
        }
        public static string JsLink(string key)
        {
            string value;
            if (!JsBundles.TryGetValue(key, out value))
            {
                throw new KeyNotFoundException(key);
            }
            return value;
        }






        public static string CdnEndpointUrl
        {
            get
            {
                return CdnLocation;
            } 
        }

        public static void RegisterBundle(
            IDictionary<string, IEnumerable<string>> registeredCssBundles,
            IDictionary<string, IEnumerable<JsFileWithCdn>> registeredJsBundles
            )
        {

            if (registeredCssBundles != null)
            {
                foreach (var registeredCssBundle in registeredCssBundles)
                {
                    RegisterCss(registeredCssBundle.Key, registeredCssBundle.Value);
                }
            }
            if (registeredJsBundles != null)
            {
                foreach (var registeredJsBundle in registeredJsBundles)
                {
                    RegisterJsRegular(registeredJsBundle.Key, registeredJsBundle.Value);
                }
            }



            CopyFilesToCdn("/Content", "*.min.css");
            CopyFilesToCdn("/Content", "*.png");
            CopyFilesToCdn("/Content", "*.jpg");
            CopyFilesToCdn("/Content", "*.gif");
            CopyFilesToCdn("/Images", "*.*");
            CopyFilesToCdn("/gzip/", "*.*", SearchOption.TopDirectoryOnly);

        }

        public static bool IsDebugEnabled()
        {
            return HttpContext.Current.Request.IsLocal || string.IsNullOrWhiteSpace(CdnLocation);
        }

        private static void RegisterCss(string key, IEnumerable<string> cssFiles)
        {
            var cssBundle = SquishIt.Framework.Bundle.Css();
            cssBundle.WithReleaseFileRenderer(new SquishItRenderer());
            foreach (var cssFile in cssFiles)
            {
                cssBundle.Add(cssFile);
            }

            var cdnUrl = CdnLocation;
            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {
                cssBundle.WithOutputBaseHref(cdnUrl);
                CssBundles.Add(key, cssBundle.Render("~/gzip/c#.css"));
                CopyFilesToCdn("~/gzip/", "*.css", SearchOption.TopDirectoryOnly);
            }
            else
            {
                CssBundles.Add(key, cssBundle.Render("~/cdn/gzip/c#.css"));
            }


        }

        private static string RegisterJs(IEnumerable<JsFileWithCdn> jsFiles, JavaScriptBundle javaScriptBundleImp)
        {
            var jsBundle = javaScriptBundleImp;
            jsBundle.WithReleaseFileRenderer(new SquishItRenderer());
            foreach (var jsFile in jsFiles)
            {
                if (string.IsNullOrWhiteSpace(jsFile.CdnFile))
                {
                    if (jsFile.LocalFile.Contains("min"))
                    {
                        jsBundle.AddMinified(jsFile.LocalFile);
                    }
                    else
                    {
                        jsBundle.Add(jsFile.LocalFile);
                    }
                }
                else
                {
                    jsBundle.AddRemote(jsFile.LocalFile, jsFile.CdnFile);
                }
            }
            var cdnUrl = CdnLocation;

            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {
                jsBundle.WithOutputBaseHref(cdnUrl);
                CopyFilesToCdn("~/gzip/", "*.js", SearchOption.TopDirectoryOnly);
                return jsBundle.Render("~/gzip/j#.js");
            }
            return jsBundle.Render("~/cdn/gzip/j#.js");
        }

        private static void RegisterJsRegular(string key, IEnumerable<JsFileWithCdn> jsFiles)
        {
            JsBundles.Add(key, RegisterJs(jsFiles, SquishIt.Framework.Bundle.JavaScript()));
        }

        private static string GetValueFromCloudConfig()
        {
            if (!RoleEnvironment.IsAvailable)
            {
                return string.Empty;
            }
            try
            {

                return RoleEnvironment.GetConfigurationSettingValue("CdnEndpoint");
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }


        private static void CopyFilesToCdn(string directoryRelativePath, string fileSearchOption, SearchOption options = SearchOption.AllDirectories)
        {
            var server = HttpContext.Current.Server;
            var appRoot = server.MapPath("~/");
            var cdnRoot = server.MapPath("~/cdn");

            var directoryPath = server.MapPath(directoryRelativePath);
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var filesPath = Directory.GetFiles(directoryPath, fileSearchOption, options);

            foreach (var filePath in filesPath)
            {
                var cdnFilePath = string.Empty;
                try
                {
                    var relativePath = filePath.Replace(appRoot, string.Empty);
                    cdnFilePath = Path.Combine(cdnRoot, relativePath);
                    if (File.Exists(Path.Combine(cdnRoot, relativePath)))
                    {
                        continue;
                    }
                    var directory = Path.GetDirectoryName(cdnFilePath);
                    if (directory != null) Directory.CreateDirectory(directory);
                    File.Copy(filePath, Path.Combine(cdnRoot, relativePath));
                }
                catch (Exception ex)
                {
                    Zbox.Infrastructure.Trace.TraceLog.WriteError(string.Format("On Copy Files to cdn filePath: {0} cdnFilePath: {1}", filePath, cdnFilePath), ex);
                }
            }
        }

        

    }
    public class JsFileWithCdn
    {
        public JsFileWithCdn(string localFile)
        {
            LocalFile = localFile;
        }
        // ReSharper disable once UnusedMember.Local
        public JsFileWithCdn(string localFile, string cdnFile)
        {
            LocalFile = localFile;
            CdnFile = cdnFile;
        }
        public string LocalFile { get; private set; }
        public string CdnFile { get; private set; }
    }
}