using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search.Question
{
    public class AzureQuestionSearch //: IQuestionSearch
    {
        private readonly ISearchIndexClient _client;

        public AzureQuestionSearch(ISearchService client)
        {
            _client = client.GetClient(QuestionSearchWrite.IndexName);
        }

        public async Task<DocumentSearchResult<Core.Entities.Search.Question>> SearchAsync(QuestionsQuery query, CancellationToken token)
        {
            string filterStr = null;

            //if (query.Source != null)
            //{
            //    filterStr = string.Join(" or ", query.Source.Select(s =>
            //        $"{nameof(Question.Subject)} eq '{s}'"));
            //}

            var searchParameter = new SearchParameters
            {
                Facets = new[] { nameof(Core.Entities.Search.Question.Subject), nameof(Core.Entities.Search.Question.State) },
                //Filter = filterStr,
                Top = 50,
                Skip = query.Page * 50,
                ScoringProfile = "ScoringProfile",
                ScoringParameters = new[]
                {
                    new ScoringParameter
                    ("Country"
                        , new[] {"IL"}) //TODO: finish
                }

            };

            return await
                _client.Documents.SearchAsync<Core.Entities.Search.Question>(query.Term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);

            //var retVal = new QuestionWithFacetDto
            //{
            //    Result = result.Results.Select(s=>s.Document)
            //    //Result = result.Results.Select(s=> new QuestionDto()
            //    //{
                    
            //    //    User = new UserDto
            //    //    {
            //    //        Id = s.Document.UserId,
            //    //        Name = s.Document.UserName,
            //    //        Image = s.Document.UserImage
            //    //    },
            //    //    Id = long.Parse(s.Document.Id),
            //    //    DateTime = s.Document.DateTime,
            //    //    Answers = s.Document.AnswerCount,
            //    //    //Subject = s.Document.Subject,
            //    //    Color = s.Document.Color,
            //    //    Files = s.Document.FilesCount,
            //    //    HasCorrectAnswer = s.Document.HasCorrectAnswer,
            //    //    Price = (decimal)s.Document.Price,
            //    //    Text = s.Document.Text
            //    //})
            //   // Result = _mapper.Map<IEnumerable<QuestionDto>>(result.Results.Select(s => s.Document))
            //};
            //if (result.Facets.TryGetValue(nameof(Question.Subject), out var p))
            //{

            //    retVal.FacetSubject = p.Select(s => (int)s.AsValueFacetResult<long>().Value);
                
            //    //retVal.Facet = p.Select(s => s.AsValueFacetResult<string>().Value);
            //}

            //if (result.Facets.TryGetValue(nameof(Question.State), out var p2))
            //{
            //    //var t = p2.Select(s =>
            //    //{
            //    //    return s.AsValueFacetResult<long>().Value;
            //    //});
            //    retVal.FacetState = p2.Select(s => (QuestionFilter) s.AsValueFacetResult<long>().Value);
            //    //
            //    // retVal.Facets[nameof(Question.State)] = p2.Select(s => s.AsValueFacetResult<string>().Value);

            //}
            //return retVal;
        }
    }
}