using System.Collections.Generic;
using Zbang.Zbox.Infrastructure;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchItemsQuery : SearchQuery
    {
        public SearchItemsQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50)
            : base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey => "items " + GetUniversityId();
    }

    public class SearchJared
    {
        public SearchJared(Language language)
        {
            Language = language;
        }

        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Courses { get; set; }
        public string University { get; set; }

        public Language Language { get; private set; }
    }
}