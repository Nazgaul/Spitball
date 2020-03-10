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
    public class PayPalClient : IPayPalService
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
        public async Task<PayPalDto> GetPaymentAsync(string orderId, CancellationToken token)
        {

            var captureRequest = new OrdersCaptureRequest(orderId);
            captureRequest.RequestBody(new OrderActionRequest());
            token.ThrowIfCancellationRequested();
            var response3 = await _client.Execute(captureRequest);
            var result = response3.Result<Order>();
            var purchaseUnit = result.PurchaseUnits[0];

            //return decimal.Parse(result.PurchaseUnits[0].Payments.Captures[0].Amount.Value);
            //Sku sku = result.PurchaseUnits[0].ReferenceId;
            return new PayPalDto(purchaseUnit.ReferenceId, decimal.Parse(purchaseUnit.Payments.Captures[0].Amount.Value));
        }

        //public async Task UpdateAndConfirmOrderAsync(string orderId, decimal charge, CancellationToken token) 
        //{
        //    var request = new OrdersPatchRequest<AmountWithBreakdown>(orderId);
        //    request.RequestBody(BuildPatchRequest(charge));
        //    await _client.Execute(request);
        //    var captureRequest = new OrdersCaptureRequest(orderId);
        //    captureRequest.RequestBody(new OrderActionRequest());
        //    var response3 = await _client.Execute(captureRequest);
        //    var result = response3.Result<Order>();

        //    //TODO - get receipt
        //    ////var approvePayment = new OrdersAuthorizeRequest(orderId);
        //    ////await _client.Execute(approvePayment);
        //    ////3. Call PayPal to patch the transaction
        //}

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

        //private static List<Patch<AmountWithBreakdown>> BuildPatchRequest(decimal charge)
        //{
        //    var patches = new List<Patch<AmountWithBreakdown>>
        //    {
        //        new Patch<AmountWithBreakdown>
        //        {
        //            Op= "replace",
        //            Path= "/purchase_units/@reference_id=='PUHF'/amount",
        //            Value= new AmountWithBreakdown
        //            {
        //                Value = charge.ToString(CultureInfo.InvariantCulture),
        //                CurrencyCode = "USD"
        //            }

        //        }

        //    };
        //    return patches;
        //}

        //public class PayPalAmount
        //{
        //    public string Value { get; set; }

        //    public string Currency_Code { get; set; }
        //}


        
    }
}