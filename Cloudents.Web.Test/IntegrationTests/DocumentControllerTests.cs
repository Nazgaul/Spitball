using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class DocumentControllerTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public DocumentControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("document/המסלול-האקדמי-המכללה-למנהל")]
        public async Task ShortUrl_Invalid_404(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync(url);
           
            var p = response.Headers.Location;
            p.Should().Be("/Error/NotFound");
            //Assert.EndsWith("error/notfound", p.AbsolutePath);
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