using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IUniversityReadSearchProvider
    {
        Task<IEnumerable<UniversityByPrefixDto>> SearchUniversityAsync(UniversitySearchQuery query, CancellationToken cancelToken);
    }
}
