using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;

namespace Cloudents.Core.QueryHandler
{
    public class QuestionSubjectQueryHandler : IQueryHandlerAsync<QuestionSubjectQuery, IEnumerable<QuestionSubjectDto>>
    {
        private readonly IQuestionSubjectRepository _repository;

        public QuestionSubjectQueryHandler(IQuestionSubjectRepository repository)
        {
            _repository = repository;
        }


        public Task<IEnumerable<QuestionSubjectDto>> GetAsync(QuestionSubjectQuery query, CancellationToken token)
        {
            return _repository.GetAllSubjectAsync(token);
        }
    }
}