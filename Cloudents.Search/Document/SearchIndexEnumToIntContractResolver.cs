using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Cloudents.Search.Document
{
    public class SearchIndexEnumToIntContractResolver : DefaultContractResolver
    {

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var p = base.CreateProperty(member, memberSerialization);
            if (p.PropertyType.IsEnum)
            {
                p.PropertyType = typeof(int);
            }

            if (p.PropertyType == typeof(Guid))
            {
                p.PropertyType = typeof(string);
            }
            var nullableType = Nullable.GetUnderlyingType(p.PropertyType);
            if (nullableType != null)
            {
                if (nullableType == typeof(Guid))
                {
                    p.PropertyType = typeof(string);
                }

                if (nullableType.IsEnum)
                {
                    p.PropertyType = typeof(int);
                }
            }
            return p;
        }
    }
}