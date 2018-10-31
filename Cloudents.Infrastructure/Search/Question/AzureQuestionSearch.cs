using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Search.Question
{
    public class AzureQuestionSearch : IQuestionSearch
    {
        private readonly ISearchIndexClient _client;

        public AzureQuestionSearch(ISearchService client)
        {
            _client = client.GetClient(QuestionSearchWrite.IndexName);
        }

        public async Task<QuestionWithFacetDto> SearchAsync(QuestionsQuery query, CancellationToken token)
        {
            var filters = new List<string>
            {
                $"{nameof(Core.Entities.Search.Question.Country)} eq '{query.Country}' or {nameof(Core.Entities.Search.Question.Language)} eq 'en'"
            };
            if (query.Source != null)
            {
                var filterStr = string.Join(" or ", query.Source.Select(s =>
                    $"{nameof(Core.Entities.Search.Question.Subject)} eq {(int)s}"));

                filters.Add($"({filterStr})");
            }

            if (query.Filters != null)
            {
                var filterStr = string.Join(" or ", query.Filters.Select(s =>
                     $"{nameof(Core.Entities.Search.Question.State)} eq {(int)s}"));
                filters.Add($"({filterStr})");
            }
            var searchParameter = new SearchParameters
            {
                Facets = new[] { nameof(Core.Entities.Search.Question.Subject), nameof(Core.Entities.Search.Question.State) },
                Filter = string.Join(" and ", filters),
                Top = 50,
                Skip = query.Page * 50,
                ScoringProfile = QuestionSearchWrite.ScoringProfile,
                ScoringParameters = new[]
                             {
                    new ScoringParameter
                    (QuestionSearchWrite.TagsCountryParameter
                        , new[] {query.Country}),

                    //new ScoringParameter
                    //(QuestionSearchWrite.TagsUniversityParameter
                    //    , new[] {query.UniversityId}),

                    //new ScoringParameter
                    //(QuestionSearchWrite.TagsLanguageParameter
                    //    , t)
                }

            };

            var result = await
                _client.Documents.SearchAsync<Core.Entities.Search.Question>(query.Term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);


            var retVal = new QuestionWithFacetDto
            {
                Result = result.Results.Select(s => new QuestionDto()
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
                    Subject = s.Document.Subject,
                    Color = s.Document.Color,
                    Files = s.Document.FilesCount,
                    HasCorrectAnswer = s.Document.HasCorrectAnswer,
                    Price = (decimal)s.Document.Price,
                    Text = s.Document.Text
                })
            };
            if (result.Facets.TryGetValue(nameof(Core.Entities.Search.Question.Subject), out var p))
            {
                retVal.FacetSubject = p.Select(s => (QuestionSubject)s.AsValueFacetResult<long>().Value);
            }

            if (result.Facets.TryGetValue(nameof(Core.Entities.Search.Question.State), out var p2))
            {
                retVal.FacetState = p2.Select(s => (QuestionFilter)s.AsValueFacetResult<long>().Value);
            }
            return retVal;
        }
    }
}