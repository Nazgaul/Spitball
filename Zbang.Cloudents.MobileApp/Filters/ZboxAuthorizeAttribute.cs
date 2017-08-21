using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.MobileApp.Filters
{
    public class ZboxAuthorizeAttribute : AuthorizeAttribute, IOverrideFilter
    {
        public ZboxAuthorizeAttribute()
        {
            
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var userId = actionContext.RequestContext.Principal.GetUserId();
            return userId != -1;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            HttpContext.Current.Response.AddHeader("AuthenticationStatus", "NotAuthorized");
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            //return;
            //base.HandleUnauthorizedRequest(actionContext);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }

        public override bool AllowMultiple
        {
            get
            {
                return base.AllowMultiple;
            }
        }

        public override bool IsDefaultAttribute()
        {
            return base.IsDefaultAttribute();
        }

        public Type FiltersToOverride
        {
            get
            {
                return typeof(IAuthorizationFilter);
            }
        }
    }
}