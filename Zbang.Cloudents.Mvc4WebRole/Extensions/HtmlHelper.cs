using System.Threading;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class HtmlHelperExtension
    {

        public static MvcHtmlString Svg(this HtmlHelper html, string name, string hash, object htmlAttributes = null)
        {
            var helper = new UrlHelper(html.ViewContext.RequestContext);
            var svgTag = new TagBuilder("svg");
            var useTag = new TagBuilder("use");
            useTag.MergeAttribute("xlink:href", string.Format("{0}?{2}#{1}", helper.Content(name), 
                hash, 
                VersionHelper.CurrentVersion(false)));
            svgTag.InnerHtml = useTag.ToString(TagRenderMode.SelfClosing);
            if (htmlAttributes != null)
            {
                var dic = System.Web.WebPages.Html.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                svgTag.MergeAttributes(dic);
            }

            return MvcHtmlString.Create(svgTag.ToString());
        }

        public static MvcHtmlString Script2(this HtmlHelper html, string key)
        {
            var jsLinks = BundleConfig.JsLink(key);
            return MvcHtmlString.Create(jsLinks);
        }

        public static MvcHtmlString AngularLocale(this HtmlHelper html)
        {
            var helper = new UrlHelper(html.ViewContext.RequestContext);
            var pathName = string.Format("{0}_{1}.js", helper.Content("/Scripts/i18n/angular-locale"),
                Thread.CurrentThread.CurrentUICulture.Name);

            var jsTag = new TagBuilder("script");
            jsTag.MergeAttribute("src", pathName);
            return MvcHtmlString.Create(jsTag.ToString());
        }
        public static MvcHtmlString Css2(this HtmlHelper html, string key)
        {
            var cssLinks = BundleConfig.CssLink(key);
            return MvcHtmlString.Create(cssLinks);
        }

        public static MvcHtmlString CssCulture(this HtmlHelper html, string key)
        {
            var cssLinks = BundleConfig.CssLink(key + "." + System.Threading.Thread.CurrentThread.CurrentCulture);
            if (string.IsNullOrEmpty(cssLinks))
            {
                return MvcHtmlString.Empty;
            }
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