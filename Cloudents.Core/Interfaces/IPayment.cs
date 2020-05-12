using Cloudents.Core.Query.Payment;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IPayment
    {
        //Task<PayMeSellerResponse> CreateSellerAsync(PayMeSeller seller, CancellationToken token);
        Task<GenerateSaleResponse> CreateBuyerAsync(string callback, string successRedirect, CancellationToken token);

        Task<GenerateSaleResponse> TransferPaymentAsync(string sellerKey, string buyerKey, decimal price, CancellationToken token);

        Task<GenerateSaleResponse> BuyTokens(PointBundle price, string successRedirect, CancellationToken token);
    }
}