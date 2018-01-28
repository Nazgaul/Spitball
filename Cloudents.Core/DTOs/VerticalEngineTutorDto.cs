using System.Collections.Generic;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class VerticalEngineTutorDto : VerticalEngineDto, IVerticalLocation
    {
        public VerticalEngineTutorDto(IEnumerable<string> term, string location) : base(term)
        {
            Location = location;
        }

        public override Vertical Vertical => Vertical.Tutor;

        public string Location { get; }

        [DataMember]
        public Location Cords { get; set; }
    }
}