using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Query.Payment;

namespace Cloudents.Core.Interfaces
{

    public interface IStripeService
    {
        //Task<string> BuyPointsAsync(PointBundle bundle,
        //    string email, 
        //    string successCallback, string fallbackCallback,
        //    CancellationToken token);

        Task<(string receipt, long points)> GetBuyPointDataByIdAsync(string sessionId, CancellationToken token);

        Task<long> GetSubscriptionByIdAsync(string sessionId, CancellationToken token);

        Task<string> CreateCustomerAsync(User user, CancellationToken token);
        Task<string> FutureCardPaymentsAsync(string stripeClientId);
        Task CreateProductAsync(Tutor tutor,CancellationToken token);
        Task<string> SubscribeToTutorAsync(long tutorId, string userEmail, string successCallback,
            string fallbackCallback, CancellationToken token);
        Task<string> GetStripeUserIdAsync(string code, CancellationToken token);

        Task<string> CreatePaymentAsync(StripePaymentRequest model,
            CancellationToken token);
    }
}