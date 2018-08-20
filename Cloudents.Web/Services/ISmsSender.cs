using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;

namespace Cloudents.Web.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(User user, CancellationToken token);
        Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token);
    }
}