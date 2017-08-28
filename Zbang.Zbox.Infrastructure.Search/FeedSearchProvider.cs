using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Qna;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class FeedSearchProvider : IFeedWriteSearchProvider, Autofac.IStartable
    {
        internal const string ContentEnglishField = "text_en";
        internal const string ContentHebrewField = "text_he";
        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;
        private readonly ILogger m_Logger;
        private readonly string m_IndexName = "feed";
        //private bool m_CheckIndexExists;
        private const string ScoringProfile = "score";

        public FeedSearchProvider(ISearchConnection connection, ILogger logger)
        {
            m_Connection = connection;
            m_Logger = logger;
            if (m_Connection.IsDevelop)
            {
                m_IndexName = m_IndexName + "-dev";
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        public async Task UpdateDataAsync(FeedSearchDto itemToUpload, IEnumerable<FeedSearchDeleteDto> itemToDelete, CancellationToken token)
        {
            //if (!m_CheckIndexExists)
            //{
            //    await BuildIndexAsync().ConfigureAwait(false);

            //}
            if (itemToUpload != null)
            {
                var uploadBatch = new FeedSearch
                {
                    Id = itemToUpload.Id.ToString(),
                    Course = JsonConvert.SerializeObject(itemToUpload.Course).ToLowerInvariant(),
                    CourseSearch = itemToUpload.Course.ToString(),
                    CourseId = itemToUpload.Course.Id.ToString(),
                    UniversityId = itemToUpload.University.Id.ToString(),
                    University = JsonConvert.SerializeObject(itemToUpload.University).ToLowerInvariant(),
                    Tags = itemToUpload.Tags?.Select(s => s.Name.ToLowerInvariant()).Distinct().ToArray(),
                    Date = itemToUpload.Date.Truncate(TimeSpan.FromSeconds(1)),
                    Likes = itemToUpload.LikeCount,
                    ReplyCount = itemToUpload.ReplyCount,
                    ItemCount = itemToUpload.ItemCount,
                    Comment = itemToUpload.Text,
                    UserName = itemToUpload.UserName,
                    UserImage = itemToUpload.UserImage,
                };
                if (!string.IsNullOrEmpty(itemToUpload.Content))
                {
                    switch (itemToUpload.Language)
                    {
                        case Language.Undefined:
                            uploadBatch.Text = itemToUpload.Content;
                            break;
                        case Language.EnglishUs:
                            uploadBatch.TextEn = itemToUpload.Content;
                            break;
                        case Language.Hebrew:
                            uploadBatch.TextHe = itemToUpload.Content;
                            break;
                        case null:
                            uploadBatch.Text = itemToUpload.Content;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                var batch = IndexBatch.MergeOrUpload(new[] { uploadBatch });
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token).ConfigureAwait(false);
            }

            var t = itemToDelete?.ToList();
            if (t?.Count > 0)
            {
                var deleteBatch = t.Select(s => new FeedSearch
                {
                    Id = s.Id.ToString()
                });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token).ConfigureAwait(false);
            }
        }

        //private async Task BuildIndexAsync()
        //{
        //    try
        //    {
        //        //m_Connection.SearchClient.Indexes.CreateOrUpdate(GetIndexStructure());
        //        await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure()).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("on item build index", ex);
        //    }
        //    m_CheckIndexExists = true;
        //}

        private Index GetIndexStructure()
        {
            var definition = new Index
            {
                Name = m_IndexName,
                Fields = FieldBuilder.BuildForType<FeedSearch>()
            };

            var weightProfile = new ScoringProfile(ScoringProfile);

            var d = new Dictionary<string, double>
            {
                [nameof(FeedSearch.Tags).ToLower()] = 3,
                [ContentEnglishField] = 2,
                [ContentHebrewField] = 2,
            };
            weightProfile.TextWeights = new TextWeights(d);


            var tagFunction = new TagScoringFunction
            {
                Boost = 8,
                FieldName = nameof(FeedSearch.Tags).ToLower(),
                Parameters = new TagScoringParameters("tag")
            };
            var tagFunction2 = new TagScoringFunction
            {
                Boost = 10,
                FieldName = nameof(FeedSearch.Course).ToLower(),
                Parameters = new TagScoringParameters("course")
            };
            var tagFunction3 = new TagScoringFunction
            {
                Boost = 5,
                FieldName = nameof(FeedSearch.University).ToLower(),
                Parameters = new TagScoringParameters("university")
            };
            var freshnessFunction = new FreshnessScoringFunction
            {
                Boost = 5,
                FieldName = nameof(FeedSearch.Date).ToLower(),
                Interpolation = ScoringFunctionInterpolation.Quadratic,
                Parameters = new FreshnessScoringParameters(TimeSpan.FromDays(100))
            };
            var likesScore = new MagnitudeScoringFunction
            {
                Boost = 4,
                FieldName = nameof(FeedSearch.Likes).ToLower(),
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
                FieldName = nameof(FeedSearch.Likes).ToLower(),
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

        public void Start()
        {
            try
            {
                m_Connection.SearchClient.Indexes.CreateOrUpdate(GetIndexStructure());
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
            }
}
    }
}
