using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IGoogleAnalytics
    {
        Task TrackEventAsync(string category, string action, string label, CancellationToken token);
    }
}