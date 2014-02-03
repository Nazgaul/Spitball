using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Security;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData;

namespace Zbang.Zbox.WebApi.Helpers
{
    public class ZboxAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        public ZboxAuthorizeAttribute()
        {
            IsAuthenticationRequired = true;
        }
        public bool IsAuthenticationRequired { get; set; }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //if (!IsAuthenticationRequired)
            //{
            //    return true;
            //}

            var token = actionContext.Request.Headers.Authorization;
            if (token == null && IsAuthenticationRequired)
            {
                return false;
            }
            var bytes = MachineKey.Decode(token.Scheme, MachineKeyProtection.All);

            var serialize = new SerializeData<UserToken>();
            var userDetails = serialize.DeserializeBinary(bytes);
            if (userDetails.ExpireTokenTime < DateTime.UtcNow && IsAuthenticationRequired)
            {
                return false; 
            }
            var identity = new GenericIdentity(userDetails.UserId.ToString());
            var principal = new GenericPrincipal(identity, new string[0]);
            Thread.CurrentPrincipal = principal;
            return true;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            actionContext.Response = actionContext.ControllerContext.Request.CreateZboxErrorResponse(HttpStatusCode.Unauthorized, "Authorization has been denied for this request.");
        }

    }
}