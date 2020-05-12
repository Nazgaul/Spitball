using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IPayPalService
    {
        Task<(string authorizationId, decimal amount)> AuthorizationOrderAsync(string orderId, CancellationToken token);
        Task CaptureAuthorizedOrderAsync(string authorizationId, decimal newAmount, CancellationToken token);
        Task<PayPalDto> GetPaymentAsync(string transactionId, CancellationToken token);
        //Task UpdateAndConfirmOrderAsync(string orderId, decimal charge, CancellationToken token);
    }

    public interface IStripeService
    {
        Task<string> BuyPointsAsync(PointBundle bundle, string email, string successCallback, string fallbackCallback,
            CancellationToken token);

        Task<(string receipt, long points)> GetEventsAsync(string sessionId, CancellationToken token);

        // Task Retrieve(string successCallback, string fallbackCallback);
    }
}