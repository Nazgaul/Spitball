using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IQuestionSearch
    {
        Task<ResultWithFacetDto<QuestionDto>> SearchAsync(string term, IEnumerable<string> facet, CancellationToken token);
    }
}