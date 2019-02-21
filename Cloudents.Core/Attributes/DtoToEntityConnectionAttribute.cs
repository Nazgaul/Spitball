using System;
using System.Collections.Generic;

namespace Cloudents.Core.Attributes
{
    public class DtoToEntityConnectionAttribute : Attribute
    {
        public DtoToEntityConnectionAttribute(string entityName): this()
        {
            EntityNames.Add(entityName);
        }
        public DtoToEntityConnectionAttribute(params string[] entityNames) : this()
        {
            EntityNames = entityNames;
        }

        protected DtoToEntityConnectionAttribute()
        {
            EntityNames = new List<string>();
        }
        
        public IList<string> EntityNames { get; set; }
    }
}