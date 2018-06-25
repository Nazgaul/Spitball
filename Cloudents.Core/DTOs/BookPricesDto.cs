using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class BookPricesDto : IUrlRedirect
    {
        [DataMember]
        public Uri Image { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Link { get; set; }

        [DataMember]
        public BookCondition Condition { get; set; } // on ios is enum - no need here i think

        [DataMember]
        public double Price { get; set; }

        public string Url {
            get => Link;
            set => Link = value;
        }

        public string Source => Name;
    }
}