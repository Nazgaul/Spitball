using Xunit;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class DocumentApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;


        public DocumentApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("api/document")]
        [InlineData("/api/document?page=1")]
        public async Task GetAsync_OK(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();
            var filters = d["filters"]?.Value<JArray>();
            var type = filters[0]["data"]?.Value<JArray>();
            var next = d["nextPageLink"]?.Value<string>();

            result.Should().NotBeNull();
            filters.Should().NotBeNull();
            type.Should().HaveCountGreaterThan(3);
        }

        [Fact]
        public async Task GetAsync_Filters()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/document");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var filters = d["filters"]?.Value<JArray>();
            var type = filters[0]["data"]?.Value<JArray>();

            filters.Should().NotBeNull();
            type.Should().HaveCountGreaterThan(3);
        }
    }
}
