using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

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
            if (!(context.Exception is OperationCanceledException)) return;
            context.ExceptionHandled = true;
            context.Result = new StatusCodeResult(400);
        }
    }
}