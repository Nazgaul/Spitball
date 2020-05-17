using Cloudents.Core.Query.Payment;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IPaymeProvider
    {
        //Task<PayMeSellerResponse> CreateSellerAsync(PayMeSeller seller, CancellationToken token);
        Task<GenerateSaleResponse> CreateBuyerAsync(string callback, string successRedirect, CancellationToken token);


        Task<GenerateSaleResponse> BuyTokens(PointBundle price, string successRedirect, CancellationToken token);
    }

    public interface IPaymentProvider
    {
        Task<string> ChargeSessionAsync(Tutor tutor, User user, decimal price, CancellationToken token);

        Task<string> ChargeSessionBySpitballAsync(Tutor tutor, decimal price, CancellationToken token);

    }


}