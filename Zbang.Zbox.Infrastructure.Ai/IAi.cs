using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public interface IAi
    {
        Task<IIntent> GetUserIntentAsync(string sentence, CancellationToken token);
    }
}