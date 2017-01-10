using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public interface IWitAi : IAi
    {
        Task UpdateCourseEntityAsync(IEnumerable<string> courses);
        Task UpdateUniversityEntityAsync(IEnumerable<UniversityEntityDto> universities);
    }
}