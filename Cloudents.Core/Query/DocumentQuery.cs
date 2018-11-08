using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Query
{
    public class DocumentQuery
    {
        public DocumentQuery(string[] course, Guid? university, string term, string country, int page, IEnumerable<DocumentType> filters)
        {
            Course = course;
            University = university;
            Term = term;
            Country = country;
            Page = page;
            Filters = filters ?? Enumerable.Empty<DocumentType>();
        }
        public string[] Course { get; set; }
        public Guid? University { get; set; }


        public string Term { get; }
        public string Country { get; }



        public int Page { get; }

        public IEnumerable<DocumentType> Filters { get; }

    }
}