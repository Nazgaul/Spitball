using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search.Entities;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search
{
    public class CourseSearch : ICourseSearch
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        public CourseSearch(ISearchServiceClient client, IMapper mapper)
        {
            _client = client.Indexes.GetClient("box2");
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseDto>> SearchAsync(string term, long universityId,
            CancellationToken token)
        {
            if (!term.Contains(" "))
            {
                term += "*";
            }

            var tResult = _client.Documents.SearchAsync<Course>(term, new SearchParameters
            {
                Select = new[] { "id", "name2" },
                Top = 40,
                SearchFields = new[] { "course2", "name2" },
                Filter = $"universityId eq {universityId}"
            }, cancellationToken: token);

            await Task.WhenAll(tResult).ConfigureAwait(false);

            return _mapper.Map<IEnumerable<Course>, IList<CourseDto>>(tResult.Result.Results.Select(s => s.Document));
        }
    }
}
