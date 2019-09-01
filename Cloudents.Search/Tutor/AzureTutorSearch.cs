﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Search.Tutor
{
    public class AzureTutorSearch : ITutorSearch
    {
        private readonly ISearchIndexClient _client;

        public AzureTutorSearch(ISearchService client)
        {
            _client = client.GetClient(TutorSearchWrite.IndexName);

        }

        public async Task<Entities.Tutor> GetByIdAsync(long id)
        {
           return await _client.Documents.GetAsync<Entities.Tutor>(id.ToString());
        }
        public async Task<IEnumerable<TutorCardDto>> SearchAsync(TutorListTabSearchQuery query, CancellationToken token)
        {
            const int pageSize = 25;
            var searchParams = new SearchParameters()
            {
                Top = pageSize,
                Skip = query.Page * pageSize,
                Select = new[]
                {
                    nameof(Entities.Tutor.Data),
                    //nameof(Entities.Tutor.Id),
                    //nameof(Entities.Tutor.Courses),
                    //nameof(Entities.Tutor.Image),
                    //nameof(Entities.Tutor.Price),
                    ////nameof(Entities.Tutor.Rate),
                    //Entities.Tutor.RateFieldName,
                    //nameof(Entities.Tutor.ReviewCount),
                    //nameof(Entities.Tutor.Bio),
                },
                HighlightFields = new[] { nameof(Entities.Tutor.Courses) },
                HighlightPostTag = string.Empty,
                HighlightPreTag = string.Empty,
                SearchFields = new[] { nameof(Entities.Tutor.Name),
                    nameof(Entities.Tutor.Prefix),
                    nameof(Entities.Tutor.Courses),
                    nameof(Entities.Tutor.Subjects)

                },
                ScoringProfile = TutorSearchWrite.ScoringProfile,
                //OrderBy = new List<string> { "search.score() desc", $"{Entities.Tutor.RateFieldName} desc" }
            };
            if (!string.IsNullOrEmpty(query.Country))
            {
                searchParams.Filter = $"{nameof(Entities.Tutor.Country)} eq '{query.Country.ToUpperInvariant()}'";
            }
            var result = await _client.Documents.SearchAsync<Entities.Tutor>(query.Term, searchParams, cancellationToken: token);
            return result.Results.Where(w => w.Document.Data != null).Select(s =>
              {
                  var courses = (s.Highlights?[nameof(Entities.Tutor.Courses)] ?? Enumerable.Empty<string>()).Union(
                      s.Document.Data.Courses).Take(3).Distinct(StringComparer.OrdinalIgnoreCase);

                  s.Document.Data.Courses = courses;
                  s.Document.Data.Subjects = s.Document.Data.Subjects?.Take(3);
                  return s.Document.Data;

                //return new TutorCardDto
                //{
                //    Name = s.Document.Name,
                //    UserId = Convert.ToInt64(s.Document.Id),
                //    Courses = courses,

                //    Image = s.Document.Image,
                //    Price = (decimal)s.Document.Price,
                //    Rate = (float)s.Document.Rate,
                //    ReviewsCount = s.Document.ReviewCount,
                //    Bio = s.Document.Bio,
                //    CourseCount = s.Document.Courses.Length,
                //    University = "Some university", // TODO
                //    Lessons = 100 //TODO
                //};
            });

        }
    }
}