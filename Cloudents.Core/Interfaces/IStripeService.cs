using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{

    public interface IStripeService
    {
        Task<string> BuyPointsAsync(PointBundle bundle, string email, string successCallback, string fallbackCallback,
            CancellationToken token);

        Task<(string receipt, long points)> GetSessionByIdAsync(string sessionId, CancellationToken token);

        Task<string> CreateCustomerAsync(User user, CancellationToken token);
        Task<string> FutureCardPayments(string stripeClientId);
        //Task<string?> RetrieveCustomerIdAsync(string email, CancellationToken token);

        Task CreateProductAsync(Tutor tutor,CancellationToken token);
        //Task<string?> RetrieveCustomerIdAsync(string email, CancellationToken token);
    }
}