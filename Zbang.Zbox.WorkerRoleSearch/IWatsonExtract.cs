using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Culture;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public interface IWatsonExtract
    {
        Task<IEnumerable<string>> GetConceptAsync(string text, CancellationToken token);
        Task<IEnumerable<string>> GetKeywordAsync(string text, CancellationToken token);
        Task<Language> GetLanguageAsync(string text, CancellationToken token);
    }
}