using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Threading.Tasks;

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
            // var client = new PayPalHttpClient(_environment);
            var t = new PaymentGetRequest(transactionId);
            var response = await _client.Execute(t);
            var payment = response.Result<Payment>();
            Sku sku = payment.Transactions[0].ItemList.Items[0].Sku;

            return new PayPalDto(transactionId, sku.Amount);
        }

        private sealed class Sku
        {
            private static readonly Sku PaymentThree = new Sku("points_3", 1400);
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