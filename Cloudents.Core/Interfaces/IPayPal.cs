using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IPayPal
    {
        Task<PayPalDto> GetPaymentAsync(string transactionId);
    }
}