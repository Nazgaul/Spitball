using System;
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
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class ItemSearchProvider3 : IItemReadSearchProvider, IItemWriteSearchProvider
    {
        private readonly string m_IndexName = "item3";
#pragma warning disable CS0169 // The field 'ItemSearchProvider3.m_CheckIndexExists' is never used
        private bool m_CheckIndexExists;
#pragma warning restore CS0169 // The field 'ItemSearchProvider3.m_CheckIndexExists' is never used
        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;
        private readonly ISearchFilterProvider m_FilterProvider;

        private object lockObj;
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
            var scoringFunction = new TagScoringFunction(UniversityidField, 2, "university");
            var scoringProfile = new ScoringProfile(ScoringProfileName)
            {
                FunctionAggregation = ScoringFunctionAggregation.Sum,
                Functions = new List<ScoringFunction>
                {
                    scoringFunction
                }
            };
            index.ScoringProfiles = new List<ScoringProfile> { scoringProfile };
            return index;
        }

        private async Task BuildIndexAsync()
        {
            try
            {
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on item build index", ex);
            }
            m_CheckIndexExists = true;
        }

        public async Task UpdateDataAsync(ItemSearchDto itemToUpload, IEnumerable<long> itemToDelete, CancellationToken token)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndexAsync();

            }
            if (itemToUpload != null)
            {
                var uploadBatch = new ItemSearch
                {
                    BoxId = itemToUpload.BoxId,
                    BoxId2 = itemToUpload.BoxId,
                    BoxName = itemToUpload.BoxName,
                    Content = itemToUpload.Content,
                    Extension = Path.GetExtension(itemToUpload.Name),
                    Id = itemToUpload.Id.ToString(CultureInfo.InvariantCulture),
                    Image = itemToUpload.Image,
                    MetaContent = itemToUpload.Content.RemoveEndOfString(SeachConnection.DescriptionLength),
                    Name = Path.GetFileNameWithoutExtension(itemToUpload.Name),
                    UniversityId = itemToUpload.UniversityId.HasValue ? itemToUpload.UniversityId.ToString() : "-1",
                    UniversityName = itemToUpload.UniversityName,
                    Url = itemToUpload.Url,
                    UserId = itemToUpload.UserIds.Select(s1 => s1.ToString(CultureInfo.InvariantCulture)).ToArray(),
                    BlobName = itemToUpload.BlobName
                };
                var batch = IndexBatch.MergeOrUpload(new[] { uploadBatch });
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
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
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            }


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
            //if we put asterisk highlight is not working
            //http://stackoverflow.com/questions/35769442/azure-search-highlights-not-returned-for-every-result/35778095
            //if (!query.Term.Contains(" "))
            //{
            //    term += "*";
            //}
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
