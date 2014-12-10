using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public class RedirectToRouteResultFragment : RedirectToRouteResult
    {
       public RedirectToRouteResultFragment(RouteValueDictionary values)
            : base(values)
        {
        }

        public RedirectToRouteResultFragment(string routeName, RouteValueDictionary values)
            : base(routeName, values)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var destination = new StringBuilder();

            var helper = new UrlHelper(context.RequestContext);
            destination.Append(helper.RouteUrl(RouteName, RouteValues));

            //Add href fragment if set
            if (!string.IsNullOrEmpty(Fragment))
            {
                destination.AppendFormat("#{0}", Fragment);
            }

            context.HttpContext.Response.Redirect(destination.ToString(), false);
        }

        public string Fragment { get; set; }
    }

/*
    public static class RedirectToRouteResultExtensions
    {
        public static RedirectToRouteResult AddFragment(this RedirectToRouteResult result, string fragment)
        {
            return new RedirectToRouteResultFragment(result.RouteName, result.RouteValues)
            {
                Fragment = fragment
            };
        }
    }
*/
}