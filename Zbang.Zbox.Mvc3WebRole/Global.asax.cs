using System;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.App_Start;
using Zbang.Zbox.Mvc3WebRole.Helpers;

namespace Zbang.Zbox.Mvc3WebRole
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            TraceLog.WriteInfo("Application Start");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ViewConfig.RegisterEngineAndViews();
            IocConfig.RegisterIoc();
            BundleConfig.RegisterBundle();
            
            MvcHandler.DisableMvcResponseHeader = true;
            
        }

       

        protected void Application_End()
        {
            TraceLog.WriteInfo("Application ending");
        }

        
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            var keys = custom.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort<string>(keys);
            var value = string.Empty;
            foreach (var key in keys)
            {
                if (key == CustomCacheKeys.Auth)
                {
                    value += User.Identity.IsAuthenticated.ToString(CultureInfo.InvariantCulture);
                }
                if (key == CustomCacheKeys.Lang)
                {
                    value += System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                }
                if (key == CustomCacheKeys.User)
                {
                    value += User.Identity.Name;
                }
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                return base.GetVaryByCustomString(context, custom);
            }
            return value;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //    //if (RoleEnvironment.IsAvailable)
            //    //{
            //    Exception ex = Server.GetLastError();
            //    TraceLog.WriteError(ex);
            //    Server.ClearError();
            //    Context.Response.Clear();
            //    Response.Redirect("/Error");
            //    //Context.Response.StatusCode = 404;
            //    //}
            //    //Response.Redirect("/Error");
            //    //Server.ClearError();

            //    // Write exception to log
            //    // redirect to error page
        }
        protected void Application_AcquireRequestState(object sender, System.EventArgs e)
        {

        }

        

    }




}