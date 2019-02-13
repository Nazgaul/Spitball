using JetBrains.Annotations;
using System.Collections.Generic;

namespace Cloudents.Core.Request
{
    public sealed class SearchQuery
    {
        public static SearchQuery Document(string query, IEnumerable<string> university,
           string course,
            int page)
        {
            return new SearchQuery
            {
                Query = query,
                University = university,
                Course = course,
                Page = page,
            };
        }

        public static SearchQuery Flashcard(string query, IEnumerable<string> university,
            string course, IEnumerable<string> sources, int page)
        {
            return new SearchQuery
            {
                Query = query,
                University = university,
                Course = course,
                Source = sources,
                Page = page,
            };
        }

        private SearchQuery()
        {

        }

        public IEnumerable<string> Source { get; private set; }

        public string Query { get; private set; }
        public int Page { get; private set; }

        public IEnumerable<string> University { get; private set; }
        [CanBeNull] public string Course { get; private set; }

    }
}