using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using SquishIt.Framework.JavaScript;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Culture;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class BundleConfig
    {
        public const string Rtl = ".rtl";
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

                RegisterJs("box", new[]
                {
                     new JsFileWithCdn("~/bower_components/plupload/js/moxie.min.js"),
                     new JsFileWithCdn("~/bower_components/plupload/js/plupload.dev.js"),
                     new JsFileWithCdn("~/bower_components/angular-plupload/src/angular-plupload.js"),
                     new JsFileWithCdn("~/scripts/draganddrop.js"),
                     new JsFileWithCdn("~/js/components/box/box.module.js"),
                     new JsFileWithCdn("~/js/components/box/box.controller.js"),
                        new JsFileWithCdn("~/js/components/box/tab.controller.js"),
                        new JsFileWithCdn("~/js/components/box/shareBox.directive.js"),
                        new JsFileWithCdn("~/js/components/box/feed.controller.js"),
                        new JsFileWithCdn("~/js/components/box/feed.likes.controller.js"),
                        new JsFileWithCdn("~/js/components/box/item.controller.js"),
                        new JsFileWithCdn("~/js/components/box/quizzes.controller.js"),
                        new JsFileWithCdn("~/js/components/box/members.controller.js"),
                        
                        new JsFileWithCdn("~/js/components/box/recommended.controller.js"),
                        new JsFileWithCdn("~/js/components/box/slideit.directive.js"),
                         new JsFileWithCdn("~/js/components/item/upload.controller.js"),
                        new JsFileWithCdn("~/js/components/item/externalProviderUpload.service.js"),
                });


                CreateLazyLoadScript("~/js/components/quiz/quizCreate.config.js", "quizCreate", "~/js/components/quiz/quizCreate1.config.js");
                CreateLazyLoadScript("~/js/components/box/box.config.js", "box", "~/js/components/box/box1.config.js");

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




            //var jsBundle = CreateJsBundle();
            ////QuizCreate
            //jsBundle.AddString(RelativePathContent("~/bower_components/textAngular/dist/textAngular-rangy.min.js"));
            //jsBundle.AddString(RelativePathContent("~/scripts/textAngularSetup.js"));
            //jsBundle.AddString(RelativePathContent("~/bower_components/textAngular/dist/textAngular.js"));
            //jsBundle.AddString(RelativePathContent("~/js/components/quiz/quizCreate.module.js"));
            //jsBundle.AddString(RelativePathContent("~/js/components/quiz/quizCreate.controller.js"));

            //RenderBundles("quizCreate", jsBundle);



            CopyFilesToCdn("/Content", "*.min.css");
            CopyFilesToCdn("/Content", "*.png");
            CopyFilesToCdn("/Content", "*.jpg");
            CopyFilesToCdn("/Content", "*.gif");

            CopyFilesToCdn("/Content/Fonts", "*.*");
            CopyFilesToCdn("/Images", "*.*");
            CopyFilesToCdn("/gzip/", "*.*", options: SearchOption.TopDirectoryOnly);

        }

        private static void CreateLazyLoadScript(string configFile,string bundleName,string outFile)
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
            //jsBundle.WithDeferredLoad();
            //var cdnUrl = CdnLocation;

            //if (!string.IsNullOrWhiteSpace(cdnUrl))
            //{
            //    jsBundle.WithOutputBaseHref(cdnUrl);
            //    CopyFilesToCdn("~/gzip/", "*.js", options: SearchOption.TopDirectoryOnly);

            //    JsBundles.Add("langText." + culture, jsBundle.Render("~/gzip/j1#.js"));
            //    return;
            //}
            //JsBundles.Add("langText." + culture, jsBundle.Render("~/cdn/gzip/j#.js"));

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