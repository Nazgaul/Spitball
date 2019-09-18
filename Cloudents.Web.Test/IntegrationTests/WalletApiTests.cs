using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;


namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class WalletApiTests
    {
        private System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/wallet/balance"
        };

        public WalletApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }


        [Fact]
        public async Task GetAsync_Balance()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();
        }

    }
}
