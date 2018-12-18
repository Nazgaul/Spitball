using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            return p;
        }
    }
}