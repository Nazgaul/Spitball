using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Query.Payment;

namespace Cloudents.Core.Interfaces
{
    public interface IPayment
    {
        //Task<PayMeSellerResponse> CreateSellerAsync(PayMeSeller seller, CancellationToken token);
        Task<GenerateSaleResponse> CreateBuyerAsync(string callback, CancellationToken token);

        Task<GenerateSaleResponse> TransferPaymentAsync(string sellerKey, string buyerKey,decimal price, CancellationToken token);
    }
}