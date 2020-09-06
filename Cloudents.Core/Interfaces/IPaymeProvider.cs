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


        //Task<string> CreatePaymentAsync(StripePaymentRequest model,
        //    CancellationToken token);

        Task<GenerateSaleResponse> BuyCourseAsync(Money price, string courseName, string successRedirect,
            string sellerId, CancellationToken token);
    }
}