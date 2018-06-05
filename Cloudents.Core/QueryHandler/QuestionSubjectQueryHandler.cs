using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.QueryHandler
{
    public class QuestionSubjectQueryHandler : IQueryHandlerAsync<IEnumerable<QuestionSubjectDto>>
    {
        private readonly IQuestionSubjectRepository _repository;

        public QuestionSubjectQueryHandler(IQuestionSubjectRepository repository)
        {
            _repository = repository;
        }


        public Task<IEnumerable<QuestionSubjectDto>> GetAsync(CancellationToken token)
        {
            return _repository.GetAllSubjectAsync(token);
        }
    }
}