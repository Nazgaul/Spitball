using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Cloudents.Infrastructure.Payments
{
    public class StripeClient : IStripeService, IPaymentProvider
    {
        public StripeClient(IConfigurationKeys configuration)
        {
            StripeConfiguration.ApiKey = configuration.Stripe;
        }
        
        public async Task<(string receipt, long points)> GetSessionByIdAsync(string sessionId, CancellationToken token)
        {
            var service2 = new SessionService();
            var session = await service2.GetAsync(sessionId, cancellationToken: token);
            var amountOfPoints = long.Parse(session.Metadata["Points"]);
            var paymentId = session.PaymentIntentId;
            return (paymentId, amountOfPoints);
        }

        public async Task<string> CreateCustomerAsync(User user, CancellationToken token)
        {
            var customer = await RetrieveCustomerByEmailAsync(user.Email, token);
            var service = new CustomerService();
            if (customer == null)
            {
                var options = new CustomerCreateOptions
                {
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Metadata = new Dictionary<string, string>()
                    {
                        ["Id"] = user.Id.ToString()
                    }
                };

                customer = await service.CreateAsync(options, cancellationToken: token);

            }
            else
            {
                await service.UpdateAsync(customer.Id, new CustomerUpdateOptions()
                {
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Metadata = new Dictionary<string, string>()
                    {
                        ["Id"] = user.Id.ToString()
                    }
                }, cancellationToken: token);
            }

            return customer.Id;
        }

        public async Task<string> FutureCardPayments(string stripeClientId)
        {
            var intentCreateOptions = new SetupIntentCreateOptions
            {
                Customer = stripeClientId

            };
            var intentService = new SetupIntentService();
            var intent = await intentService.CreateAsync(intentCreateOptions);
            return intent.ClientSecret;
        }

        private async Task<Customer?> RetrieveCustomerByEmailAsync(string email, CancellationToken token)
        {
            var options = new CustomerListOptions()
            {
                Email = email
            };

            var service = new CustomerService();
            var result = await service.ListAsync(options, cancellationToken: token);
            return result.Data.FirstOrDefault();
        }

        private async Task<string> RetrieveCustomerByIdAsync(string id, CancellationToken token)
        {
            var service = new CustomerService();
            var result = await service.GetAsync(id, cancellationToken: token);
            if (result == null)
            {
                throw new NullReferenceException("no such customer");
            }
            //var result = await RetrieveCustomerAsync(emailId, token);
            return result.Id;
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

        public async Task<string> ChargeSessionAsync(Tutor tutor, User user, double price, CancellationToken token)
        {
            var customerId = await RetrieveCustomerByIdAsync(user.Payment!.PaymentKey, default);
            var optionsX = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            var service2 = new PaymentMethodService();
            var stripeList = await service2.ListAsync(optionsX, cancellationToken: token);
            var paymentMethod = stripeList.Data.First();



            //try
            //{
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (int)(price * 100),
                Currency = "usd",
                Customer = customerId,
                PaymentMethod = paymentMethod.Id,
                Confirm = true,
                OffSession = true
            };
            var result = await service.CreateAsync(options, cancellationToken: token);
            return result.InvoiceId;
        }

        public Task<string> ChargeSessionBySpitballAsync(Tutor tutor, double price, CancellationToken token)
        {
            throw new NotImplementedException("We do not support this feature");
        }
    }
}