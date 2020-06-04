﻿using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Search.Tutor
{
    public class AzureTutorSearch : ITutorSearch
    {
        private readonly ISearchIndexClient _client;
        private readonly IUrlBuilder _urlBuilder;

        public AzureTutorSearch(ISearchService client, IUrlBuilder urlBuilder)
        {
            _urlBuilder = urlBuilder;
            _client = client.GetClient(TutorSearchWrite.IndexName);
        }

        public Task<Microsoft.Azure.Search.Models.Document> GetByIdAsync(long id)
        {
            return _client.Documents.GetAsync(id.ToString());
            //return await _client.Documents.GetAsync<Entities.Tutor>(id.ToString());
        }
        public async Task<ListWithCountDto<TutorCardDto>> SearchAsync(TutorListTabSearchQuery query, CancellationToken token)
        {
            //const int pageSize = 25;
            var searchParams = new SearchParameters
            {
                Top = query.PageSize,
                Skip = query.Page * query.PageSize,
                Select = new[] {nameof(Entities.Tutor.Data), nameof(Entities.Tutor.SbCountry)},
                HighlightFields = new[] {nameof(Entities.Tutor.Courses)},
                HighlightPostTag = string.Empty,
                HighlightPreTag = string.Empty,
                SearchFields =
                    new[]
                    {
                        nameof(Entities.Tutor.Name), nameof(Entities.Tutor.Prefix),
                        nameof(Entities.Tutor.Courses), nameof(Entities.Tutor.Subjects)
                    },
                ScoringProfile = TutorSearchWrite.ScoringProfile,
                IncludeTotalResultCount = true,
                Filter = $"{nameof(Entities.Tutor.SbCountry)} eq {query.Country.Id}"
            };
            var result = await _client.Documents.SearchAsync<Entities.Tutor>(query.Term, searchParams, cancellationToken: token);

            var obj = new ListWithCountDto<TutorCardDto>()
            {
                Result = result.Results.Where(w => w.Document.Data != null).Select(s =>
                {
                    var tutor = s.Document.Data;
                    var courses = (s.Highlights?[nameof(Entities.Tutor.Courses)] ?? Enumerable.Empty<string>()).Union(
                        s.Document.Data.Courses).Take(3).Distinct(StringComparer.OrdinalIgnoreCase);
                    if (tutor.Image != null)
                    {
                        s.Document.Data.Image = _urlBuilder.BuildUserImageEndpoint(tutor.UserId, tutor.Image);
                    }

                    //s.Document.Data.SbCountry = s.Document.SbCountry;
                    s.Document.Data.Courses = courses;
                    s.Document.Data.Subjects = s.Document.Data.Subjects?.Take(3);
                    return s.Document.Data;
                }),
                Count = result.Count
            };
            return obj;

        }
    }
}