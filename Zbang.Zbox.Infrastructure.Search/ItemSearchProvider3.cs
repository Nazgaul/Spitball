using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hyak.Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using System.IO;
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

        private const string ScoringProfileName = "university";

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
            var scoringFunction = new TagScoringFunction(new TagScoringParameters(ScoringProfileName),
                UniversityidField, 2);
            var scoringProfile = new ScoringProfile("universityTag")
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

        public async Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndex();
            }
            var listOfCommands = new List<IndexAction<ItemSearch>>();
            if (itemToUpload != null)
            {
                listOfCommands.AddRange(itemToUpload.Select(item => new IndexAction<ItemSearch>(IndexActionType.MergeOrUpload, new ItemSearch
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
                })));
            }
            if (itemToDelete != null)
            {
                listOfCommands.AddRange(itemToDelete.Select(s =>
                    new IndexAction<ItemSearch>(IndexActionType.Delete, new ItemSearch
                    {
                        Id = s.ToString(CultureInfo.InvariantCulture)
                    })));
            }
            var commands = listOfCommands.ToArray();
            if (commands.Length == 0) return true;

            try
            {
                await m_IndexClient.Documents.IndexAsync(IndexBatch.Create(listOfCommands.ToArray()));
            }
            catch (IndexBatchException ex)
            {
                TraceLog.WriteError("Failed to index some of the documents: " +
                                    String.Join(", ",
                                        ex.IndexResponse.Results.Where(r => !r.Succeeded).Select(r => r.Key)));
                return false;
            }
            catch (CloudException ex)
            {
                TraceLog.WriteError("Failed to do batch", ex);
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<SearchItems>> SearchItem(
            ViewModel.Queries.Search.SearchQueryMobile query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException("query");

            var filter = await m_FilterProvider.BuildFilterExpression(
               query.UniversityId, UniversityidField, UserIdsField, query.UserId);

            var result = await m_IndexClient.Documents.SearchAsync<ItemSearch>(query.Term, new SearchParameters
            {
                Filter = filter,
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                ScoringProfile = "universityTag",
                ScoringParameters = new[] { "university:" + query.UniversityId },
                Select = new[] { SmallContentField, IdField, NameField, BoxIdField, ExtensionField },
                HighlightFields = new[] { ContentField, NameField },
            }, cancelToken);

            return result.Select(s => new SearchItems
            {
                Content = HighLightInField(s, ContentField, s.Document.MetaContent),
                Id = long.Parse(s.Document.Id),
                Name = HighLightInField(s, NameField, s.Document.Name),
                BoxId = s.Document.BoxId.Value,
                Extension = s.Document.Extension
            }).ToList();
        }

        public async Task<IEnumerable<SearchItems>> SearchItem(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException("query");
            var term = query.Term;
            if (string.IsNullOrEmpty(term))
            {
                term = "*";
            }
            var filter = await m_FilterProvider.BuildFilterExpression(
               query.UniversityId, UniversityidField, UserIdsField, query.UserId);

            //var filter = 
            //"((universityId ne '984') "
            //+ "and (universityId ne '1173')"
            //+ "and (universityId ne '19878') "
            //+ "and (universityId ne '22906') "
            //+ "and (universityId ne '64805') "
            //+ "and (not(universityId eq '-1')))"
            //+"or "
            //+ "(userId/any(t: t eq '" + query.UserId + "')  ) ";

            var result = await m_IndexClient.Documents.SearchAsync<ItemSearch>(term, new SearchParameters
            {
                Filter = filter,
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                ScoringProfile = "universityTag",
                ScoringParameters = new[] { "university:" + query.UniversityId },
                Select = new[] { BoxNameField, SmallContentField, IdField, ImageField, NameField, UniversityNameField, UrlField, BlobNameField },
                HighlightFields = new[] { ContentField, NameField }
            }, cancelToken);

            return result.Select(s => new SearchItems
            {
                Boxname = s.Document.BoxName,
                Content = HighLightInField(s, ContentField, s.Document.MetaContent),
                Id = long.Parse(s.Document.Id),
                Image = s.Document.Image,
                Name = HighLightInField(s, NameField, s.Document.Name),
                UniName = s.Document.UniversityName,
                Url = s.Document.Url,
                BlobName = s.Document.BlobName


            });
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

        //private string HighLightInName(SearchQueryRecord record)
        //{
        //    string[] highLight;
        //    if (record.Highlights.TryGetValue(NameField, out highLight))
        //    {
        //        return String.Join("...", highLight);
        //    }
        //    return SeachConnection.ConvertToType<string>(record.Properties[NameField]);
        //}
    }
}
