using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.Tutor
{
    public class AzureTutorSearch : ITutorSearch
    {
        private readonly ISearchIndexClient _client;

        public AzureTutorSearch(ISearchService client)
        {
            _client = client.GetClient(TutorSearchWrite.IndexName);

        }
        public async Task<IEnumerable<TutorListDto>> SearchAsync(TutorListTabSearchQuery query, CancellationToken token)
        {
            var searchParams = new SearchParameters()
            {
                Top = 50,
                Skip = query.Page * 50,
                Select = new []
                {
                    nameof(Entities.Tutor.Name),
                    nameof(Entities.Tutor.Id),
                    nameof(Entities.Tutor.Courses),
                    nameof(Entities.Tutor.Image),
                    nameof(Entities.Tutor.Price),
                    nameof(Entities.Tutor.Rate),
                    nameof(Entities.Tutor.ReviewCount),
                    nameof(Entities.Tutor.Bio),
                },
                ScoringProfile = "ScoringProfile"
            };
            if (!string.IsNullOrEmpty(query.Country))
            {
                searchParams.Filter = $"{nameof(Entities.Tutor.Country)} eq '{query.Country.ToUpperInvariant()}'";
            }
            var result = await _client.Documents.SearchAsync<Entities.Tutor>(query.Term, searchParams, cancellationToken: token);
            return result.Results.Select(s => new TutorListDto()
            {
                Name = s.Document.Name,
                UserId = Convert.ToInt64(s.Document.Id),
                Courses = string.Join(",", s.Document.Courses.Take(10)),
                Image = s.Document.Image,
                Price = (decimal)s.Document.Price,
                Rate = (int)s.Document.Rate,
                ReviewsCount = s.Document.ReviewCount,
                Bio = s.Document.Bio
            });

        }
    }
}