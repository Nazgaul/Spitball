using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Payment;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cloudents.Infrastructure
{
    public class PayMePaymentProvider : IPayment
    {
        private readonly IRestClient _client;

        public PayMePaymentProvider(IRestClient client)
        {
            _client = client;
        }

        public async Task<PayMeSellerResponse> CreateSellerAsync(PayMeSeller seller, CancellationToken token)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            string json = JsonConvert.SerializeObject(seller, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
            });
            var xx = await _client.PostJsonAsync<string>(new Uri("https://preprod.paymeservice.com/api/create-seller"),
                json, null, token);

            return JsonConvert.DeserializeObject<PayMeSellerResponse>(xx, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
            });

        }
    }


}