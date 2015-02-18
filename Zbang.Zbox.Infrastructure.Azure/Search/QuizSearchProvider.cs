using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class QuizSearchProvider : IQuizWriteSearchProvider, IQuizReadSearchProvider
    {
        private readonly string m_IndexName = "quiz";
        private readonly ISearchFilterProvider m_FilterProvider;
        private bool m_CheckIndexExists;

        public QuizSearchProvider(ISearchFilterProvider filterProvider)
        {
            m_FilterProvider = filterProvider;
            if (!RoleEnvironment.IsAvailable)
            {
                m_IndexName = m_IndexName + "-dev";
                return;
            }
            if (RoleEnvironment.IsEmulated)
            {
                m_IndexName = m_IndexName + "-dev";
            }
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string BoxNameField = "boxname";

        private const string QuestionsField = "questions";
        private const string AnswersField = "answer";
        private const string ContentField = "description";

        private const string UrlField = "url";
        private const string UniversityNameField = "universityname";
        private const string UniversityidField = "unidersityid";
        private const string UseridsField = "userids";



        private Index CreateIndex()
        {
            var index = new Index(m_IndexName)
                .WithStringField(IdField, f => f
                    .IsKey()
                    .IsRetrievable()
                )
                .WithStringField(NameField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(BoxNameField, f => f
                    .IsRetrievable())
                .WithStringCollectionField(QuestionsField, f => f
                    .IsSearchable())
                .WithStringCollectionField(AnswersField, f => f
                    .IsSearchable())
                .WithStringField(ContentField, f => f
                    .IsRetrievable())
                .WithStringField(UrlField, f => f
                    .IsRetrievable())
                .WithStringField(UniversityNameField, f => f
                    .IsRetrievable())
                .WithStringField(UniversityidField, f => f
                    .IsFilterable())
                .WithStringCollectionField(UseridsField, f => f
                    .IsFilterable());

            //var universityScoreProfile = new ScoringProfile
            //{
            //    Name = "university"

            //};
            //universityScoreProfile.Functions.Add(new ScoringProfileFunction
            //{
            //    Type = ScoringProfileFunctionType.
            //})
            //index.ScoringProfiles.Add(universityScoreProfile);
            return index;

        }

        private async Task BuildIndex()
        {
            try
            {
                var response = await SeachConnection.Instance.IndexManagement.GetIndexAsync(m_IndexName);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    await SeachConnection.Instance.IndexManagement.CreateIndexAsync(CreateIndex());

                }
                else
                {
                    await SeachConnection.Instance.IndexManagement.UpdateIndexAsync(CreateIndex());
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on quiz build index", ex);
            }
            m_CheckIndexExists = true;
        }

        public async Task<bool> UpdateData(IEnumerable<QuizSearchDto> quizToUpload, IEnumerable<long> itemToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndex();
            }
            var listOfCommands = new List<IndexOperation>();
            if (quizToUpload != null)
            {
                foreach (var quiz in quizToUpload)
                {
                    var content = TextManipulation.RemoveHtmlTags.Replace(string.Join(" ", quiz.Questions), string.Empty).RemoveEndOfString(SeachConnection.DescriptionLength);
                    listOfCommands.Add(
                        new IndexOperation(IndexOperationType.Upload, IdField,
                            quiz.Id.ToString(CultureInfo.InvariantCulture))
                            .WithProperty(NameField, quiz.Name)
                            .WithProperty(BoxNameField, quiz.BoxName)
                            .WithProperty(UniversityNameField, quiz.UniversityName)
                            .WithProperty(QuestionsField, quiz.Questions)
                            .WithProperty(AnswersField, quiz.Answers)
                            .WithProperty(ContentField, content)
                            .WithProperty(UrlField, quiz.Url)
                            .WithProperty(UniversityidField, quiz.UniversityId.ToString())
                            .WithProperty(UseridsField,
                                quiz.UserIds.Select(s1 => s1.ToString(CultureInfo.InvariantCulture))));
                }

            }
            if (itemToDelete != null)
            {
                listOfCommands.AddRange(itemToDelete.Select(s =>
                    new IndexOperation(IndexOperationType.Delete, IdField, s.ToString(CultureInfo.InvariantCulture))
                    ));
            }
            var commands = listOfCommands.ToArray();
            if (commands.Length > 0)
            {

                var retVal = await SeachConnection.Instance.IndexManagement.PopulateAsync(m_IndexName, listOfCommands.ToArray());
                if (!retVal.IsSuccess)
                {
                    TraceLog.WriteError("On update search" + retVal.Error.Message);
                }
                return retVal.IsSuccess;
            }
            return true;
        }

        public async Task<IEnumerable<SearchQuizzes>> SearchQuiz(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            

            var searchResult = await SeachConnection.Instance.IndexQuery.SearchAsync(m_IndexName,
                new SearchQuery(query.Term + "*")
                {
                    Filter = await m_FilterProvider.BuildFilterExpression(
                        query.UniversityId, UniversityidField, UseridsField, query.UserId),
                    ScoringProfile = "university",
                    ScoringParameters = new[] { "university:" + query.UniversityId },
                    Top = query.RowsPerPage,
                    Skip = query.RowsPerPage * query.PageNumber,
                    Highlight = QuestionsField + "," + AnswersField,

                }, cancelToken);

            if (!searchResult.IsSuccess)
            {
                TraceLog.WriteError(string.Format("on quiz search model: {0} error: {1}", query,
                    searchResult.Error.Message));
                return null;
            }
            if (searchResult.Body.Records.Any())
            {
                return searchResult.Body.Records.Select(s => new SearchQuizzes(
                    SeachConnection.ConvertToType<string>(s.Properties[NameField]),
                    SeachConnection.ConvertToType<long>(s.Properties[IdField]),
                    ConvertHighlightToProperty(s),
                    SeachConnection.ConvertToType<string>(s.Properties[BoxNameField]),
                    SeachConnection.ConvertToType<string>(s.Properties[UniversityNameField]),
                    SeachConnection.ConvertToType<string>(s.Properties[UrlField])
                    ));
            }
            return null;



        }



        private string HighLightInName(SearchQueryRecord record)
        {
            string[] highLight;
            if (record.Highlights.TryGetValue(NameField, out highLight))
            {
                return String.Join("...", highLight);
            }
            return SeachConnection.ConvertToType<string>(record.Properties[NameField]);
        }
        private string ConvertHighlightToProperty(SearchQueryRecord record)
        {
            string[] questionHighLight;
            string[] answerHighLight;

            if (!record.Highlights.TryGetValue(QuestionsField, out questionHighLight))
            {
                questionHighLight = new string[0];
            }
            if (!record.Highlights.TryGetValue(AnswersField, out answerHighLight))
            {
                answerHighLight = new string[0];
            }
            var str = SeachConnection.LimitContentHighlight(questionHighLight.Union(answerHighLight));
            return string.IsNullOrEmpty(str) ?
               SeachConnection.ConvertToType<string>(record.Properties[ContentField]) : str;
        }
    }

    public interface IQuizWriteSearchProvider
    {
        Task<bool> UpdateData(IEnumerable<QuizSearchDto> quizToUpload, IEnumerable<long> itemToDelete);
    }

    public interface IQuizReadSearchProvider
    {
        Task<IEnumerable<SearchQuizzes>> SearchQuiz(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
    }
}
