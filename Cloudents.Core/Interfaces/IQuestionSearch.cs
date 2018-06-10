using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Query;

namespace Cloudents.Core.Interfaces
{
    public interface IQuestionSearch
    {
        Task<ResultWithFacetDto<QuestionDto>> SearchAsync(QuestionsQuery query, CancellationToken token);
    }
}