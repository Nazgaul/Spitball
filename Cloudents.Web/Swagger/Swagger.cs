using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cloudents.Web.Swagger
{
    public static class Startup
    {
        public static void SwaggerInitial(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Spitball Api", Version = "v1" });
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Cloudents.Web.xml");
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();
                c.DescribeAllParametersInCamelCase();
                c.OperationFilter<FormFileOperationFilter>();
                c.ResolveConflictingActions(f =>
                {
                    var descriptions = f.ToList();
                    var parameters = descriptions
                        .SelectMany(desc => desc.ParameterDescriptions)
                        .GroupBy(x => x, (x, xs) => new { IsOptional = xs.Count() == 1, Parameter = x },
                            ApiParameterDescriptionEqualityComparer.Instance)
                        .ToList();
                    var description = descriptions[0];
                    description.ParameterDescriptions.Clear();
                    parameters.ForEach(x =>
                    {
                        if (x.Parameter.RouteInfo != null)
                            x.Parameter.RouteInfo.IsOptional = x.IsOptional;
                        description.ParameterDescriptions.Add(x.Parameter);
                    });
                    return description;
                });
            });
        }


    }

    internal sealed class ApiParameterDescriptionEqualityComparer : IEqualityComparer<ApiParameterDescription>
    {
        private static readonly Lazy<ApiParameterDescriptionEqualityComparer> _instance
            = new Lazy<ApiParameterDescriptionEqualityComparer>(() => new ApiParameterDescriptionEqualityComparer());
        public static ApiParameterDescriptionEqualityComparer Instance
            => _instance.Value;

        private ApiParameterDescriptionEqualityComparer() { }

        public int GetHashCode(ApiParameterDescription obj)
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + obj.ModelMetadata.GetHashCode();
                hash = hash * 23 + obj.Name.GetHashCode();
                hash = hash * 23 + obj.Source.GetHashCode();
                hash = hash * 23 + obj.Type.GetHashCode();
                return hash;
            }
        }

        public bool Equals(ApiParameterDescription x, ApiParameterDescription y)
        {
            if (!x.ModelMetadata.Equals(y.ModelMetadata)) return false;
            if (!x.Name.Equals(y.Name)) return false;
            if (!x.Source.Equals(y.Source)) return false;
            if (!x.Type.Equals(y.Type)) return false;
            return true;
        }
    }
}