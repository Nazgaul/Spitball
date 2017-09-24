using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IQuestionSearch
    {
        Task<IEnumerable<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token);
    }
}