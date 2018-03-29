using System.Runtime.Serialization;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Search
{
    [DataContract]
    public class AutoComplete : ISearchObject
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Prefix { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public Vertical Vertical { get; set; }
    }
}