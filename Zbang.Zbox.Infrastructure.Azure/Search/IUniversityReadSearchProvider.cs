using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public interface IUniversityReadSearchProvider
    {
        Task<IEnumerable<UniversityByPrefixDto>> SearchUniversity(UniversitySearchQuery query);
    }
}
