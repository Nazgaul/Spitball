using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class ContentSearchProvider : IContentWriteSearchProvider, IContentReadSearchProvider
    {
        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;
        private readonly string m_IndexName = "items";
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
                await BuildIndexAsync();

            }
            if (itemToUpload != null)
            {
                var uploadBatch = new Item
                {
                    Id = itemToUpload.SearchContentId,
                    Name = Path.GetFileNameWithoutExtension(itemToUpload.Name),
                    Course = itemToUpload.BoxName,
                    Professor = itemToUpload.BoxProfessor,
                    Code = itemToUpload.BoxCode,
                    University = itemToUpload.UniversityName,
                    Type = (int)itemToUpload.Type,
                    Tags = itemToUpload.Tags?.Select(s => s.Name).ToArray()
                };
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
                var batch = IndexBatch.Upload(new[] { uploadBatch });
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            }
            if (itemToDelete != null)
            {
                var deleteBatch = itemToDelete.Select(s => new ItemSearch
                {
                    Id = s.SearchContentId
                });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            }
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

        private const string ScoringProfile = "weight";
        private Index GetIndexStructure()
        {
            var definition = new Index
            {
                Name = m_IndexName,
                Fields = FieldBuilder.BuildForType<Item>(),
                Suggesters = new List<Suggester>
                {
                    new Suggester
                    {
                        Name = "sg", SourceFields = new List<string>
                        {
                            nameof(Item.Course).ToLower(),
                            nameof(Item.Code).ToLower(),
                            nameof(Item.Professor).ToLower(),
                            nameof(Item.University).ToLower(),
                            nameof(Item.Tags).ToLower()
                        }
                    }
                }
            };

            var weightProfile = new ScoringProfile(ScoringProfile);

            var d = new Dictionary<string, double>
            {
                {nameof(Item.Tags).ToLower(), 8}, {nameof(Item.Content).ToLower(), 2}, {ContentEnglishField, 4}, {ContentHebrewField, 4},
            };
            weightProfile.TextWeights = new TextWeights(d);


            var tagFunction = new TagScoringFunction
            {
                Boost = 10,
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
                Boost = 6,
                FieldName = nameof(Item.University).ToLower(),
                Parameters = new TagScoringParameters("university")
            };
            weightProfile.Functions = new List<ScoringFunction> { tagFunction, tagFunction2, tagFunction3 };


            definition.ScoringProfiles = new List<ScoringProfile> { weightProfile };


            return definition;
        }

        public async Task<IEnumerable<SearchItem>> SearchAsync(SearchAllDocuments query, CancellationToken cancelToken)
        {
            var result = await m_IndexClient.Documents.SearchAsync<Item>(query.Term, new SearchParameters()
            {
                Top = 5,
                IncludeTotalResultCount = true,
                ScoringParameters = new[] { new ScoringParameter("tag", new string[] { string.Empty }), new ScoringParameter("course", new string[] { string.Empty }), new ScoringParameter("university", new string[] { string.Empty }) },
                ScoringProfile = ScoringProfile
            }, cancellationToken: cancelToken);

            return result.Results.Select(s => new SearchItem
            {
                Id = s.Document.Id,
                Code = s.Document.Code,
                Content = s.Document.Content,
                ContentEn = s.Document.ContentEn,
                ContentHe = s.Document.ContentHe,
                Course = s.Document.Course,
                Name = s.Document.Name,
                Professor = s.Document.Professor,
                Tags = s.Document.Tags,
                Type = s.Document.Type,
                University = s.Document.University
            });

        }

        public async Task<IEnumerable<SearchItem>> SearchAsync(BaseIntent query, CancellationToken cancelToken)
        {

            var result = await m_IndexClient.Documents.SearchAsync<Item>(query.Term, new SearchParameters()
            {
                Top = 5,
                IncludeTotalResultCount = true,
                ScoringParameters = new[] { new ScoringParameter("tags", new string[] { string.Empty }), new ScoringParameter("course", new string[] { string.Empty }), new ScoringParameter("university", new string[] { string.Empty }) },
                ScoringProfile = ScoringProfile
            }, cancellationToken: cancelToken);

            return result.Results.Select(s => new SearchItem
            {
                Id = s.Document.Id,
                Code = s.Document.Code,
                Content = s.Document.Content,
                ContentEn = s.Document.ContentEn,
                ContentHe = s.Document.ContentHe,
                Course = s.Document.Course,
                Name = s.Document.Name,
                Professor = s.Document.Professor,
                Tags = s.Document.Tags,
                Type = s.Document.Type,
                University = s.Document.University
            });
        }


        //public Task<IEnumerable<SearchItem>> SearchAsync<T>(T query, CancellationToken cancelToken) where T : IIntent
        //{
        //    return null;
        //}
    }
}
