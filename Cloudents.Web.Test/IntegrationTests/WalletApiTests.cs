using FluentAssertions;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class WalletApiTests
    {
        private System.Net.Http.HttpClient _client;

        public WalletApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }


        [Theory]
        [InlineData("api/wallet/balance")]
        [InlineData("api/wallet/transaction")]
        [InlineData("api/wallet/GetPaymentLink")]
        public async Task GetAsync_Wallet_Ok(string uri)
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("api/wallet/balance")]
        [InlineData("api/wallet/transaction")]
        [InlineData("api/wallet/GetPaymentLink")]
        public async Task GetAsync_Wallet_Unauthorized(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
