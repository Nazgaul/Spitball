using System;
using System.Runtime.Serialization;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class JobDto : IShuffleable
    {
        private string _responsibilities;

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Responsibilities
        {
            get => _responsibilities.RemoveEndOfString(300);
            set => _responsibilities = value;
        }

        [DataMember]
        public DateTime DateTime { get; set; }
        
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string CompensationType { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public string Source { get; set; }
        public object Bucket => Source;
    }
}
