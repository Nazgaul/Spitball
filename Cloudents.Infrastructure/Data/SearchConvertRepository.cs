using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    [UsedImplicitly]
    public class SearchConvertRepository : ISearchConvertRepository
    {
        private readonly IReadRepositoryAsync<UniversitySynonymDto, long> _universitySynonymRepository;
        private readonly IReadRepositoryAsync<IEnumerable<CourseNameDto>, IEnumerable<long>> _courseRepository;

        public SearchConvertRepository(IReadRepositoryAsync<UniversitySynonymDto, long> universitySynonymRepository, IReadRepositoryAsync<IEnumerable<CourseNameDto>, IEnumerable<long>> courseRepository)
        {
            _universitySynonymRepository = universitySynonymRepository;
            _courseRepository = courseRepository;
        }

        public async Task<(IEnumerable<string> universitySynonym, IEnumerable<string> courses)> ParseUniversityAndCoursesAsync(long? university, IEnumerable<long> course, CancellationToken token)
        {
            var universityTask = EmptyResultTask;
            var courseTask = EmptyResultTask;
            if (university.HasValue)
            {
                universityTask = _universitySynonymRepository.GetAsync(university.Value, token)
                    .ContinueWith(f => f.Result?.Name, token);
            }
            if (course != null)
            {
                courseTask = _courseRepository.GetAsync(course, token).ContinueWith(f => f.Result.Select(s => s.Name), token);
            }
            await Task.WhenAll(universityTask, courseTask).ConfigureAwait(false);
            return (universityTask.Result, courseTask.Result);
        }

        private static readonly Task<IEnumerable<string>> EmptyResultTask = Task.FromResult<IEnumerable<string>>(null);
    }
}
