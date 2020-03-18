using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IPayPalService
    {
        Task<PayPalDto> GetPaymentAsync(string transactionId, CancellationToken token);
        //Task UpdateAndConfirmOrderAsync(string orderId, decimal charge, CancellationToken token);
    }
}