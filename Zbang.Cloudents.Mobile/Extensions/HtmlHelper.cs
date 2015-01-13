using System.Threading;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mobile.Extensions
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
            var jsLinks = BundleConfig.JsLink("langText." + Thread.CurrentThread.CurrentUICulture.Name);
            return MvcHtmlString.Create(jsLinks);
            //return MvcHtmlString.Create(jsLinks);
            //var helper = new UrlHelper(html.ViewContext.RequestContext);
            //var pathName = string.Format("{0}_{1}.js", helper.Content("/Scripts/i18n/angular-locale"),
            //    Thread.CurrentThread.CurrentUICulture.Name);

            //var jsTag = new TagBuilder("script");
            //jsTag.MergeAttribute("src", pathName);
            //return MvcHtmlString.Create(jsTag.ToString());
        }
        public static MvcHtmlString Css2(this HtmlHelper html, string key)
        {
            //if (Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
            //{
            //    key = key + BundleConfig.Rtl;
            //}
            var cssLinks = BundleConfig.CssLink(key);
            return MvcHtmlString.Create(cssLinks);
        }

        public static MvcHtmlString CssCulture(this HtmlHelper html, string key)
        {
            var cssLinks = BundleConfig.CssLink(key + "." + Thread.CurrentThread.CurrentCulture);
            if (string.IsNullOrEmpty(cssLinks))
            {
                return MvcHtmlString.Empty;
            }
            return MvcHtmlString.Create(cssLinks);
        }

        public static MvcHtmlString CssRtl(this HtmlHelper html, string key)
        {
            if (Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
            {
                var cssLinks = BundleConfig.CssLink(key);
                return MvcHtmlString.Create(cssLinks);
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString AngularAntiForgeryToken(this HtmlHelper html)
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            var token = formToken;

            var inputBuilder = new TagBuilder("input");
            inputBuilder.MergeAttribute("data-ng-model", "antiForgeryToken");
            inputBuilder.MergeAttribute("data-ng-init", string.Format("antiForgeryToken='{0}'", token));
            inputBuilder.MergeAttribute("type", "hidden");
            return MvcHtmlString.Create(inputBuilder.ToString(TagRenderMode.SelfClosing));

        }
    }
}