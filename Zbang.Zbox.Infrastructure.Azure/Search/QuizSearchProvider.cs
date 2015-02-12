using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class QuizSearchProvider : IQuizWriteSearchProvider
    {
        private readonly string m_IndexName = "quiz";
        private bool m_CheckIndexExists;

        public QuizSearchProvider()
        {
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
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringCollectionField(AnswersField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(UrlField, f => f
                    .IsRetrievable())
                .WithStringField(UniversityNameField, f => f
                    .IsRetrievable())
                .WithField(UniversityidField, "Edm.Int64", f => f
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
            var response = await SeachConnection.Instance.IndexManagement.GetIndexAsync(m_IndexName);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await SeachConnection.Instance.IndexManagement.CreateIndexAsync(CreateIndex());
            }
            m_CheckIndexExists = true;
        }

        public async Task<bool> UpdateData(IEnumerable<QuizSearchDto> quizToUpload, IEnumerable<long> itemToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await SeachConnection.Instance.IndexManagement.DeleteIndexAsync(m_IndexName);
                await BuildIndex();
            }
            var listOfCommands = new List<IndexOperation>();
            if (quizToUpload != null)
            {
                foreach (var quiz in quizToUpload)
                {
                    listOfCommands.Add(
                        new IndexOperation(IndexOperationType.Upload, IdField,
                            quiz.Id.ToString(CultureInfo.InvariantCulture))
                            .WithProperty(NameField, quiz.Name)
                            .WithProperty(BoxNameField, quiz.BoxName)
                            .WithProperty(UniversityNameField, quiz.UniversityName)
                            .WithProperty(QuestionsField, quiz.Questions)
                            .WithProperty(AnswersField, quiz.Answers)
                            .WithProperty(UrlField, quiz.Url)
                            .WithProperty(UniversityidField, quiz.UniversityId)
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
    }

    public interface IQuizWriteSearchProvider
    {
        Task<bool> UpdateData(IEnumerable<QuizSearchDto> quizToUpload, IEnumerable<long> itemToDelete);
    }
}
