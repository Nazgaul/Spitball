using System;

namespace Cloudents.Core.Attributes
{
    public class DtoToEntityConnectionAttribute : Attribute
    {
        public DtoToEntityConnectionAttribute(string entityName)
        {
            EntityName = entityName;
        }

        public string EntityName { get; private set; }
    }
}