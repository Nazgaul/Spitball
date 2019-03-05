using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
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
        [InlineData("document")]
        [InlineData("document?page=1")]
        public async Task GetAsync_OK(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            HttpResponseMessage response = await client.GetAsync(url);

            /*var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var details = d["details"]?.Value<string>();
            var id = d["id"]?.Value<long>();
            var score = d["score"]?.Value<int>();
            id.Should().NotBeNull();
            score.Should().NotBeNull();*/
        }
    }
}