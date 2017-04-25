﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;
using FacetResult = Zbang.Zbox.ViewModel.Dto.Search.FacetResult;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class ContentSearchProvider : IContentWriteSearchProvider
    {
        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;
        private readonly string m_IndexName = "items2";
        private bool m_CheckIndexExists;


        internal const string ContentEnglishField = "content_en";
        internal const string ContentHebrewField = "content_he";

        public ContentSearchProvider(ISearchConnection connection)
        {
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {
                m_IndexName = m_IndexName + "-dev";
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        public async Task UpdateDataAsync(ItemSearchDto itemToUpload, IEnumerable<ItemToDeleteSearchDto> itemToDelete, CancellationToken token)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndexAsync().ConfigureAwait(false);

            }
            if (itemToUpload != null)
            {
                var uploadBatch = new Item
                {
                    Id = itemToUpload.SearchContentId,
                    Name = Path.GetFileNameWithoutExtension(itemToUpload.Name)?.ToLowerInvariant(),
                    Course = itemToUpload.BoxName.ToLowerInvariant(),
                    Professor = itemToUpload.BoxProfessor?.ToLowerInvariant(),
                    Code = itemToUpload.BoxCode?.ToLowerInvariant(),
                    University = itemToUpload.UniversityName.ToLowerInvariant(),
                    Type = itemToUpload.Type.Select(s => ((int)s).ToString()).ToArray(),
                    Tags = itemToUpload.Tags?.Select(s => s.Name.ToLowerInvariant()).Distinct().ToArray(),
                    Date = itemToUpload.Date.Truncate(TimeSpan.FromSeconds(1)),
                    MetaContent = itemToUpload.MetaContent,
                    BlobName = itemToUpload.BlobName,
                    Views = itemToUpload.Views,
                    Likes = itemToUpload.Likes,
                    ContentCount = itemToUpload.ContentCount,
                    CourseId = itemToUpload.BoxId
                };
                if (!string.IsNullOrEmpty(itemToUpload.Content))
                {
                    switch (itemToUpload.Language)
                    {
                        case Language.Undefined:
                            uploadBatch.Content = itemToUpload.Content;
                            break;
                        case Language.EnglishUs:
                            uploadBatch.ContentEn = itemToUpload.Content;
                            break;
                        case Language.Hebrew:
                            uploadBatch.ContentHe = itemToUpload.Content;
                            break;
                        case null:
                            uploadBatch.Content = itemToUpload.Content;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                var batch = IndexBatch.MergeOrUpload(new[] { uploadBatch });
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token).ConfigureAwait(false);
            }
            if (itemToDelete != null)
            {
                var deleteBatch = itemToDelete.Select(s => new ItemSearch
                {
                    Id = s.SearchContentId
                });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token).ConfigureAwait(false);
            }
        }

        private async Task BuildIndexAsync()
        {
            try
            {
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on item build index", ex);
            }
            m_CheckIndexExists = true;
        }

        private const string ScoringProfile = "score";
        private Index GetIndexStructure()
        {
            var definition = new Index
            {
                Name = m_IndexName,
                Fields = FieldBuilder.BuildForType<Item>()
                //Suggesters = new List<Suggester>
                //{
                //    new Suggester
                //    {
                //        Name = "sg", SourceFields = new List<string>
                //        {
                //            nameof(Item.Course).ToLower(),
                //            nameof(Item.Code).ToLower(),
                //            nameof(Item.Professor).ToLower(),
                //            nameof(Item.University).ToLower(),
                //            nameof(Item.Tags).ToLower()
                //        }
                //    }
                //}
            };

            var weightProfile = new ScoringProfile(ScoringProfile);

            var d = new Dictionary<string, double>
            {
                { nameof(Item.Tags).ToLower(), 3},
                { nameof(Item.Name).ToLower(), 4},
                { ContentEnglishField, 2},
                { ContentHebrewField, 2},
            };
            weightProfile.TextWeights = new TextWeights(d);


            var tagFunction = new TagScoringFunction
            {
                Boost = 8,
                FieldName = nameof(Item.Tags).ToLower(),
                Parameters = new TagScoringParameters("tag")
            };
            var tagFunction2 = new TagScoringFunction
            {
                Boost = 10,
                FieldName = nameof(Item.Course).ToLower(),
                Parameters = new TagScoringParameters("course")
            };
            var tagFunction3 = new TagScoringFunction
            {
                Boost = 5,
                FieldName = nameof(Item.University).ToLower(),
                Parameters = new TagScoringParameters("university")
            };
            var freshnessFunction = new FreshnessScoringFunction()
            {
                Boost = 5,
                FieldName = nameof(Item.Date).ToLower(),
                Interpolation = ScoringFunctionInterpolation.Quadratic,
                Parameters = new FreshnessScoringParameters(TimeSpan.FromDays(100))
            };
            var likesScore = new MagnitudeScoringFunction()
            {
                Boost = 4,
                FieldName = nameof(Item.Likes).ToLower(),
                Parameters = new MagnitudeScoringParameters
                {
                    BoostingRangeStart = 1,
                    BoostingRangeEnd = int.MaxValue,
                    ShouldBoostBeyondRangeByConstant = false
                }
            };
            var viewsScore = new MagnitudeScoringFunction()
            {
                Boost = 5,
                FieldName = nameof(Item.Views).ToLower(),
                Parameters = new MagnitudeScoringParameters
                {
                    BoostingRangeStart = 1,
                    BoostingRangeEnd = int.MaxValue,
                    ShouldBoostBeyondRangeByConstant = false
                }
            };
            weightProfile.Functions = new List<ScoringFunction> { tagFunction, tagFunction2, tagFunction3 , freshnessFunction, likesScore, viewsScore };
            definition.ScoringProfiles = new List<ScoringProfile> { weightProfile };

            return definition;
        }

        public async Task<SearchJaredDto> SearchAsync(KnownIntent query, SearchJared extra, CancellationToken cancelToken)
        {
            if (extra == null) throw new ArgumentNullException(nameof(extra));
            var queryDocument = query as SearchDocumentIntent;
            if (queryDocument == null)
            {
                throw new NullReferenceException("queryDocument is null");
            }
            if (string.IsNullOrEmpty(queryDocument.Term))
            {
                queryDocument.Term = "*";
            }
            var searchFiled = new List<string>()
            {
                nameof(Item.Name).ToLower(),
                nameof(Item.Tags).ToLower(),
            };
            switch (extra.Language)
            {
                case Language.Undefined:
                    searchFiled.Add(ContentEnglishField);
                    searchFiled.Add(nameof(Item.Content).ToLower());
                    break;
                case Language.EnglishUs:
                    searchFiled.Add(ContentEnglishField);
                    break;
                case Language.Hebrew:
                    searchFiled.Add(ContentHebrewField);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var searchResult = await m_IndexClient.Documents.SearchAsync<Item>(queryDocument.Term, new SearchParameters()
            {
                Top = 5,
                Filter = BuildFilter(queryDocument.Course, queryDocument.TypeToSearch, query.University),
                IncludeTotalResultCount = true,
                Facets = BuildFacet(queryDocument.Course, query.University),
                ScoringParameters = new[] {new ScoringParameter("tag", extra.Tags ?? new[] {string.Empty}),
                    new ScoringParameter("course", extra.Courses ?? new[] {string.Empty}), new ScoringParameter("university", new[] {extra.University ?? string.Empty})},
                ScoringProfile = ScoringProfile,
                SearchFields = searchFiled
            }, cancellationToken: cancelToken).ConfigureAwait(false);


            var retVal = new SearchItemResult
            {
                Result = searchResult.Results.Select(s => new SearchItem
                {
                    Id = s.Document.Id,
                    Code = s.Document.Code,
                    Course = s.Document.Course,
                    Name = s.Document.Name,
                    Professor = s.Document.Professor,
                    Tags = s.Document.Tags,
                    Type = s.Document.Type.Select(int.Parse),
                    University = s.Document.University
                }),
                Facet = new Dictionary<string, IEnumerable<FacetResult>>(),
            };
            foreach (var facetResult in searchResult.Facets)
            {
                retVal.Facet[facetResult.Key] = facetResult.Value.Select(s => new FacetResult
                {
                    Name = s.Value.ToString(),
                    Value = s.Count.GetValueOrDefault()
                });
            }
            return retVal;
        }

        private IList<string> BuildFacet(string course, string university)
        {
            var expressions = new List<string>();
            if (string.IsNullOrEmpty(course))
            {
                expressions.Add(nameof(Item.Course).ToLower());
            }
            if (string.IsNullOrEmpty(university))
            {
                expressions.Add(nameof(Item.University).ToLower());
            }
            return expressions;
        }

        private string BuildFilter(string course, ItemType? typeToSearch, string university)
        {
            var expressions = new List<string>();
            if (!string.IsNullOrEmpty(course))
            {
                expressions.Add($"{nameof(Item.Course).ToLower()} eq '{course}'");
            }
            if (!string.IsNullOrEmpty(university))
            {
                expressions.Add($"{nameof(Item.University).ToLower()} eq '{university}'");
            }
            if (typeToSearch.HasValue)
            {
                expressions.Add($"{nameof(Item.Type).ToLower()} eq '{(int)typeToSearch}'");
            }
            return string.Join(" and ", expressions);
        }
    }
}
