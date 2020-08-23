using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class WalletControllerTests
    {
        private HttpClient _client;

        public WalletControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        [Theory]
        //[InlineData("api/wallet/balance")]
        //[InlineData("api/wallet/transaction")]
        [InlineData("api/wallet/GetPaymentLink")]
        public async Task GetAsync_Wallet_OkAsync(string uri)
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            var str = await response.Content.ReadAsStringAsync();
            str.IsValidJson().Should().BeTrue();
        }

        [Theory]
        //[InlineData("api/wallet/balance")]
        //[InlineData("api/wallet/transaction")]
        [InlineData("api/wallet/GetPaymentLink")]
        public async Task GetAsync_Wallet_UnauthorizedAsync(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
