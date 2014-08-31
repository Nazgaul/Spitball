using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Library;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public interface IUniversityWriteSearchProvider
    {
        Task BuildUniversityData();
        Task UpdateData(UniversitySearchDto university);
    }
}