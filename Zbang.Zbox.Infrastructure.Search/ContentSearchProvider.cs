using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.SqlQueries;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class ContentSearchProvider : IContentWriteSearchProvider
    {
        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;
        private readonly string m_IndexName = "items3";
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

        public Task UpdateDataAsync(Document itemToUpload, CancellationToken token)
        {
            if (itemToUpload == null) throw new ArgumentNullException(nameof(itemToUpload));
            var batch = IndexBatch.MergeOrUpload(new[] { itemToUpload });
            return m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
        }

        public async Task UpdateDataAsync(ItemSearchDto itemToUpload, IEnumerable<ItemToDeleteSearchDto> itemToDelete, CancellationToken token)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndexAsync().ConfigureAwait(false);

            }
            if (itemToUpload != null)
            {
                var uploadBatch = new Document
                {
                    Id = itemToUpload.SearchContentId,
                    Name = itemToUpload.Name?.ToLowerInvariant(),
                    Course = itemToUpload.Course.ToString(),
                    UniversityId = itemToUpload.University.Id.ToString(),
                    University = itemToUpload.University.Name, //  JsonConvert.SerializeObject(itemToUpload.University).ToLowerInvariant(),
                    Tags = itemToUpload.Tags?.Select(s => s.Name.ToLowerInvariant()).Distinct().ToArray(),
                    Date = itemToUpload.Date.Truncate(TimeSpan.FromSeconds(1)),
                    MetaContent = itemToUpload.MetaContent,
                    Source = itemToUpload.BlobName,
                    Views = itemToUpload.Views,
                    Likes = itemToUpload.Likes,
                };
                if (!string.IsNullOrEmpty(itemToUpload.Content))
                {
                    var content = itemToUpload.Content.Trim();
                    switch (itemToUpload.Language)
                    {
                        case Language.Undefined:
                            uploadBatch.Content = content;
                            break;
                        case Language.EnglishUs:
                            uploadBatch.ContentEn = content;
                            uploadBatch.Content = string.Empty;
                            break;
                        case Language.Hebrew:
                            uploadBatch.ContentHe = content;
                            break;
                        case null:
                            uploadBatch.Content = content;
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
            await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure()).ConfigureAwait(false);
            m_CheckIndexExists = true;

        }

        private const string ScoringProfile = "score";
        private Index GetIndexStructure()
        {
            var definition = new Index
            {
                Name = m_IndexName,
                Fields = FieldBuilder.BuildForType<Document>()
            };

            var weightProfile = new ScoringProfile(ScoringProfile);

            var d = new Dictionary<string, double>
            {
                { nameof(Document.Tags).ToLower(), 3},
                { nameof(Document.Name).ToLower(), 4},
                { ContentEnglishField, 2},
                { ContentHebrewField, 2},
            };
            weightProfile.TextWeights = new TextWeights(d);


            var tagFunction = new TagScoringFunction
            {
                Boost = 8,
                FieldName = nameof(Document.Tags).ToLower(),
                Parameters = new TagScoringParameters("tag")
            };
            var tagFunction2 = new TagScoringFunction
            {
                Boost = 10,
                FieldName = "courseId",
                Parameters = new TagScoringParameters("course")
            };
            var tagFunction3 = new TagScoringFunction
            {
                Boost = 5,
                FieldName = "universityId",
                Parameters = new TagScoringParameters("university")
            };
            var freshnessFunction = new FreshnessScoringFunction
            {
                Boost = 5,
                FieldName = nameof(Document.Date).ToLower(),
                Interpolation = ScoringFunctionInterpolation.Quadratic,
                Parameters = new FreshnessScoringParameters(TimeSpan.FromDays(100))
            };
            var likesScore = new MagnitudeScoringFunction
            {
                Boost = 4,
                FieldName = nameof(Document.Likes).ToLower(),
                Parameters = new MagnitudeScoringParameters
                {
                    BoostingRangeStart = 1,
                    BoostingRangeEnd = int.MaxValue,
                    ShouldBoostBeyondRangeByConstant = false
                }
            };
            var viewsScore = new MagnitudeScoringFunction
            {
                Boost = 5,
                FieldName = nameof(Document.Views).ToLower(),
                Parameters = new MagnitudeScoringParameters
                {
                    BoostingRangeStart = 1,
                    BoostingRangeEnd = int.MaxValue,
                    ShouldBoostBeyondRangeByConstant = false
                }
            };
            weightProfile.Functions = new List<ScoringFunction> { tagFunction, tagFunction2, tagFunction3, freshnessFunction, likesScore, viewsScore };
            definition.ScoringProfiles = new List<ScoringProfile> { weightProfile };

            return definition;
        }


    }
}
