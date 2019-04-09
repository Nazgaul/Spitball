using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Cloudents.Web.Framework
{
    public class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
    {
        public void Apply(ParameterModel parameter)
        {
            if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
            {
                parameter.Action.Selectors.Last().ActionConstraints.Add(
                    new RequiredFromQueryActionConstraint(parameter.BindingInfo?.BinderType ?? parameter.ParameterType));
            }
        }
    }

    public class RequiredPropertyForQueryAttribute : RequiredAttribute
    {

    }


    public class RequiredFromQueryActionConstraint : ActionMethodSelectorAttribute
    {
        private readonly Type _parameter;

        public RequiredFromQueryActionConstraint(Type parameter)
        {
            _parameter = parameter;
        }

        //public RequiredFromQueryActionConstraint(string[] parameters)
        //{
        //    _parameters = parameters;
        //}

        //public int Order => 999;

        //public bool Accept(ActionConstraintContext context)
        //{

        //}
        private static ConcurrentDictionary<(string path,string method), Dictionary<ApiDescription, List<string>>> _dic =
            new ConcurrentDictionary<(string, string), Dictionary<ApiDescription, List<string>>>();

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
           // var dic = new Dictionary<ApiDescription, List<string>>();
            var requiredParameters = _parameter.GetProperties().Where(p => p.GetCustomAttribute<RequiredPropertyForQueryAttribute>() != null);

            var query = routeContext.HttpContext.Request.Query;
            var t = requiredParameters.All(a => query.ContainsKey(a.Name));

            if (t)
            {
                
               var val = _dic.GetOrAdd((routeContext.HttpContext.Request.Path,routeContext.HttpContext.Request.Method), descriptor =>
               {
                   var dic = new Dictionary<ApiDescription, List<string>>();
                   var x = routeContext.HttpContext.RequestServices.GetService<IApiDescriptionGroupCollectionProvider>();
                   var apiDescription = x.ApiDescriptionGroups.Items.SelectMany(g => g.Items)
                       .First(w => w.ActionDescriptor == action);

                   var allApis = x.ApiDescriptionGroups.Items.SelectMany(g => g.Items)
                       .Where(w => w.RelativePath == apiDescription.RelativePath &&
                                   w.HttpMethod == apiDescription.HttpMethod);
                   foreach (var api in allApis)
                   {
                       var z3 = api.ParameterDescriptions.Where(w =>
                               w.CustomAttributes()
                                   .Any(attr => attr.GetType() == typeof(RequiredPropertyForQueryAttribute)))
                           .ToList();
                       dic.Add(api, z3.Select(s => s.Name).ToList());
                   }

                   return dic;
               });

                var z = val.Where(w => w.Value.TrueForAll(a => query.ContainsKey(a))).ToList();
                var z2 = z.OrderByDescending(o => o.Value.Count(c => query.ContainsKey(c)));

                var correctRoute = z2.First();
                return correctRoute.Key.ActionDescriptor == action;
            }

            return false;
        }
    }


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