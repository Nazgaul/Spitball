using System.Web.Mvc;
using System.Web.Routing;

namespace Zbang.Zbox.Mvc3WebRole.Extension
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
    }
}