using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Cloudents.Web.Filters
{
    public class FormContentTypeAttribute : Attribute, IActionConstraint
    {
        public bool Accept(ActionConstraintContext context)
        {
          return  context.RouteContext.HttpContext.Request.HasFormContentType;
        }

        public int Order => 0;
    }
}
