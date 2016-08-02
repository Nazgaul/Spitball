﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using System.IO;
using Microsoft.Rest.Azure;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class ItemSearchProvider3 : IItemReadSearchProvider2, IItemWriteSearchProvider3
    {
        private readonly string m_IndexName = "item3";
        private bool m_CheckIndexExists;
        private readonly ISearchConnection m_Connection;
        private readonly SearchIndexClient m_IndexClient;
        private readonly ISearchFilterProvider m_FilterProvider;


        public ItemSearchProvider3(ISearchFilterProvider filterProvider, ISearchConnection connection)
        {
            m_FilterProvider = filterProvider;
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {
                m_IndexName = m_IndexName + "-dev";
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string ImageField = "image";
        private const string BoxNameField = "boxName";
        private const string ContentField = "content";
        private const string SmallContentField = "metaContent";
        private const string UrlField = "url";
        private const string UniversityNameField = "universityName";
        private const string UniversityidField = "universityId";
        private const string UserIdsField = "userId";
        private const string BoxIdField = "boxId";
        private const string BoxId2Field = "boxId2";
        private const string ExtensionField = "extension";
        private const string BlobNameField = "blobName";

        private const string ScoringProfileName = "universityTag";

        private Index GetIndexStructure()
        {
            var index = new Index(m_IndexName, new[]
            {
                new Field(IdField,DataType.String) { IsKey = true, IsRetrievable = true},
                new Field(NameField,DataType.String) { IsSearchable = true, IsRetrievable = true},
                new Field(ImageField,DataType.String) { IsRetrievable = true},
                new Field(BoxNameField,DataType.String) { IsRetrievable = true},
                new Field(ContentField,DataType.String) { IsSearchable = true, IsRetrievable = true},
                new Field(SmallContentField,DataType.String) { IsRetrievable = true},
                new Field(UrlField,DataType.String) { IsRetrievable = true},
                new Field(UniversityNameField, DataType.String) { IsRetrievable = true},
                new Field(UniversityidField, DataType.String) { IsFilterable = true, IsRetrievable = true},
                new Field(UserIdsField, DataType.Collection(DataType.String)) { IsFilterable = true, IsRetrievable = true},
                new Field(BoxIdField, DataType.Int64) { IsRetrievable = true},
                new Field(BoxId2Field, DataType.Int64) { IsRetrievable = true , IsFilterable = true},
                new Field(ExtensionField, DataType.String) { IsRetrievable = true},
                new Field(BlobNameField, DataType.String) { IsRetrievable = true}

            });
            var scoringFunction = new TagScoringFunction("university", 2, UniversityidField);
            //var scoringFunction = new TagScoringFunction(new TagScoringParameters("university"),
            //    UniversityidField, 2);
            var scoringProfile = new ScoringProfile(ScoringProfileName)
            {
                FunctionAggregation = ScoringFunctionAggregation.Sum,

            };
            scoringProfile.Functions.Add(scoringFunction);
            index.ScoringProfiles.Add(scoringProfile);
            return index;
        }

        private async Task BuildIndex()
        {
            try
            {
                // m_Connection.SearchClient.Indexes.Delete(m_IndexName);
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on item build index", ex);
            }
            m_CheckIndexExists = true;
        }

        public async Task UpdateDataAsync(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete)
        {
            if (!m_CheckIndexExists)
            {
                //  await BuildIndex();
            }
            //var listOfCommands = new List<IndexAction<ItemSearch>>();
            if (itemToUpload != null)
            {
                var uploadBatch = itemToUpload.Select(item => new ItemSearch
                {
                    BoxId = item.BoxId,
                    BoxId2 = item.BoxId,
                    BoxName = item.BoxName,
                    Content = item.Content,
                    Extension = Path.GetExtension(item.Name),
                    Id = item.Id.ToString(CultureInfo.InvariantCulture),
                    Image = item.Image,
                    MetaContent = item.Content.RemoveEndOfString(SeachConnection.DescriptionLength),
                    Name = Path.GetFileNameWithoutExtension(item.Name),
                    UniversityId = item.UniversityId.HasValue ? item.UniversityId.ToString() : "-1",
                    UniversityName = item.UniversityName,
                    Url = item.Url,
                    UserId = item.UserIds.Select(s1 => s1.ToString(CultureInfo.InvariantCulture)).ToArray(),
                    BlobName = item.BlobName
                });
                var batch = IndexBatch.Upload(uploadBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch);
            }
            if (itemToDelete != null)
            {
                var deleteBatch = itemToDelete.Select(s =>
                     new ItemSearch
                     {
                         Id = s.ToString(CultureInfo.InvariantCulture)
                     });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch);
            }

            //var commands = listOfCommands.ToArray();
            //if (commands.Length == 0) return;

            //try
            //{
            //    await m_IndexClient.Documents.IndexAsync(IndexBatch.Create(listOfCommands.ToArray()));
            //}
            //catch (IndexBatchException ex)
            //{
            //    TraceLog.WriteError("Failed to index some of the documents: " +
            //                        String.Join(", ",
            //                            ex.IndexResponse.Results.Where(r => !r.Succeeded).Select(r => r.Key)));
            //    throw;
            //}
            //catch (CloudException ex)
            //{
            //    TraceLog.WriteError("Failed to do batch", ex);
            //    //return false;
            //    throw;
            //}

        }



        public async Task<IEnumerable<SearchItems>> SearchItemOldMobileServiceAsync(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var filter = await m_FilterProvider.BuildFilterExpressionAsync(
               query.UniversityId, UniversityidField, UserIdsField, query.UserId);

            var result = await m_IndexClient.Documents.SearchAsync<ItemSearch>(query.Term, new SearchParameters
            {
                Filter = filter,
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                ScoringProfile = ScoringProfileName,
                ScoringParameters = new[] { new ScoringParameter("university", new[] { query.UniversityId.ToString() }) },
                Select = new[] { IdField, NameField, UrlField },
            }, cancellationToken: cancelToken);

            return result.Results.Select(s => new SearchItems
            {
                Id = long.Parse(s.Document.Id),
                Url = s.Document.Url,
                Name = s.Document.Name
            }).ToList();
        }

        public async Task<IEnumerable<SearchItems>> SearchItemAsync(
            ViewModel.Queries.Search.SearchQueryMobile query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var filter = await m_FilterProvider.BuildFilterExpressionAsync(
               query.UniversityId, UniversityidField, UserIdsField, query.UserId);

            var result = await m_IndexClient.Documents.SearchAsync<ItemSearch>(query.Term, new SearchParameters
            {
                Filter = filter,
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                ScoringProfile = ScoringProfileName,
                ScoringParameters = new[] { new ScoringParameter("university", new[] { query.UniversityId.ToString() }) },
                Select = new[] { SmallContentField, IdField, NameField, BoxIdField, ExtensionField, BlobNameField },
                HighlightFields = new[] { ContentField, NameField },
            }, cancellationToken: cancelToken);

            return result.Results.Select(s => new SearchItems
            {
                Content = HighLightInField(s, ContentField, s.Document.MetaContent),
                Id = long.Parse(s.Document.Id),
                Name = HighLightInField(s, NameField, s.Document.Name),
                BoxId = s.Document.BoxId.Value,
                Extension = s.Document.Extension,
                Source = s.Document.BlobName
            }).ToList();
        }

        public async Task<IEnumerable<SearchItems>> SearchItemAsync(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var term = query.Term;
            if (!query.Term.Contains(" "))
            {
                term += "*";
            }
            var filter = await m_FilterProvider.BuildFilterExpressionAsync(
               query.UniversityId, UniversityidField, UserIdsField, query.UserId);

            var result = await m_IndexClient.Documents.SearchAsync<ItemSearch>(term, new SearchParameters
            {
                Filter = filter,
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                ScoringProfile = ScoringProfileName,
                ScoringParameters = new[] { new ScoringParameter("university", new[] { query.UniversityId.ToString() }) },
                Select = new[] { BoxNameField, SmallContentField, IdField, ImageField, NameField, UniversityNameField, UrlField, BlobNameField },
                HighlightFields = new[] { ContentField, NameField }
            }, cancellationToken: cancelToken);

            return result.Results.Select(s => new SearchItems
            {
                Boxname = s.Document.BoxName,
                Content = HighLightInField(s, ContentField, s.Document.MetaContent),
                Id = long.Parse(s.Document.Id),
                //Image = s.Document.Image,
                Name = HighLightInField(s, NameField, s.Document.Name),
                UniName = s.Document.UniversityName,
                Url = s.Document.Url,
                Source = s.Document.BlobName


            });
        }

        public async Task<IEnumerable<SearchItems>> SearchItemAsync(ViewModel.Queries.Search.SearchItemInBox query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var term = query.Term;
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }

            var result = await m_IndexClient.Documents.SearchAsync<ItemSearch>(term + "*", new SearchParameters
            {
                Filter = $"{BoxId2Field} eq {query.BoxId}",
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                Select = new[] { BoxNameField, SmallContentField, IdField, ImageField, NameField, UniversityNameField, UrlField, BlobNameField },
            }, cancellationToken: cancelToken);

            return result.Results.Select(s => new SearchItems
            {
                Boxname = s.Document.BoxName,
                Id = long.Parse(s.Document.Id),
                Name = s.Document.Name,
                Url = s.Document.Url,
                Source = s.Document.BlobName


            });
        }

        public async Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken)
        {
            try
            {
                var item =
                    await
                        m_IndexClient.Documents.GetAsync<ItemSearch>(itemId.ToString(CultureInfo.InvariantCulture),
                            new[] { ContentField }, cancellationToken: cancelToken);
                return item.Content;
            }
            //item may not exists in the search....
            catch (CloudException)
            {
                return null;
            }
        }


        private static string HighLightInField(SearchResult<ItemSearch> record, string field, string defaultValue)
        {
            if (record.Highlights == null)
            {
                return defaultValue;
            }
            IList<string> highLight;
            if (record.Highlights.TryGetValue(field, out highLight))
            {
                return String.Join("...", highLight);
            }
            return defaultValue;
        }


    }
}
