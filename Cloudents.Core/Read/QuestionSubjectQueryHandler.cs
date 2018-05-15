using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Read
{
    public class QuestionSubjectQueryHandler : IQueryHandlerAsync<IEnumerable<QuestionSubjectDto>>
    {
        private readonly IQuestionSubjectRepository _repository;
        private readonly IMapper _mapper;

        public QuestionSubjectQueryHandler(IQuestionSubjectRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<QuestionSubjectDto>> GetAsync(CancellationToken token)
        {
            var t = await _repository.GetAllSubjectAsync(token).ConfigureAwait(false);
            return _mapper.Map<IEnumerable<QuestionSubjectDto>>(t);
        }
    }
}