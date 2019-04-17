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
        private readonly SbWebApplicationFactory _factory;

        public WalletControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAsync_Balance()
        {
            var client = _factory.CreateClient();
            string[] types = { "Earned", "Stake", "Spent" };
            
            await client.LogInAsync();

            var response = await client.GetAsync("api/wallet/balance");

            var str = await response.Content.ReadAsStringAsync();

            var d = JArray.Parse(str);

            for (int i = 0; i < 3; i++)
            {
                var type = d[i]["type"]?.Value<string>();
                var name = d[i]["name"]?.Value<string>();
                var points = d[i]["points"]?.Value<decimal?>();
                type.Should().Be(types[i]);
                name.Should().Be(types[i]);
                points.Should().NotBeNull();
            }            
        }

        [Fact]
        public async Task GetAsync_Transaction()
        {
            var client = _factory.CreateClient();

            await client.LogInAsync();
            var response = await client.GetAsync("api/wallet/transaction");

            var str = await response.Content.ReadAsStringAsync();

            var d = JArray.Parse(str);

            var date = d[d.Count-1]["date"]?.Value<DateTime?>();
            var action = d[d.Count-1]["action"]?.Value<string>();
            var type = d[d.Count-1]["type"]?.Value<string>();
            var amount = d[d.Count-1]["amount"]?.Value<decimal?>();
            var balance = d[d.Count-1]["balance"]?.Value<decimal?>();

            date.Should().NotBeNull();
            action.Should().Be("Sign up");
            type.Should().Be("Earned");
            amount.Should().Be(150);
            balance.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task PostAsync_Redeem()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Wallet/redeem", new StringContent("{amount:1000}", Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
    }
}
