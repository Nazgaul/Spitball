using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class AjaxImageLink
    {
        /// <summary>
        /// A Simple ActionLink Image
        /// </summary>
        /// <param name="actionName">name of the action in controller</param>
        /// <param name="ajaxHelper"> </param>
        /// <param name="imgUrl">url of the image</param>
        /// <param name="alt">alt text of the image</param>
        /// <param name="ajaxOptions"> </param>
        /// <returns></returns>
        public static MvcHtmlString ImageLink(this AjaxHelper ajaxHelper, string imgUrl, string alt, string actionName, AjaxOptions ajaxOptions)
        {
            return ImageLink(ajaxHelper, imgUrl, alt, actionName, null, null, null, null, ajaxOptions);
        }

        /// <summary>
        /// A Simple ActionLink Image
        /// </summary>
        /// <param name="actionName">name of the action in controller</param>
        /// <param name="ajaxHelper"> </param>
        /// <param name="imgUrl">url of the iamge</param>
        /// <param name="alt">alt text of the image</param>
        /// <param name="routeValues"> </param>
        /// <param name="ajaxOptions"> </param>
        /// <returns></returns>
        public static MvcHtmlString ImageLink(this AjaxHelper ajaxHelper, string imgUrl, string alt, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ImageLink(ajaxHelper, imgUrl, alt, actionName, null, routeValues, null, null, ajaxOptions);
        }
        public static MvcHtmlString ImageLink(this AjaxHelper ajaxHelper, string imgUrl, string alt, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            return ImageLink(ajaxHelper, imgUrl, alt, actionName, controllerName, null, null, null, ajaxOptions);
        }


        /// <summary>
        /// A Simple ActionLink Image
        /// </summary>
        /// <param name="ajaxHelper"> </param>
        /// <param name="imgUrl">url of the image</param>
        /// <param name="alt">alt text of the image</param>
        /// <param name="routeValues"> </param>
        /// <param name="linkHtmlAttributes">attributes for the link</param>
        /// <param name="imageHtmlAttributes">attributes for the image</param>
        /// <param name="actionName"> </param>
        /// <param name="controllerName"> </param>
        /// <param name="ajaxOptions"> </param>
        /// <returns></returns>
        public static MvcHtmlString ImageLink(this AjaxHelper ajaxHelper, string imgUrl, string alt, string actionName,
            string controllerName, object routeValues, object linkHtmlAttributes, object imageHtmlAttributes, AjaxOptions ajaxOptions)
        {
            var urlHelper = new UrlHelper(ajaxHelper.ViewContext.RequestContext);

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
            linkTagBuilder.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            return MvcHtmlString.Create(linkTagBuilder.ToString());
        }

        public static MvcHtmlString InnerSpanImageLink(this AjaxHelper ajaxHelper, string ancorText, string actionName, string controllerName, object routeValues,
            object linkHtmlAttributes, object spanHtmlAttributes, AjaxOptions ajaxOptions)
        {
            var urlHelper = new UrlHelper(ajaxHelper.ViewContext.RequestContext);

            var url = urlHelper.Action(actionName, controllerName, routeValues);

            //Create the link
            var linkTagBuilder = new TagBuilder("a");
            linkTagBuilder.MergeAttribute("href", url);
            linkTagBuilder.MergeAttributes(new RouteValueDictionary(linkHtmlAttributes));
            
            //Create span
            var spanTagBuilder = new TagBuilder("span");
            spanTagBuilder.MergeAttributes(new RouteValueDictionary(spanHtmlAttributes));

            //Add span to link
            linkTagBuilder.InnerHtml = spanTagBuilder.ToString(TagRenderMode.Normal);
            linkTagBuilder.InnerHtml += ancorText;
            linkTagBuilder.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            return MvcHtmlString.Create(linkTagBuilder.ToString());
        }
    }
}