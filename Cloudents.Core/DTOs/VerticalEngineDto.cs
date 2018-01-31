using System.Collections.Generic;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [DataContract]

    public abstract class VerticalEngineDto
    {
        protected VerticalEngineDto(IEnumerable<string> term)
        {
            Term = term;
        }

        [DataMember]
        public abstract Vertical Vertical { get; }
        [DataMember]
        public IEnumerable<string> Term { get; }
    }
}
