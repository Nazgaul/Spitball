using System;
using System.Reflection;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class NonAjaxAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            #region Validation
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            #endregion

            return !controllerContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}