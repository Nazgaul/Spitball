﻿using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Search.Extensions;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Search.Question
{
    public class AzureQuestionSearch : IQuestionsSearch
    {
        private const int PageSize = 20;
        private readonly ISearchIndexClient _client;

        public AzureQuestionSearch(ISearchService client)
        {
            _client = client.GetClient(QuestionSearchWrite.IndexName);
        }

        public async Task<Entities.Question> GetById(string id)
        {
            var t = await _client.Documents.GetAsync<Entities.Question>(id);
            return t;
        }

        public async Task<(IEnumerable<long> result,  IEnumerable<QuestionFilter> facetFilter)>
            SearchAsync(QuestionsQuery query, CancellationToken token)
        {
            var filters = new List<string>();

            //var country = query.UserProfile.Country ?? query.UserProfile.University?.Country;

            //if (country != null)
            //{
            //    var filter1 = $"{nameof(Entities.Question.Country)} eq '{country.ToUpperInvariant()}'";
            //    filters.Add($"({filter1})");
            //}

            if (query.Course != null)
            {

                var filterStr = $"{nameof(Entities.Question.Course)} eq '{query.Course.ToUpperInvariant().Replace("'", "''")}'";
                filters.Add($"({filterStr})");
            }

            if (query.FilterByUniversity)
            {
                var universityStr = $"{nameof(Entities.Question.UniversityName)} eq '{query.UniversityId.GetValueOrDefault().ToString()}'";
                filters.Add($"({universityStr})");

            }

            if (query.Country != null)
            {
                filters.Add($"{nameof(Entities.Question.Country)} eq '{query.Country.ToUpperInvariant()}'");
            }


            //if (query.Source != null)
            //{
            //    var filterStr = string.Join(" or ", query.Source.Select(s =>
            //        $"{nameof(Entities.Question.Subject)} eq {(int)s}"));

            //    if (!string.IsNullOrEmpty(filterStr))
            //    {
            //        filters.Add($"({filterStr})");
            //    }
            //}

            if (query.Filters != null)
            {
                var filterStr = string.Join(" or ", query.Filters.Select(s =>
                     $"{nameof(Entities.Question.State)} eq {(int)s}"));
                if (!string.IsNullOrEmpty(filterStr))
                {
                    filters.Add($"({filterStr})");
                }
                
            }
            var searchParameter = new SearchParameters
            {
                Filter = string.Join(" and ", filters),
                Select = new[] { nameof(Entities.Question.Id) },
                Top = PageSize,
                Skip = query.Page * PageSize,
                OrderBy = new List<string> { "search.score() desc", $"{nameof(Entities.Question.DateTime)} desc" }
            };
            if (!string.IsNullOrEmpty(query.Term))
            {
                searchParameter.Facets = new[]
                {
                    nameof(Entities.Question.State)
                };
            }

            var result = await
                _client.Documents.SearchAsync<Entities.Question>(query.Term, searchParameter,
                    cancellationToken: token);

           // IEnumerable<QuestionSubject> facetSubject = null;
            IEnumerable<QuestionFilter> questionFilter = null;

            if (result.Facets != null)
            {
              

                if (result.Facets.TryGetValue(nameof(Entities.Question.State), out var p2))
                {
                    questionFilter = p2.AsEnumFacetResult<QuestionFilter>();
                }
            }

            return (result.Results.Select(s => Convert.ToInt64(s.Document.Id)),  questionFilter);
        }


    }
}