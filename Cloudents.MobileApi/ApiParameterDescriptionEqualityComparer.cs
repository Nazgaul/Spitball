using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cloudents.Api
{
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

    internal class ApplySchemaVendorExtensions : ISchemaFilter
    {


        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (model?.Properties == null)
            {
                return;
            }

            var p = context.SystemType.GetCustomAttribute<JsonIgnoreAttribute>();
            if (p != null)
            {
               // model.Properties.Remove(p);
            }

            //var excludedProperties = context.SystemType.GetProperties().Where(t => t.GetCustomAttribute<JsonIgnoreAttribute>() != null);
            //foreach (var excludedProperty in excludedProperties)
            //{
            //    if (model.Properties.ContainsKey(excludedProperty.Name))
            //    {
            //        model.Properties.Remove(excludedProperty.Name);
            //    }
            //}
        }
    }
}