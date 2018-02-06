using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    public class BookSearchDto
    {
        public string Isbn10 { get; set; }
        public string Isbn13 { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Edition { get; set; }
        public string Binding { get; set; }
        public string Image { get; set; }
    }

    public class BookDetailsDto
    {
        public BookSearchDto Details { get; set; }
        public IEnumerable<BookPricesDto> Prices { get; set; }
    }

    [DataContract]
    public class BookPricesDto
    {
        [DataMember]
        public Uri Image { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Link { get; set; }
        [DataMember]
        public string Condition { get; set; } // on ios is enum - no need here i think
        [DataMember]
        public double Price { get; set; }

    }
}
