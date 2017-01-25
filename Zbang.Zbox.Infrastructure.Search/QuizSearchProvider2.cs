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
using Zbang.Zbox.ViewModel.Dto.Search;
using Index = Microsoft.Azure.Search.Models.Index;
using ScoringProfile = Microsoft.Azure.Search.Models.ScoringProfile;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class QuizSearchProvider2 : IQuizReadSearchProvider2, IQuizWriteSearchProvider2
    {
        private readonly string m_IndexName = "quiz2";
        private readonly ISearchFilterProvider m_FilterProvider;
        private readonly ISearchConnection m_Connection;
        private bool m_CheckIndexExists;
        private readonly ISearchIndexClient m_IndexClient;

        public QuizSearchProvider2(ISearchFilterProvider filterProvider, ISearchConnection connection)
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
        private const string BoxNameField = "boxName";

        private const string QuestionsField = "questions";
        private const string AnswersField = "answers";
        private const string ContentField = "metaContent";

        private const string UrlField = "url";
        private const string UniversityNameField = "universityName";
        private const string UniversityidField = "universityId";
        private const string UseridsField = "userId";
        private const string BoxIdField = "boxId";

        private const string ScoringProfileName = "university";

        private Index GetIndexStructure()
        {
            var index = new Index(m_IndexName, new[]
            {
                new Field(IdField, DataType.String) { IsKey = true, IsRetrievable = true},
                new Field(NameField, DataType.String) { IsSearchable = true, IsRetrievable = true},
                new Field(BoxNameField, DataType.String) { IsRetrievable = true},
                new Field(UrlField, DataType.String) { IsRetrievable = true},
                new Field(UniversityNameField, DataType.String) { IsRetrievable = true},
                new Field(UniversityidField, DataType.String) { IsRetrievable = true, IsFilterable = true},
                new Field(UseridsField, DataType.Collection(DataType.String)) { IsFilterable = true, IsRetrievable = true} ,
                new Field(QuestionsField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},
                new Field(AnswersField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},
                new Field(ContentField, DataType.String) { IsRetrievable = true},
                new Field(BoxIdField, DataType.Int64) { IsRetrievable = true}

            });
            var scoringFunction = new TagScoringFunction(UniversityidField, 2, ScoringProfileName);
            //UniversityidField, 2);
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
                // m_Connection.SearchClient.Indexes.Delete(m_IndexName);
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on quiz build index", ex);
            }
            m_CheckIndexExists = true;
        }

        public async Task<bool> UpdateDataAsync(IEnumerable<QuizSearchDto> quizToUpload, IEnumerable<long> quizToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndexAsync();
            }
            //var listOfCommands = new List<IndexAction<QuizSearch>>();

            if (quizToUpload != null)
            {

                var uploadBatch = quizToUpload.Select(s => new QuizSearch
                {
                    Answers = s.Answers.ToArray(),
                    BoxId = s.BoxId,
                    BoxName = s.BoxName,
                    Id = s.Id.ToString(CultureInfo.InvariantCulture),
                    MetaContent = TextManipulation.RemoveHtmlTags.Replace(string.Join(" ", s.Questions), string.Empty).RemoveEndOfString(SeachConnection.DescriptionLength),
                    Name = s.Name,
                    Questions = s.Questions.ToArray(),
                    UniversityId = s.UniversityId.HasValue ? s.UniversityId.ToString() : "-1",
                    UniversityName = s.UniversityName,
                    Url = s.Url,
                    UserId = s.UserIds.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()
                });
                var batch = IndexBatch.Upload(uploadBatch);
                if (batch.Actions.Any())
                {
                    await m_IndexClient.Documents.IndexAsync(batch);
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
                    await m_IndexClient.Documents.IndexAsync(batch);
            }
            return true;

        }

        public async Task<IEnumerable<SearchQuizzes>> SearchQuizAsync(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var filter = await m_FilterProvider.BuildFilterExpressionAsync(
              query.UniversityId, UniversityidField, UseridsField, query.UserId);

            //if we put asterisk highlight is not working
            var result = await m_IndexClient.Documents.SearchAsync<QuizSearch>(query.Term, new SearchParameters
            {
                Filter = filter,
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                ScoringProfile = "universityTag",
                ScoringParameters = new[] { new ScoringParameter("university", new[] { query.UniversityId.ToString() }) },
                HighlightFields = new[] { QuestionsField, AnswersField, NameField },
                Select = new[] { NameField, IdField, BoxNameField, UniversityNameField, UrlField, ContentField }
            }, cancellationToken: cancelToken);

            return result.Results.Select(s => new SearchQuizzes
            {
                Boxname = s.Document.BoxName,
                Content = HighLightInField(s, new[] { QuestionsField, AnswersField }, s.Document.MetaContent),
                Id = long.Parse(s.Document.Id),
                Name = HighLightInField(s, new[] { NameField }, s.Document.Name),
                UniName = s.Document.UniversityName,
                Url = s.Document.Url
            });

        }
        private static string HighLightInField(SearchResult<QuizSearch> record, IEnumerable<string> fields, string defaultValue)
        {
            if (record.Highlights == null)
            {
                return defaultValue;
            }
            foreach (var field in fields)
            {
                IList<string> highLight;
                if (record.Highlights.TryGetValue(field, out highLight))
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


    public interface IQuizReadSearchProvider2
    {
        Task<IEnumerable<SearchQuizzes>> SearchQuizAsync(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
    }
}
