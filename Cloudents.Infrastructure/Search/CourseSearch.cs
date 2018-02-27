using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search
{
    [UsedImplicitly]
    public class CourseSearch : ICourseSearch
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        public CourseSearch(ISearchServiceClient client, IMapper mapper)
        {
            _client = client.Indexes.GetClient(CourseSearchWrite.IndexName);
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseDto>> SearchAsync([CanBeNull]string term, long universityId,
            CancellationToken token)
        {
            term = term?.Replace(":", @"\:");

            var tResult = _client.Documents.SearchAsync<Course>(term, new SearchParameters
            {
                Select = new[] { nameof(Course.Id), nameof(Course.Name) },
                Top = 40,
                Filter = $"{nameof(Course.UniversityId)} eq {universityId}",
                ScoringProfile = CourseSearchWrite.ScoringProfile

            }, cancellationToken: token);

            await Task.WhenAll(tResult).ConfigureAwait(false);

            return _mapper.Map<IEnumerable<Course>, IList<CourseDto>>(tResult.Result.Results.Select(s => s.Document));
        }
    }
}
