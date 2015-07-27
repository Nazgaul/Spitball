using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hyak.Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Search;
using Index = Microsoft.Azure.Search.Models.Index;
using ScoringProfile = Microsoft.Azure.Search.Models.ScoringProfile;

namespace Zbang.Zbox.Infrastructure.Search
{
    class QuizSearchProvider2 : IQuizReadSearchProvider2, IQuizWriteSearchProvider2
    {
        private readonly string m_IndexName = "quiz2";
        private readonly ISearchFilterProvider m_FilterProvider;
        private readonly ISearchConnection m_Connection;
        private bool m_CheckIndexExists;
        private readonly SearchIndexClient m_IndexClient;

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
                TraceLog.WriteError("on box build index", ex);
            }
            m_CheckIndexExists = true;
        }

        public async Task<bool> UpdateData(IEnumerable<QuizSearchDto> quizToUpload, IEnumerable<long> quizToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndex();
            }
            var listOfCommands = new List<IndexAction<QuizSearch>>();

            if (quizToUpload != null)
            {
                listOfCommands.AddRange(quizToUpload.Select(s => new IndexAction<QuizSearch>(IndexActionType.MergeOrUpload, new QuizSearch
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
                })));
            }
            if (quizToDelete != null)
            {
                listOfCommands.AddRange(quizToDelete.Select(s =>
                    new IndexAction<QuizSearch>(IndexActionType.Delete, new QuizSearch
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

        public async Task<IEnumerable<SearchQuizzes>> SearchQuiz(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException("query");
            var filter = await m_FilterProvider.BuildFilterExpression(
              query.UniversityId, UniversityidField, UseridsField, query.UserId);

            var term = query.Term;
            if (string.IsNullOrEmpty(term))
            {
                term = "*";
            }
            var result = await m_IndexClient.Documents.SearchAsync<QuizSearch>(term, new SearchParameters
            {
                Filter = filter,
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                ScoringProfile = "universityTag",
                ScoringParameters = new[] { "university:" + query.UniversityId },
                HighlightFields = new[] { QuestionsField, AnswersField, NameField },
                Select = new[] { NameField, IdField, BoxNameField, UniversityNameField, UrlField, ContentField }
            });

            return result.Select(s => new SearchQuizzes
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
                    return String.Join("...", highLight);
                }
            }
            return defaultValue;
        }
    }
}
