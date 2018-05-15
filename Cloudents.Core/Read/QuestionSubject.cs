using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Read
{
    public class QuestionSubject : IQueryHandlerAsync<QuestionSubjectQuery, QuestionSubjectResult>
    {
        private readonly IQuestionSubjectRepository _repository;

        public QuestionSubject(IQuestionSubjectRepository repository)
        {
            _repository = repository;
        }
       

        public async Task<QuestionSubjectResult> ExecuteAsync(QuestionSubjectQuery command, CancellationToken token)
        {
            var t = await _repository.GetAllSubjectAsync().ConfigureAwait(false);

            return null;
        }
    }
}