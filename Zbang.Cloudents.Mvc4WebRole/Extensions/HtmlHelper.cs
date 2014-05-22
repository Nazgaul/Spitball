using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.App_Start;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
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

        public static System.Web.HtmlString GetAntiForgeryKey2()
        {
            var x = System.Web.Helpers.AntiForgery.GetHtml();

            return x;

        }



        //public static MvcHtmlString RadioButtonForEnum<TModel, TProperty>(
        //    this HtmlHelper<TModel> htmlHelper,
        //    Expression<Func<TModel, TProperty>> expression
        //)
        //{
        //    var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
        //    if (!metaData.ModelType.IsEnum)
        //    {
        //        throw new ArgumentException("This helper is intended to be used with enum types");
        //    }

        //    var names = Enum.GetNames(metaData.ModelType);
        //    var sb = new StringBuilder();

        //    var fields = metaData.ModelType.GetFields(
        //        BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public
        //    );

        //    foreach (var name in names)
        //    {
        //        var id = string.Format(
        //            "{0}_{1}_{2}",
        //            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
        //            metaData.PropertyName,
        //            name
        //        );

        //        var radio = htmlHelper.RadioButtonFor(expression, name, new { id = id }).ToHtmlString();
        //        var field = fields.Single(f => f.Name == name);
        //        var label = name;
        //        var display = field
        //            .GetCustomAttributes(typeof(DisplayAttribute), false)
        //            .OfType<DisplayAttribute>()
        //            .FirstOrDefault();
        //        if (display != null)
        //        {
        //            label = display.GetName();
        //        }

        //        sb.AppendFormat(
        //            "<label for=\"{0}\">{1}</label> {2}",
        //            id,
        //            HttpUtility.HtmlEncode(label),
        //            radio
        //        );
        //    }
        //    return MvcHtmlString.Create(sb.ToString());
        //}

    }
}