using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace Cloudents.Core.Request
{
    public sealed class SearchQuery
    {
        public static SearchQuery Document(string query, long? university,
            IList<string> courses, IEnumerable<string> sources,
            int page)
        {
            return new SearchQuery
            {
                Query = query,
                University = university,
                Courses = courses,
                Source = sources,
                Page = page,
                //Point = point
            };
        }

        public static SearchQuery Flashcard(string query, long? university,
            IList<string> courses, IEnumerable<string> sources, int page)
        {
            return new SearchQuery
            {
                Query = query,
                University = university,
                Courses = courses,
                Source = sources,
                Page = page,
               // Point = point
            };
        }

        private SearchQuery()
        {

        }

        public IEnumerable<string> Source { get; private set; }

        public string Query { get; private set; }
        public int Page { get; private set; }

        public long? University { get; private set; }
        [CanBeNull] public IList<string> Courses { get; private set; }

       // public GeoPoint Point { get; private set; }
    }
}