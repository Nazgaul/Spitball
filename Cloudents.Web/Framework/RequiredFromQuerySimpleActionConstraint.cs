using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Cloudents.Web.Framework
{
    public class RequiredFromQuerySimpleActionConstraint : IActionConstraint
    {
        private readonly string _parameter;

        public RequiredFromQuerySimpleActionConstraint(string parameter)
        {
            _parameter = parameter;
        }

        public int Order => 999;

        public bool Accept(ActionConstraintContext context)
        {
            if (!context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
            {
                return false;
            }

            return true;
        }
    }
}