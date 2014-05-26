using System.Web.Mvc;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class ImageLinkExtension
    {
        public static MvcHtmlString ImageLink(this  HtmlHelper helper, string imgUrl, string alt, string actionName)
        {
            return ImageLink(helper, imgUrl, alt, actionName, null, null, null, null);
        }

        public static MvcHtmlString ImageLink(this HtmlHelper helper, string imgUrl, string alt, string actionName, object routeValues)
        {
            return ImageLink(helper, imgUrl, alt, actionName, null, routeValues, null, null);
        }
        public static MvcHtmlString ImageLink(this HtmlHelper helper, string imgUrl, string alt, string actionName, string controllerName)
        {
            return ImageLink(helper, imgUrl, alt, actionName, controllerName, null, null, null);
        }


        public static MvcHtmlString ImageLink(this HtmlHelper helper, string imgUrl, string alt, string actionName, string controllerName, object routeValues, object linkHtmlAttributes, object imageHtmlAttributes)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var url = urlHelper.Action(actionName, controllerName, routeValues);

            //Create the link
            var linkTagBuilder = new TagBuilder("a");
            linkTagBuilder.MergeAttribute("href", url);
            linkTagBuilder.MergeAttributes(new RouteValueDictionary(linkHtmlAttributes));

            //Create image
            var imageTagBuilder = new TagBuilder("img");
            imageTagBuilder.MergeAttribute("src", urlHelper.Content(imgUrl));
            imageTagBuilder.MergeAttribute("alt", alt);
            imageTagBuilder.MergeAttributes(new RouteValueDictionary(imageHtmlAttributes));

            //Add image to link
            linkTagBuilder.InnerHtml = imageTagBuilder.ToString(TagRenderMode.SelfClosing);
            return MvcHtmlString.Create(linkTagBuilder.ToString());
        }

        //public static MvcHtmlString Test(this HtmlHelper helper, Type enumparam, string ulId)
        //{
        //    if (!enumparam.IsEnum)
        //    {
        //        throw new ArgumentException("should be enum", "enumparam");
        //    }
        //    var values = Enum.GetValues(enumparam).Cast<Enum>();

        //    var ulTagBuilder = new TagBuilder("ul");
        //    ulTagBuilder.MergeAttribute("id", ulId);

        //    foreach (var value in values)
        //    {
        //        var liTagBuilder = new TagBuilder("li");
        //        liTagBuilder.SetInnerText(value.GetEnumDescription());
        //        liTagBuilder.MergeAttribute("data-value", value.GetStringValue());
        //        ulTagBuilder.InnerHtml += liTagBuilder.ToString(TagRenderMode.Normal);
        //    }

        //    return MvcHtmlString.Create(ulTagBuilder.ToString(TagRenderMode.Normal));
        //}
    }
}