using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Application.Interfaces
{
    public interface IKeyGenerator
    {
        string GenerateKey(object sourceObject);
    }

    public interface ITextAnalysis
    {
        Task<IEnumerable<KeyValuePair<T, CultureInfo>>> DetectLanguageAsync<T>(
            IEnumerable<KeyValuePair<T, string>> texts, CancellationToken token);

        Task<CultureInfo> DetectLanguageAsync(string text, CancellationToken token);
    }
}