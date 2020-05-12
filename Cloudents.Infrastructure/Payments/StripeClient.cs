using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Cloudents.Infrastructure.Payments
{
    public class StripeClient : IStripeService
    {
        static StripeClient()
        {
            StripeConfiguration.ApiKey = "sk_test_Ihn6pkUZV9VFpDo7JWUGwT8700FAQ3Gbhf";

        }
        public async Task GetEventsAsync(string sessionId)
        {
            var service = new EventService();
            var options = new EventListOptions
            {
                Type = "checkout.session.completed",
                
                Created = new DateRangeOptions
                {
                    // Check for events created in the last 24 hours.
                    GreaterThan = DateTime.Now.Subtract(new TimeSpan(1, 0, 0)),
                },
            };
            

            foreach (var stripeEvent in service.ListAutoPaging(options))
            {
                var session = stripeEvent.Data.Object as Session;

                // Fulfill the purchase...
               // handleCheckoutSession(session);
            }
        }

        public async Task<string> BuyPointsAsync(string successCallback, string fallbackCallback)
        {

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions {
                        Name = "Buy Points on Spitball",
                        //Description = "Comfortable cotton t-shirt",
                        Amount = 500,
                        Currency = "usd",
                        Quantity = 1,
                        
                    },
                    
                },
                Metadata = new Dictionary<string, string>()
                {
                    ["Points"] = 100.ToString()
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