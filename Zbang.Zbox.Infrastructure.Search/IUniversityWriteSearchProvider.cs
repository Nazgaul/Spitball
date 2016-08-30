using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Library;

namespace Zbang.Zbox.Infrastructure.Search
{
    //public interface IUniversityWriteSearchProvider
    //{
    //    Task BuildUniversityData();
    //    Task UpdateData(UniversitySearchDto university);
    //}

    public interface IUniversityWriteSearchProvider2
    {
        Task<bool> UpdateDataAsync(IEnumerable<UniversitySearchDto> universityToUpload, IEnumerable<long> universityToDelete);
    }
}