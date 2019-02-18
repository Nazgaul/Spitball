using JetBrains.Annotations;
using System.Collections.Generic;

namespace Cloudents.Core.Request
{
    public sealed class BingSearchQuery
    {
        public static BingSearchQuery Document(string query, IEnumerable<string> university,
           string course,
            int page)
        {
            return new BingSearchQuery
            {
                Query = query,
                University = university,
                Course = course,
                Page = page,
            };
        }

        public static BingSearchQuery Flashcard(string query, IEnumerable<string> university,
            string course, IEnumerable<string> sources, int page)
        {
            return new BingSearchQuery
            {
                Query = query,
                University = university,
                Course = course,
                Source = sources,
                Page = page,
            };
        }

        private BingSearchQuery()
        {

        }

        public IEnumerable<string> Source { get; private set; }

        public string Query { get; private set; }
        public int Page { get; private set; }

        public IEnumerable<string> University { get; private set; }
        [CanBeNull] public string Course { get; private set; }

    }
}