using System.Collections.Generic;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class VerticalEngineSearchDto : VerticalEngineDto
    {
        public VerticalEngineSearchDto(IEnumerable<string> term, string university, string docType) : base(term)
        {
            University = university;
            DocType = docType;
        }

        public override Vertical Vertical => Vertical.Note;

        public string University { get; }
        [DataMember]
        public string DocType { get; }
    }
}