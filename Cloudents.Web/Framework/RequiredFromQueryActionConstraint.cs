using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudents.Web.Framework
{
    public class RequiredFromQueryActionConstraint : ActionMethodSelectorAttribute
    {
        private readonly Type _parameter;

        public RequiredFromQueryActionConstraint(Type parameter)
        {
            _parameter = parameter;
        }

        private static ConcurrentDictionary<(string path, string method), Dictionary<ApiDescription, List<string>>> _dic =
            new ConcurrentDictionary<(string, string), Dictionary<ApiDescription, List<string>>>();

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            var requiredParameters = _parameter.GetProperties().Where(p => CustomAttributeExtensions.GetCustomAttribute<RequiredPropertyForQueryAttribute>((MemberInfo) p) != null);

            var query = routeContext.HttpContext.Request.Query;
            var t = requiredParameters.All(a => query.ContainsKey(a.Name));

            if (!t) return false;
            var val = _dic.GetOrAdd((routeContext.HttpContext.Request.Path, routeContext.HttpContext.Request.Method),
                descriptor =>
                {
                    var dic = new Dictionary<ApiDescription, List<string>>();
                    var x =
                        routeContext.HttpContext.RequestServices.GetService<IApiDescriptionGroupCollectionProvider>();
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
    }
}