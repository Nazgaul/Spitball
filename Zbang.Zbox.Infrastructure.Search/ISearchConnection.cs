using Microsoft.Azure.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface ISearchConnection
    {
        SearchServiceClient SearchClient { get; }
        bool IsDevelop { get; }
    }
}
