﻿using System.Collections.Generic;
using Cloudents.Core.Models;

namespace Cloudents.Core.Request
{
    public class SearchQuery
    {
        public static SearchQuery Document(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> sources, 
            int page, string docType, GeoPoint point)
        {
            return new SearchQuery
            {
                Query = query,
                University = university,
                Courses = courses,
                Source = sources,
                Page = page,
                DocType = docType,
                Point = point
            };
        }

        public static SearchQuery Flashcard(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> sources, int page, GeoPoint point)
        {
            return new SearchQuery
            {
                Query = query,
                University = university,
                Courses = courses,
                Source = sources,
                Page = page,
                Point = point
            };
        }

       

        private SearchQuery()
        {

        }

       

        public IEnumerable<string> Source { get; private set; }
        
        public IEnumerable<string> Query { get; private set; }
        public int Page { get; private set; }

        public string DocType { get; private set; }
        public long? University { get; private set; }
        public IEnumerable<long> Courses { get; private set; }

        public GeoPoint Point { get; private set; }
    }
}