using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Library;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public interface IUniversityReadSearchProvider
    {
        Task<IEnumerable<UniversityByPrefixDto>> SearchUniversity(string term);
    }

    public interface IUniversityWriteSearchProvider
    {
        Task BuildUniversityData();
    }

}
