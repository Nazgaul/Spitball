using System.Collections.Generic;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class VerticalEngineFoodDto : VerticalEngineDto, IVerticalLocation
    {
        public VerticalEngineFoodDto(IEnumerable<string> term, string location) : base(term)
        {
            Location = location;
        }

        public override Vertical Vertical => Vertical.Food;

        public string Location { get; }
        [DataMember]
        public GeoPoint Cords { get; set; }
    }
}