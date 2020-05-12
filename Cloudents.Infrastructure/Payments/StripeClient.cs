using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Cloudents.Infrastructure.Payments
{
    public class StripeClient : IStripeService
    {
        public StripeClient(IConfigurationKeys configuration)
        {
            StripeConfiguration.ApiKey = configuration.Stripe;
        }
        static StripeClient()
        {
            StripeConfiguration.ApiKey = "sk_test_Ihn6pkUZV9VFpDo7JWUGwT8700FAQ3Gbhf";

        }
        public async Task<(string receipt, long points)> GetEventsAsync(string sessionId, CancellationToken token)
        {
            var service2 = new SessionService();
            var sessionxx = await service2.GetAsync(sessionId);
            var amountOfPoints = long.Parse(sessionxx.Metadata["Points"]);
            var paymentId = sessionxx.PaymentIntentId;
            return (paymentId, amountOfPoints);
        }

        public async Task<string> BuyPointsAsync(PointBundle bundle, string email, string successCallback, string fallbackCallback, CancellationToken token)
        {

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions {
                        Name = "Buy Points on Spitball",
                        Amount = (long)(bundle.PriceInUsd * 100),
                        Currency = "usd",
                        Quantity = 1
                    },

                },
                Metadata = new Dictionary<string, string>()
                {
                    ["Points"] = bundle.Points.ToString()
                },

                SuccessUrl = successCallback,
                CancelUrl = fallbackCallback,
                CustomerEmail = email
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: token);
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