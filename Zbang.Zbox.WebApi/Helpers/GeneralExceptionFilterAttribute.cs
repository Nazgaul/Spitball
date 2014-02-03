using System.Net;
using System.Web.Http.Filters;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WebApi.Helpers
{
    public class GeneralExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            TraceLog.WriteError(actionExecutedContext.Request.ToString(), actionExecutedContext.Exception);
            actionExecutedContext.Response = actionExecutedContext.Request.CreateZboxErrorResponse(HttpStatusCode.InternalServerError, string.Empty);
        }
    }

    public class BoxAccessDeniedExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is BoxAccessDeniedException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateZboxErrorResponse(HttpStatusCode.Unauthorized, "User is not authorized to see this data");
            }
        }

    }


}