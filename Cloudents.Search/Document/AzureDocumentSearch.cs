using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Documents.Queries.GetDocumentsList;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;

namespace Cloudents.Search.Document
{
    public class AzureDocumentSearch : IDocumentsSearch
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
                            cancellationToken: cancelToken).ConfigureAwait(false);
                return item;
            }
            //item may not exists in the search....
            catch (CloudException)
            {
                return null;
            }
        }


        //public async Task<string> ItemMetaContentAsync(long itemId, CancellationToken cancelToken)
        //{
        //    try
        //    {
        //        var item =
        //            await
        //                _client.Documents.GetAsync<Entities.Document>
        //                    (itemId.ToString(CultureInfo.InvariantCulture),
        //                        new[] { nameof(Entities.Document.MetaContent) },
        //                        cancellationToken: cancelToken)
        //                    .ConfigureAwait(false);
        //        return item.MetaContent;
        //    }
        //    //item may not exists in the search....
        //    catch (CloudException)
        //    {
        //        return null;
        //    }
        //}

        public async Task<IEnumerable<DocumentSearchResultWithScore>> SearchAsync(DocumentQuery query, CancellationToken token)
        {
            var filters = new List<string>
            {
                $"({nameof(Entities.Document.Country)} eq '{query.Profile.Country.ToUpperInvariant()}')" 
               // $" or {nameof(Entities.Document.Language)} eq 'en')"
            };
            if (query.Course != null)
            {
                var filterStr = string.Join(" or ", query.Course.Select(s =>
                    $"{nameof(Entities.Document.Course)} eq '{s.ToUpperInvariant().Replace("'","''")}'"));

                if (!string.IsNullOrWhiteSpace(filterStr))
                {
                    filters.Add($"({filterStr})");
                }
            }

            if (query.Filters != null)
            {
                var filterStr = string.Join(" or ", query.Filters.Select(s =>
                     $"{nameof(Entities.Document.Type)} eq {(int)s}"));
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
                    //nameof(Entities.Document.MetaContent) },
                Top = pageSize,
                Skip = query.Page * pageSize,
                OrderBy = new List<string> { "search.score() desc", $"{nameof(Entities.Document.DateTime)} desc" },
                ScoringProfile = DocumentSearchWrite.ScoringProfile,
                ScoringParameters = new[]
                             {
                                 new ScoringParameter(DocumentSearchWrite.TagsUniversityParameter, new[] {query.Profile.University?.Id.ToString()}),
                                 new ScoringParameter(DocumentSearchWrite.TagsTagsParameter,GenerateScoringParameterValues( query.Profile.Tags)),
                                 new ScoringParameter(DocumentSearchWrite.TagsCourseParameter, GenerateScoringParameterValues( query.Profile.Courses )),
                }

            };

            var result = await
                _client.Documents.SearchAsync<Entities.Document>(query.Term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);

           return result.Results.Select(s => new DocumentSearchResultWithScore
            {
                Id = Convert.ToInt64(s.Document.Id),
                Score = s.Score,
               // MetaContent = s.Document.MetaContent
            });

        }

        public static IEnumerable<string> GenerateScoringParameterValues(IEnumerable<string> input)
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