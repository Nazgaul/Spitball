using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Swagger;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

namespace Cloudents.Mobile
{
    /// <summary>
    /// Partial startup
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Add swagger config
        /// </summary>
        /// <param name="config"></param>
        public static void ConfigureSwagger(HttpConfiguration config)
        {
            // Use the custom ApiExplorer that applies constraints. This prevents
            // duplicate routes on /api and /tables from showing in the Swagger doc.
            config.Services.Replace(typeof(IApiExplorer), new MobileAppApiExplorer(config));
            config
                .EnableSwagger("{apiVersion}/swagger",c =>
                {
                    //c.MultipleApiVersions(
                    //    (apiDescription, version) => apiDescription.GetGroupName() == version,
                    //    info =>
                    //    {
                    //        foreach (var group in apiExplorer.ApiDescriptions)
                    //        {
                    //            var description = "A sample application with Swagger, Swashbuckle, and API versioning.";

                    //            if (group.IsDeprecated)
                    //            {
                    //                description += " This API version has been deprecated.";
                    //            }

                    //            info.Version(group.Name, $"Sample API {group.ApiVersion}")
                    //                .Contact(d => d.Name("Bill Mei").Email("bill.mei@somewhere.com"))
                    //                .Description(description)
                    //                .License(l => l.Name("MIT").Url("https://opensource.org/licenses/MIT"))
                    //                .TermsOfService("Shareware");
                    //        }
                    //    });
                    c.SingleApiVersion("v1", "Spitball Mobile Service");
                    c.ResolveConflictingActions(f=>f.LastOrDefault());
                    // Tells the Swagger doc that any MobileAppController needs a
                    // ZUMO-API-VERSION header with default 2.0.0
                    c.OperationFilter<MobileAppHeaderFilter>();
                    // Looks at attributes on properties to decide whether they are readOnly.
                    // Right now, this only applies to the DatabaseGeneratedAttribute.
                    c.SchemaFilter<MobileAppSchemaFilter>();
                    c.OperationFilter<SwaggerDefaultValues>();
                    c.DescribeAllEnumsAsStrings(true);
                    c.IncludeXmlComments(
                        $@"{AppDomain.CurrentDomain.BaseDirectory}\bin\Cloudents.Mobile.xml");
                })
                .EnableSwaggerUi(swagger => swagger.EnableDiscoveryUrlSelector());
        }
    }

    internal class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(
            Operation operation,
            SchemaRegistry schemaRegistry,
            ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.parameters)
            {
                var description = apiDescription.ParameterDescriptions
                    .FirstOrDefault(p => p.Name == parameter.name);
                if (description == null)
                {
                    continue;
                }

                if (parameter.description == null)
                {
                    parameter.description = description.Documentation;
                }

                if (parameter.@default == null)
                {
                    parameter.@default = description.ParameterDescriptor?.DefaultValue;
                }
            }
        }
    }
}