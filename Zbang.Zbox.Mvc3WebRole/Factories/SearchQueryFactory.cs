using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Mvc3WebRole.Factories
{
    public class SearchQueryFactory
    {
        public SearchQueryBase GetQuery(string query, int pageNumber, SearchType type, long userId)
        {
            switch (type)
            {
                case SearchType.Item:
                    return new SearchItemQuery(userId, query, pageNumber);
                case SearchType.Box:
                    return new SearchBoxQuery(userId, query, pageNumber);
                case SearchType.Library:
                    return new SearchLibraryQuery(userId, query, pageNumber, 1);
                default:
                    return new SearchItemQuery(userId, query, pageNumber);
            }
        }
    }
}