using System;
using System.Runtime.Serialization;

namespace Cloudents.Application.DTOs
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
        public BookCondition Condition { get; set; } 

        [DataMember]
        public double Price { get; set; }

        public string Url {
            get => Link;
            set => Link = value;
        }

        public string Source => Name;
    }
}