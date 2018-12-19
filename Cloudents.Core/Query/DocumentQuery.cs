﻿using System.Collections.Generic;
using System.Linq;
using Cloudents.Application.Models;
using Cloudents.Common.Enum;

namespace Cloudents.Application.Query
{
    public class DocumentQuery
    {
        public DocumentQuery(string[] course, UserProfile profile, string term, int page, IEnumerable<DocumentType> filters)
        {
            Course = course;
            Term = term;
            Page = page;
            Filters = filters ?? Enumerable.Empty<DocumentType>();
            Profile = profile;
        }
        public string[] Course { get; set; }


        public string Term { get; }
        public UserProfile Profile { get; }



        public int Page { get; }

        public IEnumerable<DocumentType> Filters { get; }

    }
}