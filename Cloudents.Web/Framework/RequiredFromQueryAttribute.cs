using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;
using System.Reflection;

namespace Cloudents.Web.Framework
{
    public class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
    {
        public void Apply(ParameterModel parameter)
        {
            if (parameter.Action.Selectors == null || !parameter.Action.Selectors.Any()) return;
            var type = parameter.BindingInfo?.BinderType ?? parameter.ParameterType;
            if (type.GetProperties().Any(p => p.GetCustomAttribute<RequiredPropertyForQueryAttribute>() != null))
            {
                parameter.Action.Selectors.Last().ActionConstraints.Add(
                    new RequiredFromQueryActionConstraint(type));
            }
            else
            {

                parameter.Action.Selectors.Last().ActionConstraints.Add(
                    new RequiredFromQuerySimpleActionConstraint(parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName));
            }

        }
    }
}