using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ISmsProvider
    {
        Task<(string phoneNumber, string country)> ValidateNumberAsync(string phoneNumber, CancellationToken token);
    }
}