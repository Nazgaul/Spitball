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
}