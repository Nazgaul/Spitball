using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.ApplicationServices;
using Zbang.Zbox.Infrastructure.Web.Ioc;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WCFServiceWebRole
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            AuthenticationService.Authenticating += new EventHandler<AuthenticatingEventArgs>(AuthenticationService_Authenticating);
            AuthenticationService.CreatingCookie += new EventHandler<CreatingCookieEventArgs>(AuthenticationService_CreatingCookie);            
        }

       

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        void AuthenticationService_Authenticating(object sender, AuthenticatingEventArgs e)
        {
            var unityFactory = new UnityControllerFactory();
            var membership = unityFactory.Resolve<IMembershipService>();
            Guid membershipUserId = Guid.Empty;
            e.Authenticated = membership.ValidateUser(e.UserName, e.Password.Trim(), out membershipUserId);
            e.AuthenticationIsComplete = true;
        }
        void AuthenticationService_CreatingCookie(object sender, CreatingCookieEventArgs e)
        {
            var unityFactory = new UnityControllerFactory();
            var membership = unityFactory.Resolve<IMembershipService>();
            var m_ZboxService = unityFactory.Resolve<IZboxService>();
            var m_FormsAuthenticationService = unityFactory.Resolve<IFormsAuthenticationService>();

            try
            {
                Guid membershipUserId = Guid.Empty;
                membership.ValidateUser(e.UserName, e.Password.Trim(), out membershipUserId);
                if (membershipUserId == Guid.Empty)
                {
                    throw new ArgumentException("Cannot find user");
                }
                var query = new GetUserByMembershipQuery(membershipUserId);
                var result = m_ZboxService.GetUserDetailsByMembershipId(query);

                m_FormsAuthenticationService.SignIn(result.GetUserId().ToString(), false);
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo("Error at AuthenticationService_CreatingCookie");
                TraceLog.WriteError(ex);
                throw ex;
            }

            e.CookieIsSet = true;
            //throw new NotImplementedException();
        }
    }
}