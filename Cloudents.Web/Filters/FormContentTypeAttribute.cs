using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace Cloudents.Web.Filters
{
    public class FormContentTypeAttribute : Attribute, IActionConstraint
    {
        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.HasFormContentType;
        }

        public int Order => 0;
    }
}
