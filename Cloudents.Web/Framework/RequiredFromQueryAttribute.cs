using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace Cloudents.Web.Framework
{
    public class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
    {
        public void Apply(ParameterModel parameter)
        {
            if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
            {
                parameter.Action.Selectors.Last().ActionConstraints.Add(new RequiredFromQueryActionConstraint(parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName));
            }
        }
    }

    public class IgnoreFromQueryActionConstraint : ActionMethodSelectorAttribute
    {
        private readonly string _parameter;

        public IgnoreFromQueryActionConstraint(string parameter)
        {
            _parameter = parameter;
        }


        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            if (routeContext.HttpContext.Request.Query.ContainsKey(_parameter))
            {
                return false;
            }

            return true;
        }

        public int Order => 999;
    }

    public class RequiredFromQueryActionConstraint : IActionConstraint
    {
        private readonly string _parameter;

        public RequiredFromQueryActionConstraint(string parameter)
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