using System;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Microsoft.Spatial;

namespace Cloudents.Core.Entities.Search
{
    [DataContract]
    public class Tutor : ISearchObject
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Image { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public GeographyPoint Location { get; set; }

        [DataMember]
        public double Fee { get; set; }

        public TutorFilter TutorFilter { get; set; }

        [DataMember(Name = nameof(TutorFilter))]
        public int TutorFilterInt
        {
            get => (int)TutorFilter;
            set => TutorFilter = (TutorFilter)value;
        }

        [DataMember]
        public DateTime? InsertDate { get; set; }

        [DataMember]
        public string[] Extra { get; set; }

        [DataMember]
        public string Source { get; set; }
    }
}