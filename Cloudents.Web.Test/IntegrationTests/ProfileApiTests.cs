using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class ProfileApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/profile"
        };


        public ProfileApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Get_About_Regular_Profile()
        {
            var response = await _client.GetAsync(_uri.Path + "/160171/about");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var courses = d["courses"]?.Value<JArray>();

            var reviews = d["reviews"]?.Value<JArray>();

            courses.Should().NotBeNull();
            reviews.Should().NotBeNull();
        }

        [Theory]
        [InlineData("/api/profile/159039")]
        [InlineData("/api/profile/160171")]
        public async Task GetAsync_OK(string url)
        {
            var response = await _client.GetAsync(url);
            
            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var id = d["id"]?.Value<long?>();
            var name = d["name"]?.Value<string>();
            //var university = d["universityName"]?.Value<string>();
            var score = d["score"]?.Value<int?>();

            id.Should().BeGreaterThan(0);
            name.Should().NotBeNull();
            score.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task GetAsync_NotFound()
        {
            var response = await _client.GetAsync(_uri.Path + "/1");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("questions")]
        [InlineData("answers")]
        [InlineData("documents")]
        [InlineData("purchaseDocuments")]
        public async Task GetAsync_UserTabs_OK(string tab)
        {
            var response = await _client.GetAsync(_uri.Path + "/159039/" + tab);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();

            JArray obj = JArray.Parse(str);

            var id = obj.Children();

            id.Should().NotBeNull();
        }

        [Fact]
        public async Task Get_About_Tutor_Profile()
        {
            var response = await _client.GetAsync(_uri.Path + "/159039/about");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var courses = d["courses"]?.Value<JArray>();

            var bio = d["bio"]?.Value<string>();

            var reviews = d["reviews"]?.Value<JArray>();

            courses.Should().NotBeNull();
            bio.Should().NotBeNull();
            reviews.Should().NotBeNull();
        }
    }
}
