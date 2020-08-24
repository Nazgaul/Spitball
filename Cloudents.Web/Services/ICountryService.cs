using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Services
{
    public interface ICountryService
    {
        Task<string> GetUserCountryAsync(CancellationToken token);
    }
}