using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;
using Cloudents.Application.Extension;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;
using Cloudents.Application.Questions.Queries.GetQuestionsList;
using Cloudents.Common;
using Cloudents.Common.Attributes;
using Cloudents.Common.Enum;

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
            var queryDb = new IdsQuery<long>(ids);
            var dbResult = await _queryBus.QueryAsync<IList<QuestionFeedDto>>(queryDb, token);
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
                retVal.FacetSubject = EnumExtension.GetValues<QuestionSubject>();
                retVal.FacetState = EnumExtension.GetValues<QuestionFilter>().Where(w => w.GetAttributeValue<PublicValueAttribute>() != null);
            }
            else
            {
                retVal.FacetSubject = searchResult.facetSubject;
                retVal.FacetState = searchResult.facetFileter;
            }

            return retVal;
        }
    }
}