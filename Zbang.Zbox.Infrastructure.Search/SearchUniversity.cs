using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    [DataContract]
    public class SearchUniversity
    {
        [DataMember(Name = "@Search.Action")]
        public string SearchAction { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Extra1 { get; set; }
        [DataMember]
        public string Extra2 { get; set; }
        [DataMember]
        public string Extra3 { get; set; }
        [DataMember]
        public string Extra4 { get; set; }

        [DataMember]
        public string ImageField { get; set; }
    }
}