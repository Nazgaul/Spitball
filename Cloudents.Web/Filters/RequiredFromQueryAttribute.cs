using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Cloudents.Web.Filters
{
    [System.AttributeUsageAttribute(System.AttributeTargets.All, AllowMultiple = false)]
    public sealed class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
    {
        public RequiredFromQueryAttribute() : base()
        {
        }

        public void Apply(ParameterModel parameter)
        {
            if (parameter.Action.Selectors?.Any() == true)
            {
                parameter.Action.Selectors.Last().ActionConstraints.Add(new RequiredFromQueryActionConstraint(parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName));
            }
        }
    }
}