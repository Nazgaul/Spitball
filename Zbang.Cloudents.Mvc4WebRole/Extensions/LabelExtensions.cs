using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class LabelExtensions
    {
        public static MvcHtmlString LabelForWithColon<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {

            var str = html.LabelFor(expression).ToString();
            var position = str.IndexOf("</label>");
            str = str.Insert(position, ":");
            return MvcHtmlString.Create(str);


        }
    }
}