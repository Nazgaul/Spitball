using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface ISearchReadProvider
    {

        Task<SearchJaredDto> SearchAsync(KnownIntent query, SearchJared extra, CancellationToken cancelToken);
    }
}