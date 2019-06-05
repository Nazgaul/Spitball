using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UnregFeedTests
    {
        private readonly System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder();
        

        public UnregFeedTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAsync_Ask_OK()
        {
            _uri.Path = "api/question";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            d.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_Note_OK()
        {
            _uri.Path = "api/document";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            d.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_Tutor_OK()
        {
            _uri.Path = "api/tutor/search";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            d.Should().NotBeNull();
        }
    }
}
