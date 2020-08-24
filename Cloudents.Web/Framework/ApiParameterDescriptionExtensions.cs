using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Cloudents.Web.Framework
{
    public static class ApiParameterDescriptionExtensions
    {
        public static ParameterInfo ParameterInfo(this ApiParameterDescription apiParameter)
        {
            var controllerParameterDescriptor = apiParameter.ParameterDescriptor as ControllerParameterDescriptor;
            return controllerParameterDescriptor?.ParameterInfo;
        }

        public static PropertyInfo PropertyInfo(this ApiParameterDescription apiParameter)
        {
            var modelMetadata = apiParameter.ModelMetadata;

            return (modelMetadata?.ContainerType != null)
                ? modelMetadata.ContainerType.GetProperty(modelMetadata.PropertyName)
                : null;
        }

        public static IEnumerable<object> CustomAttributes(this ApiParameterDescription apiParameter)
        {
            var parameterInfo = apiParameter.ParameterInfo();
            var parameterAttributes = (parameterInfo != null) ? parameterInfo.GetCustomAttributes(true) : Enumerable.Empty<object>();

            var propertyInfo = apiParameter.PropertyInfo();
            var propertyAttributes = (propertyInfo != null) ? propertyInfo.GetCustomAttributes(true) : Enumerable.Empty<object>();

            return parameterAttributes.Union(propertyAttributes);
        }
    }
}