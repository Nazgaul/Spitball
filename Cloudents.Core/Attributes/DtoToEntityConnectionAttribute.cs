using System;
using System.Collections.Generic;

namespace Cloudents.Core.Attributes
{
    public class EntityBindAttribute : Attribute
    {
        public EntityBindAttribute(string entityName) : this()
        {
            EntityNames.Add(entityName);
        }
        public EntityBindAttribute(params string[] entityNames) : this()
        {
            EntityNames = entityNames;
        }

        private EntityBindAttribute()
        {
            EntityNames = new List<string>();
        }

        public IList<string> EntityNames { get; set; }
    }
}