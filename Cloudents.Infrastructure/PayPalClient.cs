using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace Cloudents.Infrastructure
{
    public class PayPalClient : IPayPal
    {

        private readonly PayPalHttpClient _client;
        public PayPalClient(IConfigurationKeys configurationKeys)
        {
            if (configurationKeys.PayPal.IsDevelop)
            {
                var environment = new SandboxEnvironment(
                    configurationKeys.PayPal.ClientId,
                    configurationKeys.PayPal.ClientSecret);
                _client = new PayPalHttpClient(environment);
            }
            else
            {
                var environment = new LiveEnvironment(
                    configurationKeys.PayPal.ClientId,
                    configurationKeys.PayPal.ClientSecret);
                _client = new PayPalHttpClient(environment);
            }
        }

        //public async Test()
        //{
        //    var request = new OrdersGetRequest();
        //}
        public async Task<PayPalDto> GetPaymentAsync(string transactionId)
        {
            // // var client = new PayPalHttpClient(_environment);
//            var t = new PayPalCheckoutSdk.Payments.AuthorizationsGetRequest(transactionId);
            var request = new PayPalCheckoutSdk.Payments.CapturesGetRequest(transactionId);
            //var t = new PaymentsGetRequest(transactionId);
            var response = await _client.Execute(request);
            var payment = response.Result<PayPalCheckoutSdk.Payments.Capture>();
            Sku sku = Sku.PaymentThree;
            return new PayPalDto(transactionId,  sku.Amount);
        }

        public async Task PathOrderAsync(string orderId, CancellationToken token)
        {
            //var get = new OrdersGetRequest(orderId);
            //var response = await _client.Execute(get);
            //var result = response.Result<Order>();
            var request = new OrdersPatchRequest<AmountWithBreakdown>(orderId);

            request.RequestBody(BuildPatchRequest());
            var response = await _client.Execute(request);

            var request2 = new OrdersPatchRequest<string>(orderId);

            //request2.RequestBody(BuildPatchesRequest2());
            //var response2 = await _client.Execute(request2);

            var captureRequest = new OrdersCaptureRequest(orderId);
            captureRequest.RequestBody(new OrderActionRequest());
            var response3 = await _client.Execute(captureRequest);
            //var approvePayment = new OrdersAuthorizeRequest(orderId);
            //await _client.Execute(approvePayment);
            //3. Call PayPal to patch the transaction
        }

        //private static List<Patch<string>> BuildPatchesRequest2()
        //{
        //    return new List<Patch<string>>
        //    {
        //        new Patch<string>
        //        {
        //            Op = "replace",
        //            Path = "/intent",
        //            Value = "CAPTURE"

        //        }
        //    };
        //}

        private static List<Patch<AmountWithBreakdown>> BuildPatchRequest()
        {
            var patches = new List<Patch<AmountWithBreakdown>>
            {
                //new Patch<object>
                //{
                //    Op= "replace",
                //    Path= "/intent",
                //    Value= "CAPTURE"

                //},
                //new Patch<object>
                //{
                //    Op= "add",
                //    Path= "/purchase_units/@reference_id=='PUHF'/description",
                //    Value= "Physical Goods"

                //},
                new Patch<AmountWithBreakdown>
                {
                    Op= "replace",
                    Path= "/purchase_units/@reference_id=='PUHF'/amount",
                    Value= new AmountWithBreakdown
                    {
                        Value = "500",
                        CurrencyCode = "USD"
                    }

                }

            };
            return patches;
        }

        public class PayPalAmount
        {
            public string Value { get; set; }

            public string Currency_Code { get; set; }
        }


        private sealed class Sku
        {
            public static readonly Sku PaymentThree = new Sku("points_3", 1400);
            private static readonly Sku PaymentTwo = new Sku("points_2", 500);
            private static readonly Sku PaymentOne = new Sku("points_1", 100);


            public static implicit operator Sku(string tb)
            {
                if (PaymentOne.Id.Equals(tb))
                {
                    return PaymentOne;
                }
                if (PaymentTwo.Id.Equals(tb))
                {
                    return PaymentTwo;
                }
                if (PaymentThree.Id.Equals(tb))
                {
                    return PaymentThree;
                }
                throw new ArgumentException();
            }
            private Sku(string id, decimal amount)
            {
                Id = id;
                Amount = amount;
            }
            private string Id { get; }
            public decimal Amount { get; }



            private bool Equals(Sku other)
            {
                return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Sku other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (Id != null ? Id.ToLowerInvariant().GetHashCode() : 0);
            }
        }
    }
}