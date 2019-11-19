using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class WalletControllerTests //: IClassFixture<SbWebApplicationFactory>
    {

        private readonly System.Net.Http.HttpClient _client;

        private readonly string[] _types = { "Earned", "Stake", "Spent" };



        public WalletControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        [Fact]
        public async Task GetAsync_Balance()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync("api/wallet/balance");

            var str = await response.Content.ReadAsStringAsync();

            var d = JArray.Parse(str);

            for (int i = 0; i < 3; i++)
            {
                var type = d[i]["type"]?.Value<string>();
                var points = d[i]["points"]?.Value<decimal?>();
                var value = d[i]["value"]?.Value<string>();

                type.Should().Be(_types[i]);
                points.Should().NotBeNull();
                value.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetAsync_Transaction()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync("api/wallet/transaction");

            var str = await response.Content.ReadAsStringAsync();

            var d = JArray.Parse(str);

            var date = d[d.Count - 1]["date"]?.Value<DateTime?>();
            var action = d[d.Count - 1]["action"]?.Value<string>();
            var type = d[d.Count - 1]["type"]?.Value<string>();
            var amount = d[d.Count - 1]["amount"]?.Value<decimal?>();
            var balance = d[d.Count - 1]["balance"]?.Value<decimal?>();

            date.Should().NotBeNull();
            action.Should().Be("Sign up");
            type.Should().Be("Earned");
            amount.Should().Be(150);
            balance.Should().BeGreaterThan(0);
        }

        [Fact(Skip = "This is not good - what you want to accomplish")]
        public async Task PostAsync_Redeem()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/wallet/redeem", new StringContent("{amount:1000}", Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }

        [Fact(Skip = "This is not good - it is not the use case")]
        public async Task GetAsync_PaymentLink()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync("api/wallet/getpaymentlink");

            response.EnsureSuccessStatusCode();

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var link = d["link"]?.Value<string>();

            link.Should().NotBeNull();
        }
    }
}
