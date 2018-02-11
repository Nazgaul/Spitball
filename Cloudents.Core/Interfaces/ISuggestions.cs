using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ISuggestions
    {
        Task<IEnumerable<string>> SuggestAsync(string query, CancellationToken token);
    }
}