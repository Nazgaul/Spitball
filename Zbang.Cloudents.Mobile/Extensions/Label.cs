using System;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class Label
    {
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelHelper(html,
                               ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                               ExpressionHelper.GetExpressionText(expression),
                               htmlAttributes);
        }


        internal static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, object htmlAttributes, string labelText = null)
        {
            string resolvedLabelText = labelText ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(resolvedLabelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            var htmlObjects = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            tag.MergeAttributes(htmlObjects);
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
            tag.SetInnerText(resolvedLabelText);

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }
}