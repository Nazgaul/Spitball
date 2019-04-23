using System;
using System.IO;
using System.Net.Http;
using System.Text;
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
        private readonly HttpClient _client;

        public PayMePaymentProvider(HttpClient client)
        {
            _client = client;
        }

        //public async Task<PayMeSellerResponse> CreateSellerAsync(PayMeSeller seller, CancellationToken token)
        //{
        //    var contractResolver = new DefaultContractResolver
        //    {
        //        NamingStrategy = new SnakeCaseNamingStrategy()
        //    };
        //    string json = JsonConvert.SerializeObject(seller, new JsonSerializerSettings
        //    {
        //        ContractResolver = contractResolver,
        //    });
        //    var xx = await _client.PostJsonAsync<string>(new Uri("https://preprod.paymeservice.com/api/create-seller"),
        //        json, null, token);

        //    return JsonConvert.DeserializeObject<PayMeSellerResponse>(xx, new JsonSerializerSettings
        //    {
        //        ContractResolver = contractResolver,
        //    });

        //}

        private static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        public async Task<GenerateSaleResponse> CreatePayment(string callback, CancellationToken token)
        {
            var generateSale = new GenerateSale(callback);

            var json = JsonConvert.SerializeObject(generateSale, new JsonSerializerSettings
            {
                ContractResolver = ContractResolver,
            });
            using (var sr = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                var response = await _client.PostAsync("https://preprod.paymeservice.com/api/generate-sale", sr, token);
                response.EnsureSuccessStatusCode();
                using (var s = await response.Content.ReadAsStreamAsync())
                {
                    var result = s.ToJsonReader(reader =>
                    {
                        var serializer = JsonSerializer.Create(new JsonSerializerSettings
                        {
                            ContractResolver = ContractResolver,
                        });
                        return serializer.Deserialize<GenerateSaleResponse>(reader);
                    });

                    return result;
                }

            }
        }


        private class GenerateSale
        {
            public GenerateSale(string saleCallbackUrl)
            {
                SaleCallbackUrl = saleCallbackUrl;
            }

            public string SellerPaymeId => "MPL15546-31186SKB-53ES24ZG-WGVCBKO2";
            public int SalePrice => 0;
            public string Currency => "ILS";
            public string ProductName => "Tutoring";
            public int CaptureBuyer => 1;
            public string SaleType => "token";
            public string SaleCallbackUrl { get; }


        }
    }

    public static class StreamExtensions
    {
        public static T ToJsonReader<T>(this Stream s, Func<JsonTextReader, T> func)
        {

            using (var sr = new StreamReader(s))
            using (var reader = new JsonTextReader(sr))
            {
                return func(reader);
            }
        }
    }


}