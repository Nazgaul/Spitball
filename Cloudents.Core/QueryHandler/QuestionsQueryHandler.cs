using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;

namespace Cloudents.Core.QueryHandler
{
    public class QuestionsQueryHandler : IQueryHandlerAsync<QuestionsQuery, ResultWithFacetDto<QuestionDto>>
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionsQueryHandler(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public Task<ResultWithFacetDto<QuestionDto>> GetAsync(QuestionsQuery query, CancellationToken token)
        {
            return _questionRepository.GetQuestionsAsync(query, token);
        }
    }
}