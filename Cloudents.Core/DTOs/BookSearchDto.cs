﻿using System.Collections.Generic;
using Cloudents.Common.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Application.DTOs
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

    public enum BookCondition
    {
        None,
        [ResourceDescription(typeof(EnumResources), "BookConditionNew")]
        New,
        [ResourceDescription(typeof(EnumResources), "BookConditionRental")]
        Rental,
        [ResourceDescription(typeof(EnumResources), "BookConditionEBook")]
        EBook,
        [ResourceDescription(typeof(EnumResources), "BookConditionUsed")]
        Used
    }
}
