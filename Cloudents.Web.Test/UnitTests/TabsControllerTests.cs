using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TabsControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public TabsControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("flashcard")]
        [InlineData("tutor")]
        [InlineData("book")]
        [InlineData("job")]
        public async Task GetAsync_Redirect_ToHomePage(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync(url);

            response.StatusCode.Should().Be(302);
            response.Headers.Location.Should().Be("/");
        }

        [Fact]
        public async Task GetAsync_StudyDoc()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/note");

            response.EnsureSuccessStatusCode();
            response.RequestMessage.RequestUri.Should().Be("http://localhost/note");
        }

        [Fact]
        public async Task GetAsync_HW()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/ask");

            response.EnsureSuccessStatusCode();
            response.RequestMessage.RequestUri.Should().Be("http://localhost/ask");
        }
    }
}
