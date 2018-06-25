using System;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Microsoft.Spatial;

namespace Cloudents.Core.Entities.Search
{
    [DataContract]
    public class Job :ISearchObject
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Company { get; set; }


        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Compensation { get; set; }

        [DataMember]
        public DateTime? DateTime { get; set; }

        [DataMember]
        public GeographyPoint Location { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string[] Extra { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public DateTime? InsertDate { get; set; }

        public JobFilter JobType { get; set; }

        [DataMember(Name = nameof(JobType))]
        public string JobTypeStr
        {
            get => JobType.GetDescription();
            set
            {
                if (value.TryToEnum(out JobFilter filter))
                {
                    JobType = filter;
                }
            }
        }
    }
}