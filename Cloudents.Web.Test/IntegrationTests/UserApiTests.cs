using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class UserApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public UserApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/profile/304600")]
        public async Task GetAsync_OK(string url)
        {
            var client = _factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var id = d["id"]?.Value<long>();
            var score = d["score"]?.Value<int>();
            id.Should().NotBeNull();
            score.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_NotFound()
        {
            var client = _factory.CreateClient();
            
            HttpResponseMessage response = await client.GetAsync("api/profile/1");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("api/profile/159039/questions")]
        [InlineData("api/profile/159039/answers")]
        [InlineData("api/profile/159039/documents")]
        [InlineData("api/profile/159039/purchaseDocuments")]
        public async Task GetAsync_UserTabs_OK(string url)
        {
            var client = _factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            JArray obj = JArray.Parse(str);

            var id = obj.Children();
            
            id.Should().NotBeNull();
        }
    }
}
