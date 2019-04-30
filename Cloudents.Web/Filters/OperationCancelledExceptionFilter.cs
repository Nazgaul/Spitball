using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public class OperationCancelledExceptionFilter : ExceptionFilterAttribute
    {
        public OperationCancelledExceptionFilter()
        {
            Order = 1;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException)
            {
                context.ExceptionHandled = true;
                context.Result = new StatusCodeResult(400);
            }
        }
    }
}