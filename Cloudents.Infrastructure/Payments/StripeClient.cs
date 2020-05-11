using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Cloudents.Infrastructure.Payments
{
    public class StripeClient : IPaymentStripe
    {
        public async Task<string> BuyPointsAsync(string successCallback, string fallbackCallback)
        {
            StripeConfiguration.ApiKey = "sk_test_Ihn6pkUZV9VFpDo7JWUGwT8700FAQ3Gbhf";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions {
                        Name = "T-shirt",
                        Description = "Comfortable cotton t-shirt",
                        Amount = 500,
                        Currency = "usd",
                        Quantity = 1,
                    },
                },
                SuccessUrl = successCallback,
                CancelUrl = fallbackCallback,
                CustomerEmail = "ram@cloudents.com"
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Id;

            //var options = new PaymentIntentCreateOptions
            //{
            //    Amount = 1099,
            //    Currency = "usd",
            //    // Verify your integration in this guide by including this parameter
            //    Metadata = new Dictionary<string, string>
            //    {
            //        { "integration_check", "accept_a_payment" },
            //    },
            //};

            //var service = new PaymentIntentService();
            //var paymentIntent = await service.CreateAsync(options);
            //return paymentIntent
        }
    }
}