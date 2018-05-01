using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface ISuggestions
    {
        Task<IEnumerable<string>> SuggestAsync(string query, CancellationToken token);
    }

    public interface ITutorSuggestion : ISuggestions
    {
        [ItemCanBeNull]
        Task<string> GetValueAsync(string query, CancellationToken token);

    }
}