using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IMapper = AutoMapper.IMapper;

namespace Cloudents.Infrastructure.Search
{
    [UsedImplicitly]
    public class UniversitySearch : IUniversitySearch
    {
        private readonly ISearchIndexClient _client;

        private readonly string[] _listOfSelectParams = {
            nameof(University.Id),
            nameof(University.DisplayName),
        };

        public UniversitySearch(ISearchService client)
        {
            _client = client.GetClient(UniversitySearchWrite.IndexName);
        }

        public async Task<IEnumerable<UniversityDto>> SearchAsync(string term, string country,
            CancellationToken token)
        {
            var searchParameter = new SearchParameters
            {
                Select = _listOfSelectParams,
                Top = 15,
                OrderBy = new List<string> {"search.score() desc", nameof(University.DisplayName)},
                ScoringProfile = UniversitySearchWrite.ScoringProfile,
                ScoringParameters = new[]
                {
                    new ScoringParameter
                    (UniversitySearchWrite.CountryTagScoringParameters
                        , new[] {country})
                }
            };

            var result = await
                _client.Documents.SearchAsync<University>(term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);
            return result.Results.Select(s => new UniversityDto(long.Parse(s.Document.Id), s.Document.DisplayName));
        }
    }
}

