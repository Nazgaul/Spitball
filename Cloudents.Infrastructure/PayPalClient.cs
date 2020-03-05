using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public async Task<PayPalDto> GetPaymentAsync(string orderId)
        {

            var captureRequest = new OrdersCaptureRequest(orderId);
            captureRequest.RequestBody(new OrderActionRequest());
            var response3 = await _client.Execute(captureRequest);
            var result = response3.Result<Order>();
            Sku sku = result.PurchaseUnits[0].ReferenceId;
            return new PayPalDto(orderId,  sku.Amount);
        }

        public async Task UpdateAndConfirmOrderAsync(string orderId, decimal charge, CancellationToken token) 
        {
            var request = new OrdersPatchRequest<AmountWithBreakdown>(orderId);
            request.RequestBody(BuildPatchRequest(charge));
            await _client.Execute(request);
            var captureRequest = new OrdersCaptureRequest(orderId);
            captureRequest.RequestBody(new OrderActionRequest());
            var response3 = await _client.Execute(captureRequest);
            var result = response3.Result<Order>();

            //TODO - get receipt
            ////var approvePayment = new OrdersAuthorizeRequest(orderId);
            ////await _client.Execute(approvePayment);
            ////3. Call PayPal to patch the transaction
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

        private static List<Patch<AmountWithBreakdown>> BuildPatchRequest(decimal charge)
        {
            var patches = new List<Patch<AmountWithBreakdown>>
            {
                new Patch<AmountWithBreakdown>
                {
                    Op= "replace",
                    Path= "/purchase_units/@reference_id=='PUHF'/amount",
                    Value= new AmountWithBreakdown
                    {
                        Value = charge.ToString(CultureInfo.InvariantCulture),
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
            public static readonly Sku PaymentThree = new Sku("points_3", 1000);
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