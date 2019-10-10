using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Questions.Queries.GetQuestionsList;
using Cloudents.Query;
using Cloudents.Query.Query;

namespace Cloudents.Infrastructure.Search.Question
{
    public class QuestionSearch : IQuestionSearch
    {
        private readonly IQuestionsSearch _questionSearch;
        private readonly IQueryBus _queryBus;

        public QuestionSearch(IQuestionsSearch questionSearch, IQueryBus queryBus)
        {
            _questionSearch = questionSearch;
            _queryBus = queryBus;
        }

        public async Task<QuestionWithFacetDto> SearchAsync(QuestionsQuery query, CancellationToken token)
        {
            var searchResult = await _questionSearch.SearchAsync(query, token);
            var ids = searchResult.result.ToList();
            var queryDb = new IdsQuestionsQuery<long>(ids);
            var dbResult = await _queryBus.QueryAsync(queryDb, token);
            var dic = dbResult.ToDictionary(x => x.Id);

            var retVal = new QuestionWithFacetDto();// {Result = dbResult};
            foreach (var resultResult in ids)
            {
                if (dic.TryGetValue(resultResult, out var value))
                {
                    retVal.Result.Add(value);
                }

            }

            if (string.IsNullOrEmpty(query.Term))
            {
               // retVal.FacetSubject = EnumExtension.GetValues<QuestionSubject>();
                retVal.FacetState = EnumExtension.GetValues<QuestionFilter>().Where(w => w.GetAttributeValue<PublicValueAttribute>() != null);
            }
            else
            {
                retVal.FacetState = searchResult.facetFilter;
            }

            return retVal;
        }
    }
}