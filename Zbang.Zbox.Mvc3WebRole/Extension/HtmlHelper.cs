using System.Text;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Mvc3WebRole.App_Start;

namespace Zbang.Zbox.Mvc3WebRole.Extension
{
    public static class HtmlHelperExtension
    {
        //public static MvcHtmlString Script(this HtmlHelper html, params string[] scriptsSources)
        //{
        //    var u = new UrlHelper(html.ViewContext.RequestContext);
        //    var sb = new StringBuilder();
        //    foreach (var scriptSrc in scriptsSources)
        //    {
        //        var script = new TagBuilder("script");
        //        script.Attributes["src"] = u.Content(scriptSrc);
        //        script.Attributes["type"] = "text/javascript";
        //        // script.Attributes["charset"] = "utf-8";
        //        sb.Append(script.ToString());
        //    }

        //    return MvcHtmlString.Create(sb.ToString());

        //}

        //public static MvcHtmlString Css(this HtmlHelper html, params string[] cssSources)
        //{
        //    var u = new UrlHelper(html.ViewContext.RequestContext);
        //    var sb = new StringBuilder();
        //    foreach (var cssSource in cssSources)
        //    {
        //        var css = new TagBuilder("link");
        //        css.Attributes["href"] = u.Content(cssSource);
        //        css.Attributes["rel"] = "stylesheet";
        //        css.Attributes["type"] = "text/css";
        //        sb.Append(css.ToString());
        //    }
        //    return MvcHtmlString.Create(sb.ToString());

        //}

        public static MvcHtmlString Script2(this HtmlHelper html, string key)
        {
            var jsLinks = BundleConfig.JsLink(key);
            return MvcHtmlString.Create(jsLinks);
        }
        public static MvcHtmlString Css2(this HtmlHelper html, string key)
        {
            var cssLinks = BundleConfig.CssLink(key);
            return MvcHtmlString.Create(cssLinks);
        }

        public static MvcHtmlString CssRtl(this HtmlHelper html, string key)
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
            {
                var cssLinks = BundleConfig.CssLink(key);
                return MvcHtmlString.Create(cssLinks);
            }
            return MvcHtmlString.Empty;
        }
    }
}