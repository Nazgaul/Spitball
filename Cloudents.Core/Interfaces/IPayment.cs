using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Query.Payment;

namespace Cloudents.Core.Interfaces
{
    public interface IPayment
    {
        Task<PayMeSellerResponse> CreateSellerAsync(PayMeSeller seller, CancellationToken token);
    }
}