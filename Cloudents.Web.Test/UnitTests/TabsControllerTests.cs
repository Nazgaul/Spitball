using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TabsControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        private readonly System.Net.Http.HttpClient _client;

        private readonly UriBuilder _uri = new UriBuilder()
        {
            Path = "/note"
        };


        public TabsControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("flashcard")]
        [InlineData("book")]
        [InlineData("job")]
        public async Task GetAsync_Redirect_ToHomePage(string url)
        {
            var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            response.Headers.Location.Should().Be("/");
        }

        [Fact]
        public async Task GetAsync_StudyDoc()
        {
            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();

            response.RequestMessage.RequestUri.Should().Be(_uri.Host + _uri.Path);
        }

        [Fact]
        public async Task GetAsync_HW()
        {
            _uri.Path = "/ask";

            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();

            response.RequestMessage.RequestUri.Should().Be(_uri.Host + _uri.Path);
        }
    }
}
