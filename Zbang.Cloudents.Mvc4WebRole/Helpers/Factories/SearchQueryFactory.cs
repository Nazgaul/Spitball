using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers.Factories
{
    public class SearchQueryFactory
    {
        public SearchQueryBase GetQuery(string query, int pageNumber, SearchType type, long userId)
        {
            return new SearchBoxQuery(userId, query, pageNumber);
            //switch (type)
            //{
            //    //case SearchType.Item:
            //    //    return new SearchItemQuery(userId, query, pageNumber);
            //    case SearchType.Box:
            //        return new SearchBoxQuery(userId, query, pageNumber);
            //    //case SearchType.Library:
            //    //    return new SearchLibraryQuery(userId, query, pageNumber);              
            //    default:
            //        return new SearchBoxQuery(userId, query, pageNumber);
            //}
        }
    }
}