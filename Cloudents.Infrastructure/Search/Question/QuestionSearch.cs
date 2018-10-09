using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search.Question
{
    public class QuestionSearch : IQuestionSearch
    {
        private readonly AzureQuestionSearch _questionSearch;
        private readonly IQueryBus _queryBus;

        public QuestionSearch(AzureQuestionSearch questionSearch, IQueryBus queryBus)
        {
            _questionSearch = questionSearch;
            _queryBus = queryBus;
        }

        public async Task<QuestionWithFacetDto> SearchAsync(QuestionsQuery query, CancellationToken token)
        {
            var taskResult = _questionSearch.SearchAsync(query, token);
            var querySubject = new QuestionSubjectQuery();
            var taskSubjects = _queryBus.QueryAsync(querySubject, token);

            await Task.WhenAll(taskResult, taskSubjects);
            var result = taskResult.Result;
            var subject = taskSubjects.Result.ToDictionary(x => x.Id, x => x.Subject);


            var retVal = new QuestionWithFacetDto
            {
                Result = result.Results.Select<SearchResult<Core.Entities.Search.Question>, QuestionDto>(s => new QuestionDto()
                {

                    User = new UserDto
                    {
                        Id = s.Document.UserId,
                        Name = s.Document.UserName,
                        Image = s.Document.UserImage
                    },
                    Id = long.Parse(s.Document.Id),
                    DateTime = s.Document.DateTime,
                    Answers = s.Document.AnswerCount,
                    Subject = subject[s.Document.Subject],
                    Color = s.Document.Color,
                    Files = s.Document.FilesCount,
                    HasCorrectAnswer = s.Document.HasCorrectAnswer,
                    Price = (decimal)s.Document.Price,
                    Text = s.Document.Text
                })
                // Result = _mapper.Map<IEnumerable<QuestionDto>>(result.Results.Select(s => s.Document))
            };
            if (result.Facets.TryGetValue(nameof(Core.Entities.Search.Question.Subject), out var p))
            {

                retVal.FacetSubject = p.Select(s => subject[(int)s.AsValueFacetResult<long>().Value]);

                //retVal.Facet = p.Select(s => s.AsValueFacetResult<string>().Value);
            }

            if (result.Facets.TryGetValue(nameof(Core.Entities.Search.Question.State), out var p2))
            {
                //var t = p2.Select(s =>
                //{
                //    return s.AsValueFacetResult<long>().Value;
                //});
                retVal.FacetState = p2.Select(s => (QuestionFilter)s.AsValueFacetResult<long>().Value);
                //
                // retVal.Facets[nameof(Question.State)] = p2.Select(s => s.AsValueFacetResult<string>().Value);

            }
            return retVal;
        }
    }
}