using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Search")]
    public class SearchController : Controller
    {
        private readonly IReadRepositoryAsync<UniversitySynonymDto, long> _universitySynonymRepository;
        private readonly IReadRepositoryAsync<IEnumerable<CourseNameDto>, IEnumerable<long>> _courseRepository;

        public SearchController(IReadRepositoryAsync<UniversitySynonymDto, long> universitySynonymRepository,
            IReadRepositoryAsync<IEnumerable<CourseNameDto>, IEnumerable<long>> courseRepository)
        {
            _universitySynonymRepository = universitySynonymRepository;
            _courseRepository = courseRepository;
        }

        private static readonly Task<IEnumerable<string>> EmptyResultTask = Task.FromResult<IEnumerable<string>>(null);

        [Route("documents")]
        public async Task<IActionResult> SearchDocumentAsync([FromQuery] DocumentSearchRequest model,
            CancellationToken token, [FromServices] IDocumentCseSearch searchProvider)
        {
            var parseResult = await ParseUniversityAndCoursesAsync(model.University, model.Course, token).ConfigureAwait(false);
            
            var query = new SearchQuery(model.Term, parseResult.university, parseResult.course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await searchProvider.SearchAsync(query, token).ConfigureAwait(false);
            return Json(result);
        }

        private async Task<(IEnumerable<string> university, IEnumerable<string> course)> ParseUniversityAndCoursesAsync(long? university, long[] course, CancellationToken token)
        {
            var universityTask = EmptyResultTask;
            var courseTask = EmptyResultTask;
            if (university.HasValue)
            {
                universityTask = _universitySynonymRepository.GetAsync(university.Value, token)
                    .ContinueWith(f => f.Result.Name, token);
            }
            if (course != null)
            {
                courseTask = _courseRepository.GetAsync(course, token).ContinueWith(f => f.Result.Select(s => s.Name), token);
            }
            await Task.WhenAll(universityTask, courseTask).ConfigureAwait(false);
            return (universityTask.Result, courseTask.Result);
        }

        [Route("flashcards")]
        public async Task<IActionResult> SearchFlashcardsAsync([FromQuery] SearchRequest model,
            CancellationToken token, [FromServices] IFlashcardSearch searchProvider)
        {
            var parseResult = await ParseUniversityAndCoursesAsync(model.University, model.Course, token).ConfigureAwait(false);
            var query = new SearchQuery(model.Term, parseResult.university, parseResult.course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await searchProvider.SearchAsync(query, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}