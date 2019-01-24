using System;

namespace Cloudents.Core.Attributes
{
    public class DtoToEntityConnectionAttribute : Attribute
    {
        public DtoToEntityConnectionAttribute(string entityName)
        {
            EntityName = entityName;
        }

        private string EntityName { get; set; }
    }
}