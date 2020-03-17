using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SalesControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;
        public SalesControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("api/Sales/sales")]
        public async Task SalesTestGet_OkAsync(string url)
        {
            await _client.LogInAsync();
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            str.IsValidJson().Should().BeTrue();
        }

        [Fact]
        public async Task SessionTestGet_OkAsync()
        {
            await _client.LogInAsync();
            var response = await _client.GetAsync("api/sales/session?id=29FA48E7-65E0-4E4F-9916-AB1E00A8BC8B");
            response.EnsureSuccessStatusCode();
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                var str = await response.Content.ReadAsStringAsync();
                str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
            }
        }
    }
}
