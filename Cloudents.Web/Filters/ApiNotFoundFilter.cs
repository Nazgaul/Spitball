using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public sealed class ApiNotFoundFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments(new PathString("/api"),
                StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new NotFoundResult();
                return;
            }
            base.OnActionExecuting(context);
            // do something before the action executes
        }


    }
}