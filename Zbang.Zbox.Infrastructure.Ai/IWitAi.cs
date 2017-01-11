using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public interface IWitAi : IAi
    {
        Task AddCoursesEntityAsync(IEnumerable<string> courses, CancellationToken token);
        Task AddUniversitiesEntityAsync(IEnumerable<UniversityEntityDto> universities, CancellationToken token);
    }
}