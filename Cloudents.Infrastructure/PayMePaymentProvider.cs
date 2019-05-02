using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Payment;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cloudents.Infrastructure
{
    public class PayMePaymentProvider : IPayment
    {
        private readonly HttpClient _client;
        private readonly PayMeCredentials _credentials;

        public PayMePaymentProvider(HttpClient client, [NotNull] PayMeCredentials credentials)
        {
            _client = client;
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        private static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        public async Task<GenerateSaleResponse> CreateBuyerAsync(string callback, CancellationToken token)
        {
            var generateSale = GenerateSale.CreateBuyer(callback);

            return await GenerateSaleAsync(token, generateSale);
        }

        public async Task<GenerateSaleResponse> TransferPaymentAsync(string sellerKey, string buyerKey, decimal price, CancellationToken token)
        {
            var generateSale = GenerateSale.TransferMoney(sellerKey, buyerKey, price);

            return await GenerateSaleAsync(token, generateSale);
        }

        private async Task<GenerateSaleResponse> GenerateSaleAsync(CancellationToken token, GenerateSale generateSale)
        {
            var json = JsonConvert.SerializeObject(generateSale, new JsonSerializerSettings
            {
                ContractResolver = ContractResolver,
                NullValueHandling = NullValueHandling.Ignore
            });
            using (var sr = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                Uri.TryCreate(new Uri(_credentials.EndPoint), "generate-sale", out var uri);

                var response = await _client.PostAsync(uri, sr, token);
                if (!response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"statusCode: {response.StatusCode} reason: {response.ReasonPhrase}, body: {str}");
                }
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
            public static GenerateSale TransferMoney(string sellerId, string buyerId, decimal price)
            {
                return new GenerateSale()
                {
                    SellerPaymeId = sellerId,
                    SalePrice = (int)price * 100,
                    BuyerKey = buyerId
                };
            }

            private GenerateSale()
            {

            }

            public static GenerateSale CreateBuyer(string saleCallbackUrl)
            {
                return new GenerateSale()
                {
                    SellerPaymeId = "MPL15546-31186SKB-53ES24ZG-WGVCBKO2",
                    SalePrice = 0,
                    CaptureBuyer = 1,
                    SaleType = "token",
                    SaleCallbackUrl = saleCallbackUrl
                };
            }

            public string SellerPaymeId { get; private set; }

            public int SalePrice { get; private set; }
            public string Currency => "ILS";
            public string ProductName => "Tutoring";
            public int? CaptureBuyer { get; private set; }
            public string SaleType { get; private set; }
            public string SaleCallbackUrl { get; private set; }
            public string BuyerKey { get; private set; }


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