using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Core.Enum
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]

    public sealed class ParseAttribute : Attribute
    {
        public ParseAttribute(string description)
        {
            Description = description;
        }
        public string Description { get; private set; }

    }
}
