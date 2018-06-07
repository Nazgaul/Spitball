using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Cloudents.Web.Swagger
{
    internal sealed class ApiParameterDescriptionEqualityComparer : IEqualityComparer<ApiParameterDescription>
    {
        private static readonly Lazy<ApiParameterDescriptionEqualityComparer> PrivateInstance
            = new Lazy<ApiParameterDescriptionEqualityComparer>(() => new ApiParameterDescriptionEqualityComparer());
        public static ApiParameterDescriptionEqualityComparer Instance
            => PrivateInstance.Value;

        private ApiParameterDescriptionEqualityComparer() { }

        public int GetHashCode(ApiParameterDescription obj)
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + obj.ModelMetadata.GetHashCode();
                hash = (hash * 23) + obj.Name.GetHashCode();
                hash = (hash * 23) + obj.Source.GetHashCode();
                return (hash * 23) + obj.Type.GetHashCode();
            }
        }

        public bool Equals(ApiParameterDescription x, ApiParameterDescription y)
        {
            if (!x.ModelMetadata.Equals(y.ModelMetadata)) return false;
            if (!x.Name.Equals(y.Name)) return false;
            if (!x.Source.Equals(y.Source)) return false;
            if (x.Type != y.Type) return false;
            return true;
        }
    }
}