using Microsoft.Azure.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface ISearchConnection
    {
     //   IndexQueryClient IndexQuery { get; }
     //   IndexManagementClient IndexManagement { get; }
        SearchServiceClient SearchClient { get; }
        bool IsDevelop { get; }
    }
}
