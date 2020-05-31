﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Payment;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cloudents.Infrastructure.Payments
{
    public class PayMePaymentProvider : IPaymeProvider, IPaymentProvider
    {
        private readonly HttpClient _client;
        private readonly PayMeCredentials _credentials;

        public PayMePaymentProvider(HttpClient client, PayMeCredentials credentials)
        {
            _client = client;
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        private static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        public async Task<GenerateSaleResponse> CreateBuyerAsync(string callback, string successRedirect, CancellationToken token)
        {
            var generateSale = GenerateSale.CreateBuyer(callback, successRedirect, _credentials.SellerId);

            return await GenerateSaleAsync(token, generateSale);
        }

        public async Task<string> ChargeSessionAsync(Tutor tutor, User user, Guid id, double price, CancellationToken token)
        {

            var sellerKey = tutor.SellerKey ?? throw new ArgumentNullException(nameof(tutor.SellerKey), "tutor seller key is empty");
            var buyerKey = user.Payment?.PaymentKey ?? throw new ArgumentNullException(nameof(PaymePayment.PaymentKey), "buyer key is empty");

            var result = await TransferPaymentAsync(sellerKey, buyerKey, price, token);
            return result.PaymeSaleId;
        }

        public Task<string> ChargeSessionAsync(StudyRoomPayment sessionPayment, double price, CancellationToken token)
        {
            var tutor = sessionPayment.Tutor;
            var user = sessionPayment.User;
            return ChargeSessionAsync(tutor, user, sessionPayment.Id, price, token);

        }

        public async Task<string> ChargeSessionBySpitballAsync(Tutor tutor, double price, CancellationToken token)
        {
            var sellerKey = tutor.SellerKey!;
            var spitballKey = _credentials.BuyerKey;
            var result = await TransferPaymentAsync(sellerKey, spitballKey, price, token);
            return result.PaymeSaleId;

        }

        private async Task<GenerateSaleResponse> TransferPaymentAsync(string sellerKey, string buyerKey, double price, CancellationToken token)
        {
            var generateSale = GenerateSale.TransferMoney(sellerKey, buyerKey, price);

            return await GenerateSaleAsync(token, generateSale);
        }

        public async Task<GenerateSaleResponse> BuyTokens(PointBundle price, string successRedirect, CancellationToken token)
        {
            var generateSale = GenerateSale.BuyTokens(price, successRedirect, _credentials.SellerId);

            return await GenerateSaleAsync(token, generateSale);
        }

        private async Task<GenerateSaleResponse> GenerateSaleAsync(CancellationToken token, GenerateSale generateSale)
        {
            var json = JsonConvert.SerializeObject(generateSale, new JsonSerializerSettings
            {
                ContractResolver = ContractResolver,
                NullValueHandling = NullValueHandling.Ignore
            });
            using var sr = new StringContent(json, Encoding.UTF8, "application/json");
            Uri.TryCreate(new Uri(_credentials.EndPoint), "generate-sale", out var uri);

            var response = await _client.PostAsync(uri, sr, token);
            if (!response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"statusCode: {response.StatusCode} reason: {response.ReasonPhrase}, body: {str}");
            }

            await using var s = await response.Content.ReadAsStreamAsync();
            var serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = ContractResolver,
            });

            return s.ToJsonReader<GenerateSaleResponse>(serializer);
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local", Justification = "Need for serialization")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Need for serialization")]

        private class GenerateSale
        {
            public static GenerateSale TransferMoney(string sellerId, string buyerId, double price)
            {
                return new GenerateSale()
                {
                    SellerPaymeId = sellerId,
                    SalePrice = (int)(price * 100),
                    BuyerKey = buyerId,
                    ProductName = "עבור שיעורים פרטיים בספיטבול"
                };
            }

            private GenerateSale()
            {

            }

            public static GenerateSale BuyTokens(PointBundle price, string saleReturnUrl, string sellerId)
            {
                return new GenerateSale()
                {
                    CaptureBuyer = 0,
                    SalePrice = (price.PriceInILS * 100),
                    SaleReturnUrl = saleReturnUrl,
                    SellerPaymeId = sellerId,
                    ProductName = "עבור קניית נקודות בספיטבול"
                };
            }

            public static GenerateSale CreateBuyer(string saleCallbackUrl, string saleReturnUrl, string sellerId)
            {
                return new GenerateSale()
                {
                    SellerPaymeId = sellerId,
                    SalePrice = 0,
                    CaptureBuyer = 1,
                    SaleType = "token",
                    SaleReturnUrl = saleReturnUrl,
                    SaleCallbackUrl = saleCallbackUrl,
                    ProductName = "עבור שיעורים פרטיים בספיטבול"

                };
            }



            public string SellerPaymeId { get; private set; }

            public int SalePrice { get; private set; }
            [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used for serialize")]
            public string Currency => "ILS";
            public string ProductName { get; private set; }
            public int? CaptureBuyer { get; private set; }
            public string SaleType { get; private set; }
            public string SaleCallbackUrl { get; private set; }
            public string BuyerKey { get; private set; }

            public string SaleReturnUrl { get; private set; }


        }


    }
}