using System;
using System.Collections.Generic;

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

    public class BookPricesDto
    {
        public Uri Image { get; set; }
        public string Name { get; set; }

        public string Link { get; set; }
        public string Condition { get; set; } // on ios is enum - no need here i think
        public double Price { get; set; }
    }
}
