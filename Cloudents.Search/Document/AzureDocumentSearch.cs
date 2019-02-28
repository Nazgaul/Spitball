﻿using Cloudents.Core.Documents.Queries.GetDocumentsList;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Search.Document
{
    public class AzureDocumentSearch : IDocumentsSearch
    {
        private readonly ISearchIndexClient _client;

        public AzureDocumentSearch(ISearchService client)
        {
            _client = client.GetClient(DocumentSearchWrite.IndexName);

        }

        public async Task<IEnumerable<string>> GetNone(int page)
        {
            var t = await _client.Documents.SearchAsync<Entities.Document>(null, new SearchParameters
            {
                Filter = "TypeFieldName eq 'None'",
                Top = 1000,
                Skip = page * 1000,
                Select = new[] { "Id" }
            });
            return t.Results.Select(s => s.Document.Id);

        }

        public async Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken)
        {
            try
            {
                var item =
                    await
                        _client.Documents.GetAsync<Entities.Document>
                        (itemId.ToString(CultureInfo.InvariantCulture),
                            new[] { nameof(Entities.Document.Content) }, cancellationToken: cancelToken);
                return item.Content;
            }
            //item may not exists in the search....
            catch (CloudException)
            {
                return null;
            }
        }


        // ReSharper disable once UnusedMember.Global - we used that for testing
        public async Task<Entities.Document> ItemAsync(long itemId, CancellationToken cancelToken)
        {
            try
            {
                var item =
                    await
                        _client.Documents.GetAsync<Entities.Document>
                        (itemId.ToString(CultureInfo.InvariantCulture),
                            cancellationToken: cancelToken);
                return item;
            }
            //item may not exists in the search....
            catch (CloudException)
            {
                return null;
            }
        }



        public async Task<(IEnumerable<DocumentSearchResultWithScore> result, IEnumerable<string> facetSubject)>
            SearchAsync(DocumentQuery query, UserProfile userProfile, CancellationToken token)
        {
            var country = userProfile.University?.Country ?? userProfile.Country;
            var filters = new List<string>
            {
                $"({nameof(Entities.Document.Country)} eq '{country.ToUpperInvariant()}')"
            };
            if (query.Course != null)
            {

                var filterStr = $"{Entities.Document.CourseNameField} eq '{query.Course.ToUpperInvariant().Replace("'", "''")}'";
                filters.Add($"({filterStr})");
            }

            if (query.FilterByUniversity && userProfile.University != null)
            {
                var universityStr = $"{Entities.Document.UniversityIdFieldName} eq '{userProfile.University.Id.ToString()}'";
                filters.Add($"({universityStr})");

            }

            if (query.Filters != null)
            {
                var filterStr = string.Join(" or ", query.Filters.Select(s =>
                     $"{Entities.Document.TypeFieldName} eq '{s.Replace("'", "''")}'"));
                if (!string.IsNullOrWhiteSpace(filterStr))
                {
                    filters.Add($"({filterStr})");
                }
            }

            const int pageSize = 20;
            var searchParameter = new SearchParameters
            {
                Filter = string.Join(" and ", filters),
                Select = new[] { nameof(Entities.Document.Id) },
                Top = pageSize,
                Skip = query.Page * pageSize,
                OrderBy = new List<string> { "search.score() desc", $"{nameof(Entities.Document.DateTime)} desc" },
                ScoringProfile = DocumentSearchWrite.ScoringProfile,
                ScoringParameters = new[]
                             {
                                 new ScoringParameter(DocumentSearchWrite.TagsUniversityParameter, new[] { userProfile.University?.Id.ToString()}),
                                 new ScoringParameter(DocumentSearchWrite.TagsTagsParameter,GenerateScoringParameterValues( userProfile.Tags)),
                                 new ScoringParameter(DocumentSearchWrite.TagsCourseParameter, GenerateScoringParameterValues( userProfile.Courses )),
                },
                Facets = new[]
                {
                    Entities.Document.TypeFieldName
                }

            };
            IEnumerable<string> facetDocumentType = null;
            var result = await
                _client.Documents.SearchAsync<Entities.Document>(query.Term, searchParameter,
                    cancellationToken: token);

            if (result.Facets != null && result.Facets.TryGetValue(Entities.Document.TypeFieldName, out var p))
            {
                facetDocumentType = p.Select(s => s.Value.ToString());//.AsEnumFacetResult<DocumentType>();
            }

            return (result.Results.Select(s => new DocumentSearchResultWithScore
            {
                Id = Convert.ToInt64(s.Document.Id),
                Score = s.Score,
            }), facetDocumentType);

        }

        internal static IEnumerable<string> GenerateScoringParameterValues(IEnumerable<string> input)
        {
            if (input == null)
            {
                return new string[] { null };
            }

            var inputList = input.ToList();
            if (!inputList.Any())
            {
                return new string[] { null };
            }

            return inputList.Select(w => w.ToUpperInvariant());
        }
    }
}