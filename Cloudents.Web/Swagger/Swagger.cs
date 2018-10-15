using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Cloudents.Web.Swagger
{
    public static class Startup
    {
        public static void SwaggerInitial(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "Spitball Api", Version = "v1"});
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Cloudents.Web.xml");
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();
                c.DescribeAllParametersInCamelCase();
                c.OperationFilter<FormFileOperationFilter>();
                //c.ResolveConflictingActions(f =>
                //{
                //    var descriptions = f.ToList();
                //    var parameters = descriptions
                //        .SelectMany(desc => desc.ParameterDescriptions)
                //        .GroupBy(x => x, (x, xs) => new {IsOptional = xs.Count() == 1, Parameter = x},
                //            ApiParameterDescriptionEqualityComparer.Instance)
                //        .ToList();
                //    var description = descriptions[0];
                //    description.ParameterDescriptions.Clear();
                //    parameters.ForEach(x =>
                //    {
                //        if (x.Parameter.RouteInfo != null)
                //            x.Parameter.RouteInfo.IsOptional = x.IsOptional;
                //        description.ParameterDescriptions.Add(x.Parameter);
                //    });
                //    return description;
                //});
            });
        }
    }
}