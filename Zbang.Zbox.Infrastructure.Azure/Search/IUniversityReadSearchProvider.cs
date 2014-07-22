using System.Collections.Generic;
using Zbang.Zbox.ViewModel.Dto.Library;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public interface IUniversityReadSearchProvider
    {
        IEnumerable<UniversityByPrefixDto> SearchUniversity(string term);
    }

    public interface IUniversityWriteSearchProvider
    {
        void BuildUniversityData();
    }

}
