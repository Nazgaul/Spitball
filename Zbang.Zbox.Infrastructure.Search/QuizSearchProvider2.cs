using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Index = Microsoft.Azure.Search.Models.Index;
using ScoringProfile = Microsoft.Azure.Search.Models.ScoringProfile;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class QuizSearchProvider2 : IQuizWriteSearchProvider2
    {
        private readonly string _indexName = "quiz2";
        private readonly ISearchConnection _connection;
        private bool _checkIndexExists;
        private readonly ISearchIndexClient _indexClient;
        private readonly ILogger _logger;

        public QuizSearchProvider2(ISearchConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
            if (_connection.IsDevelop)
            {
                _indexName = _indexName + "-dev";
            }
            _indexClient = connection.SearchClient.Indexes.GetClient(_indexName);
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string BoxNameField = "boxName";

        private const string QuestionsField = "questions";
        private const string AnswersField = "answers";
        private const string ContentField = "metaContent";

        private const string UrlField = "url";
        private const string UniversityNameField = "universityName";
        private const string UniversityIdField = "universityId";
        private const string UserIdsField = "userId";
        private const string BoxIdField = "boxId";

        private const string ScoringProfileName = "university";

        private Index GetIndexStructure()
        {
            var index = new Index(_indexName, new[]
            {
                new Field(IdField, DataType.String) { IsKey = true, IsRetrievable = true},
                new Field(NameField, DataType.String) { IsSearchable = true, IsRetrievable = true},
                new Field(BoxNameField, DataType.String) { IsRetrievable = true},
                new Field(UrlField, DataType.String) { IsRetrievable = true},
                new Field(UniversityNameField, DataType.String) { IsRetrievable = true},
                new Field(UniversityIdField, DataType.String) { IsRetrievable = true, IsFilterable = true},
                new Field(UserIdsField, DataType.Collection(DataType.String)) { IsFilterable = true, IsRetrievable = true} ,
                new Field(QuestionsField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},
                new Field(AnswersField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},
                new Field(ContentField, DataType.String) { IsRetrievable = true},
                new Field(BoxIdField, DataType.Int64) { IsRetrievable = true}

            });
            var scoringFunction = new TagScoringFunction(UniversityIdField, 2, ScoringProfileName);
            var scoringProfile = new ScoringProfile("universityTag")
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
                // _connection.SearchClient.Indexes.Delete(_indexName);
                await _connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
            }
            _checkIndexExists = true;
        }

        public async Task<bool> UpdateDataAsync(IEnumerable<QuizSearchDto> quizToUpload, IEnumerable<long> quizToDelete)
        {
            if (!_checkIndexExists)
            {
                await BuildIndexAsync().ConfigureAwait(false);
            }

            if (quizToUpload != null)
            {
                var uploadBatch = quizToUpload.Select(s => new QuizSearch
                {
                    Answers = s.Answers.ToArray(),
                    BoxId = s.Course.Id,
                    BoxName = s.Course.Name,
                    Id = s.Id.ToString(CultureInfo.InvariantCulture),
                    MetaContent = TextManipulation.RemoveHtmlTags.Replace(string.Join(" ", s.Questions), string.Empty).RemoveEndOfString(SearchConnection.DescriptionLength),
                    Name = s.Name,
                    Questions = s.Questions.ToArray(),
                    UniversityId = s.University?.Id.ToString(), //.HasValue ? s.UniversityId.ToString() : "-1",
                    UniversityName = s.University?.Name,
                    Url = s.Url,
                    UserId = s.UserIds.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()
                });
                var batch = IndexBatch.Upload(uploadBatch);
                if (batch.Actions.Any())
                {
                    await _indexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
                }
            }
            if (quizToDelete != null)
            {
                var deleteBatch = quizToDelete.Select(s =>
                     new QuizSearch
                     {
                         Id = s.ToString(CultureInfo.InvariantCulture)
                     });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                    await _indexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
            }
            return true;
        }

        private static string HighLightInField(SearchResult<QuizSearch> record, IEnumerable<string> fields, string defaultValue)
        {
            if (record.Highlights == null)
            {
                return defaultValue;
            }
            foreach (var field in fields)
            {
                if (record.Highlights.TryGetValue(field, out IList<string> highLight))
                {
                    return string.Join("...", highLight);
                }
            }
            return defaultValue;
        }
    }

    public interface IQuizWriteSearchProvider2
    {
        Task<bool> UpdateDataAsync(IEnumerable<QuizSearchDto> quizToUpload, IEnumerable<long> itemToDelete);
    }
}
