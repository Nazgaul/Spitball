using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Mvc3WebRole.Attributes
{
    public class ZboxHandleErrorAttribute: HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
           
            var info = string.Format("url {0} user {1} params {2} ", filterContext.HttpContext.Request.RawUrl,
                filterContext.HttpContext.User.Identity.Name
                , filterContext.HttpContext.Request.Params);
            TraceLog.WriteError(info, filterContext.Exception);
            base.OnException(filterContext);
        }
    }
    
}