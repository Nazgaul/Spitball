using Cloudents.Core.Documents.Queries.GetDocumentsList;
using Cloudents.Core.Interfaces;
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

        public async Task< IEnumerable<string>> GetNone()
        {
           var t =await  _client.Documents.SearchAsync<Entities.Document>(null, new SearchParameters
            {
                Filter = "TypeFieldName eq 'None'",
                Top = 2000,
                Select = new [] {"Id"}
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
                            new[] { nameof(Entities.Document.Content) }, cancellationToken: cancelToken).ConfigureAwait(false);
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



        public async Task<(IEnumerable<DocumentSearchResultWithScore> result, IEnumerable<string> facetSubject)> SearchAsync(DocumentQuery query, CancellationToken token)
        {
            var country = query.Profile.University?.Country ?? query.Profile.Country;
            var filters = new List<string>
            {
                $"({nameof(Entities.Document.Country)} eq '{country.ToUpperInvariant()}')"
            };
            if (query.Course != null)
            {
                var filterStr = string.Join(" or ", query.Course.Select(s =>
                    $"{Entities.Document.CourseNameField} eq '{s.ToUpperInvariant().Replace("'", "''")}'"));

                if (!string.IsNullOrWhiteSpace(filterStr))
                {
                    filters.Add($"({filterStr})");
                }
            }

            if (query.Filters != null)
            {
                var filterStr = string.Join(" or ", query.Filters.Select(s =>
                     $"{Entities.Document.TypeFieldName} eq '{s}'"));
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
                                 new ScoringParameter(DocumentSearchWrite.TagsUniversityParameter, new[] {query.Profile.University?.Id.ToString()}),
                                 new ScoringParameter(DocumentSearchWrite.TagsTagsParameter,GenerateScoringParameterValues( query.Profile.Tags)),
                                 new ScoringParameter(DocumentSearchWrite.TagsCourseParameter, GenerateScoringParameterValues( query.Profile.Courses )),
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

            if (result.Facets != null)
            {
                if (result.Facets.TryGetValue(Entities.Document.TypeFieldName, out var p))

                {
                    facetDocumentType = p.Select(s => s.Value.ToString());//.AsEnumFacetResult<DocumentType>();
                }


            }

            return (result.Results.Select(s => new DocumentSearchResultWithScore
            {
                Id = Convert.ToInt64(s.Document.Id),
                Score = s.Score,
            }), facetDocumentType);

        }

        private static IEnumerable<string> GenerateScoringParameterValues(IEnumerable<string> input)
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