using System;

namespace Cloudents.Core.Query
{
    public class DocumentQuery
    {
        public DocumentQuery(string[] course, Guid? university, string term, string country, int page, string[] sources)
        {
            Course = course;
            University = university;
            Term = term;
            Country = country;
            Page = page;
            Sources = sources;
        }
        public string[] Course { get; set; }
        public Guid? University { get; set; }


        public string Term { get; }
        public string Country { get; }


        public string[] Sources { get;  }

        public int Page { get; }

    }
}