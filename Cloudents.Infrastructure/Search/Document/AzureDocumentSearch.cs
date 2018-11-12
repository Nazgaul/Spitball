using Cloudents.Core.Query;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Search.Document
{
    public class AzureDocumentSearch
    {
        private readonly ISearchIndexClient _client;

        public AzureDocumentSearch(ISearchService client)
        {
            _client = client.GetClient(DocumentSearchWrite.IndexName);

        }

        public async Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken)
        {
            try
            {
                var item =
                    await
                        _client.Documents.GetAsync<Core.Entities.Search.Document>
                        (itemId.ToString(CultureInfo.InvariantCulture),
                            new[] { nameof(Core.Entities.Search.Document.Content) }, cancellationToken: cancelToken).ConfigureAwait(false);
                return item.Content;
            }
            //item may not exists in the search....
            catch (CloudException)
            {
                return null;
            }
        }


        public async Task<string> ItemMetaContentAsync(long itemId, CancellationToken cancelToken)
        {
            try
            {
                var item =
                    await
                        _client.Documents.GetAsync<Core.Entities.Search.Document>
                            (itemId.ToString(CultureInfo.InvariantCulture),
                                new[] { nameof(Core.Entities.Search.Document.MetaContent) },
                                cancellationToken: cancelToken)
                            .ConfigureAwait(false);
                return item.MetaContent;
            }
            //item may not exists in the search....
            catch (CloudException)
            {
                return null;
            }
        }

        public async Task<DocumentSearchResult<Core.Entities.Search.Document>> SearchAsync(DocumentQuery query, CancellationToken token)
        {
            var filters = new List<string>
            {
                $"({nameof(Core.Entities.Search.Document.Country)} eq '{query.Profile.Country}' or {nameof(Core.Entities.Search.Document.Language)} eq 'en')"
            };
            if (query.Course != null)
            {
                var filterStr = string.Join(" or ", query.Course.Select(s =>
                    $"{nameof(Core.Entities.Search.Document.Course)} eq '{s}'"));

                if (!string.IsNullOrWhiteSpace(filterStr))
                {
                    filters.Add($"({filterStr})");
                }
            }

            if (query.Filters != null)
            {
                var filterStr = string.Join(" or ", query.Filters.Select(s =>
                     $"{nameof(Core.Entities.Search.Document.Type)} eq {(int)s}"));
                if (!string.IsNullOrWhiteSpace(filterStr))
                {
                    filters.Add($"({filterStr})");
                }
            }
            var searchParameter = new SearchParameters
            {
                //Facets = new[] { nameof(Document.Subject),
                //    nameof(Document.State) },
                Filter = string.Join(" and ", filters),
                Select = new[] { nameof(Core.Entities.Search.Document.Id),
                    nameof(Core.Entities.Search.Document.MetaContent) },
                Top = 50,
                Skip = query.Page * 50,
                OrderBy = new List<string> { "search.score() desc", $"{nameof(Core.Entities.Search.Document.DateTime)} desc" },
                ScoringProfile = DocumentSearchWrite.ScoringProfile,
                ScoringParameters = new[]
                             {
                                 new ScoringParameter(DocumentSearchWrite.TagsUniversityParameter, new[] {query.Profile.University?.Id.ToString()}),
                                 new ScoringParameter(DocumentSearchWrite.TagsTagsParameter,GenerateScoringParameterValues( query.Profile.Tags)),
                                 new ScoringParameter(DocumentSearchWrite.TagsCourseParameter, GenerateScoringParameterValues( query.Profile.Courses )),
                }

            };

            var result = await
                _client.Documents.SearchAsync<Core.Entities.Search.Document>(query.Term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);

            return result;
        }

        public IEnumerable<string> GenerateScoringParameterValues(IEnumerable<string> input)
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

            return inputList;
        }
    }
}