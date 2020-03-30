using System.Globalization;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;

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

        public async Task<(string authorizationId, decimal amount)> AuthorizationOrderAsync(string orderId, CancellationToken token)
        {

            var authorizeRequest = new OrdersAuthorizeRequest(orderId);
            authorizeRequest.RequestBody(new AuthorizeRequest());
            var response3 = await _client.Execute(authorizeRequest);
            token.ThrowIfCancellationRequested();
            var result = response3.Result<Order>();
            var authorization = result.PurchaseUnits[0].Payments.Authorizations[0];
            //var authorization = result.PurchaseUnits[0].Payments.Authorizations[0].Id;
            return (authorization.Id, decimal.Parse(authorization.Amount.Value));
        }

        public async Task CaptureAuthorizedOrderAsync(string authorizationId, decimal newAmount, CancellationToken token)
        {
            var authorizationsCapture = new AuthorizationsCaptureRequest(authorizationId);
            token.ThrowIfCancellationRequested();
            authorizationsCapture.RequestBody(new CaptureRequest()
            {
                Amount = new PayPalCheckoutSdk.Payments.Money()
                {
                    Value = newAmount.ToString(CultureInfo.InvariantCulture),
                    CurrencyCode = "USD"
                },
                FinalCapture = true
            });
            await _client.Execute(authorizationsCapture);
        }




        public async Task<PayPalDto> GetPaymentAsync(string orderId, CancellationToken token)
        {
            var request = new OrdersGetRequest(orderId);
            var response3 = await _client.Execute(request);
            var result = response3.Result<Order>();
            var purchaseUnit = result.PurchaseUnits[0];

            return new PayPalDto(purchaseUnit.ReferenceId,
                decimal.Parse(purchaseUnit.Payments.Captures[0].Amount.Value));
        }
    }
}