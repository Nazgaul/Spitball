using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
//using System.Web.Optimization;
using Microsoft.WindowsAzure.ServiceRuntime;
using SquishIt.Framework.JavaScript;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class BundleConfig
    {
        public const string Rtl = ".rtl";

        //public const string Rtl2 = "-rtl";

        private static readonly Dictionary<string, string> CssBundles = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> JsBundles = new Dictionary<string, string>();

        private static readonly string CdnLocation = GetValueFromCloudConfig();

        //static BundleConfig()
        //{

        //}
        //public static void RegisterBundles(
        //    IEnumerable<KeyValuePair<string, IEnumerable<CssWithRtl>>> cssBundles,
        //    IEnumerable<KeyValuePair<string, IEnumerable<JsFileWithCdn>>> jsBundles)
        //{
        //    var bundles = BundleTable.Bundles;
        //    bundles.IgnoreList.Ignore("*.min.css", OptimizationMode.Always);
        //    //bundles.IgnoreList.Ignore("*.min.js", OptimizationMode.Always);
        //    var cdnUrl = CdnEndpointUrl + "/{0}?" + VersionHelper.CurrentVersion(true);
        //    bundles.UseCdn = true;
        //    RegisterCssBundles(bundles, cdnUrl, cssBundles);
        //    RegisterJsBundle(bundles, cdnUrl, jsBundles);
        //    //BundleTable.EnableOptimizations = true;
        //}
        //public static void RegisterCssBundles(BundleCollection bundles, string cdnUrl,
        //    IEnumerable<KeyValuePair<string, IEnumerable<CssWithRtl>>> cssBundles)
        //{

        //    foreach (var cssBundle in cssBundles)
        //    {
        //        var styleLeft = new YUIStyleBundle("~/bundles/" + cssBundle.Key, string.Format(cdnUrl, "bundles/" + cssBundle.Key))
        //            .IncludeFallback("~/" + cssBundle.Key, "cssCheck", "absolute", "-2000px");

        //        var styleRight = new YUIStyleBundle("~/bundles/" + cssBundle.Key + Rtl2, string.Format(cdnUrl, "bundles/" + cssBundle.Key + Rtl2))
        //            .IncludeFallback("~/" + cssBundle.Key + Rtl2, "cssCheck", "absolute", "-2000px");

        //        foreach (var bundle in cssBundle.Value)
        //        {
        //            styleLeft.Include(bundle.LeftCssFile);
        //            styleRight.Include(bundle.RightCssFile);
        //        }

        //        bundles.Add(styleLeft);
        //        bundles.Add(styleRight);
        //    }
        //    bundles.Add(new StyleBundle("~/bundles/angularMaterial",
        //        "https://ajax.googleapis.com/ajax/libs/angular_material/1.1.1/angular-material.min.css")
        //        .IncludeFallback("~/angularMaterial", "md-display-4", "font-size", "112px")
        //        .Include("~/content/angular-material.css")
        //        );
        //}

        //public static void RegisterJsBundle(BundleCollection bundles, string cdnUrl,
        //    IEnumerable<KeyValuePair<string, IEnumerable<JsFileWithCdn>>> jsBundles)
        //{

        //    foreach (var jsBundle in jsBundles)
        //    {
        //        var scriptBundle = new ScriptBundle("~/bundles/" + jsBundle.Key, string.Format(cdnUrl, "bundles/" + jsBundle.Key));
        //        foreach (var bundle in jsBundle.Value)
        //        {
        //            scriptBundle.Include(bundle.LocalFile);
        //        }
        //        bundles.Add(scriptBundle);

        //        //scriptBundle
        //    }
        //    foreach (var language in Languages.SupportedCultures)
        //    {
        //        foreach (var culture in language.Culture)
        //        {
        //            ////"langText." + culture
        //            var scriptBundle = new ScriptBundle("~/bundles/" + "langText-" + culture, string.Format(cdnUrl, "bundles/" + "langText-" + culture));

        //            //foreach (var bundle in jsBundle.Value)
        //            //{
        //            //    scriptBundle.Include(bundle.LocalFile);
        //            //}
        //            //bundles.Add(scriptBundle);
        //            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
        //            //var angularResource =
        //            //    $"{HttpContext.Current.Server.MapPath("/Scripts/i18n/angular-locale")}_{Thread.CurrentThread.CurrentUICulture.Name}.js";
        //            scriptBundle.Include($"~/Scripts/i18n/angular-locale_{Thread.CurrentThread.CurrentUICulture.Name}.js");
        //            string path = $"~/js/sbResources_{culture}.js";
        //            File.WriteAllText(HttpContext.Current.Server.MapPath(path), JsResourceHelper.BuildResourceObject());
        //            scriptBundle.Include(path);
        //            bundles.Add(scriptBundle);
        //            //RegisterLocaleJs(File.ReadAllText(angularResource),
        //            //    JsResourceHelper.BuildResourceObject(), culture);
        //        }

        //    }

        //    bundles.Add(new ScriptBundle("~/bundles/jquery", "https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js")
        //    {
        //        //CdnFallbackExpression = "window.jquery"
        //    }
        //        .Include("~/scripts/jquery-2.2.0.js"));
        //    bundles.Add(new ScriptBundle("~/bundles/angular", "https://ajax.googleapis.com/ajax/libs/angularjs/1.5.8/angular.min.js")
        //    {
        //        CdnFallbackExpression = "window.angular"
        //    }
        //        .Include("~/scripts/angular.js"));

        //    bundles.Add(new ScriptBundle("~/bundles/angularMaterialJs", "https://ajax.googleapis.com/ajax/libs/angular_material/1.1.1/angular-material.min.js")
        //    {
        //        // CdnFallbackExpression = "window.angular"
        //    }
        //        .Include("~/scripts/angular-material/angular-material.js"));

        //    var quizCreateBundle = new ScriptBundle("~/bundles/quizCreate")
        //    {
        //        // CdnFallbackExpression = "window.angular"
        //    }
        //        .Include("~/bower_components/textAngular/dist/textAngular-rangy.min.js")
        //        .Include("~/bower_components/textAngular/dist/textAngular.js")
        //        .Include("~/scripts/textAngularSetup.js")
        //        .Include("~/js/components/quiz/quizCreate.module.js")
        //        .Include("~/js/components/quiz/quizCreate2.controller.js");
        //    bundles.Add(quizCreateBundle);
        //}

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

        public static string CdnEndpointUrl => CdnLocation;

        public static void RegisterBundle(
            IEnumerable<KeyValuePair<string, IEnumerable<CssWithRtl>>> registeredCssBundles,
            IEnumerable<KeyValuePair<string, IEnumerable<JsFileWithCdn>>> registeredJsBundles
            )
        {
            if (registeredCssBundles != null)
            {
                foreach (var registeredCssBundle in registeredCssBundles)
                {
                    RegisterCss(registeredCssBundle.Key, registeredCssBundle.Value.Select(s => s.LeftCssFile));
                    RegisterCss(registeredCssBundle.Key + Rtl, registeredCssBundle.Value.Select(s => s.RightCssFile));
                }
            }
            if (registeredJsBundles != null)
            {
                RegisterJs("quizCreate", new[]
                {
                    new JsFileWithCdn("~/bower_components/textAngular/dist/textAngular-rangy.min.js"),
                    new JsFileWithCdn("~/bower_components/textAngular/dist/textAngular.js"),
                    new JsFileWithCdn("~/scripts/textAngularSetup.js"),
                    new JsFileWithCdn("~/js/components/quiz/quizCreate.module.js"),
                    new JsFileWithCdn("~/js/components/quiz/quizCreate2.controller.js")
                });

                CreateLazyLoadScript("~/js/components/quiz/quizCreate.config.js", "quizCreate", "~/js/components/quiz/quizCreate1.config.js");
                // CreateLazyLoadScript("~/js/components/box/upload.config.js", "upload", "~/js/components/box/upload1.config.js");

                foreach (var registeredJsBundle in registeredJsBundles)
                {
                    RegisterJs(registeredJsBundle.Key, registeredJsBundle.Value);
                }
            }
            foreach (var language in Languages.SupportedCultures)
            {
                foreach (var culture in language.Culture)
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
                    var angularResource =
                        $"{HttpContext.Current.Server.MapPath("/Scripts/i18n/angular-locale")}_{Thread.CurrentThread.CurrentUICulture.Name}.js";
                    RegisterLocaleJs(File.ReadAllText(angularResource),
                        JsResourceHelper.BuildResourceObject(), culture);
                }
            }

            RelativePathContent("~/js/components/quiz/quizCreate.config.js");

            CopyFilesToCdn("/Content", "*.min.css");
            CopyFilesToCdn("/Content", "*.png");
            CopyFilesToCdn("/Content", "*.jpg");
            CopyFilesToCdn("/Content", "*.gif");

            CopyFilesToCdn("/Content/Fonts", "*.*");
            CopyFilesToCdn("/Images", "*.*");
            CopyFilesToCdn("/gzip/", "*.*", options: SearchOption.TopDirectoryOnly);
        }

        private static void CreateLazyLoadScript(string configFile, string bundleName, string outFile)
        {
            var scriptToLoad = new List<string>();
            var quizCreateScript = RelativePathContent(configFile);
            foreach (
                Match match in
                    Regex.Matches(JsBundles[bundleName], "<script.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase))
            {
                scriptToLoad.Add(match.Groups[1].Value);
            }

            quizCreateScript = quizCreateScript.Replace("{0}", string.Join(",", scriptToLoad.Select(
                s => $"'{s}'")));
            File.WriteAllText(HttpContext.Current.Server.MapPath(outFile),
                quizCreateScript);
        }

        private static string RelativePathContent(string relativePath)
        {
            var path = HttpContext.Current.Server.MapPath(relativePath);
            return File.ReadAllText(path);
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
                if (!string.IsNullOrEmpty(cssFile))
                {
                    cssBundle.Add(cssFile);
                }
            }

            var cdnUrl = CdnLocation;
            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {
                cssBundle.WithOutputBaseHref(cdnUrl);
                CssBundles.Add(key, cssBundle.Render("~/gzip/c#.css"));
                CopyFilesToCdn("~/gzip/", "*.css", options: SearchOption.TopDirectoryOnly);
            }
            else
            {
                CssBundles.Add(key, cssBundle.Render("~/cdn/gzip/c#.css"));
            }
        }

        private static void RegisterLocaleJs(string angularPath, string jsResourceString, string culture)
        {
            var jsBundle = CreateJsBundle();
            jsBundle.AddString(angularPath);
            jsBundle.AddString(jsResourceString);

            RenderBundles("langText." + culture, jsBundle);
        }

        private static JavaScriptBundle CreateJsBundle()
        {
            var jsBundle = SquishIt.Framework.Bundle.JavaScript();
            jsBundle.WithReleaseFileRenderer(new SquishItRenderer());
#if (!DEBUG)
            jsBundle.WithDeferredLoad();
#endif
            return jsBundle;
        }

        private static void RenderBundles(string key, JavaScriptBundle bundle)
        {
            var cdnUrl = CdnLocation;
            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {
                bundle.WithOutputBaseHref(cdnUrl);
                CopyFilesToCdn("~/gzip/", "*.js", options: SearchOption.TopDirectoryOnly);

                JsBundles.Add(key, bundle.Render("~/gzip/j#.js"));
                return;
            }
            JsBundles.Add(key, bundle.Render("~/cdn/gzip/j#.js"));
        }

        private static void RegisterJs(string key, IEnumerable<JsFileWithCdn> jsFiles)
        {
            var jsBundle = CreateJsBundle();
            //jsBundle.WithDeferredLoad();
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
            RenderBundles(key, jsBundle);
            //var cdnUrl = CdnLocation;

            //if (!string.IsNullOrWhiteSpace(cdnUrl))
            //{
            //    jsBundle.WithOutputBaseHref(cdnUrl);
            //    CopyFilesToCdn("~/gzip/", "*.js", options: SearchOption.TopDirectoryOnly);
            //    return jsBundle.Render("~/gzip/j#.js");
            //}
            //return jsBundle.Render("~/cdn/gzip/j#.js");
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

        private static void CopyFilesToCdn(string directoryRelativePath, string fileSearchOption, string cdnRelativePath = null, SearchOption options = SearchOption.AllDirectories)
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
                    if (!string.IsNullOrEmpty(cdnRelativePath))
                    {
                        relativePath = Path.Combine(cdnRelativePath, Path.GetFileName(filePath));
                    }

                    cdnFilePath = Path.Combine(cdnRoot, relativePath);

                    if (File.Exists(Path.Combine(cdnRoot, relativePath)))
                    {
                        continue;
                    }
                    var directory = Path.GetDirectoryName(cdnFilePath);
                    if (directory != null) Directory.CreateDirectory(directory);
                    File.Copy(filePath, Path.Combine(cdnRoot, relativePath), true);
                }
                catch (Exception ex)
                {
                    Zbox.Infrastructure.Trace.TraceLog.WriteError(
                        $"On Copy Files to cdn filePath: {filePath} cdnFilePath: {cdnFilePath}", ex);
                }
            }
        }
    }
}