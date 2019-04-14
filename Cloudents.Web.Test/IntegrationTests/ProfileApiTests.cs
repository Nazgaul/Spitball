using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class ProfileApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public ProfileApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_About_Tutor_Profile()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/profile/159039/about");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var courses = d["courses"]?.Value<JArray>();

            var bio = d["bio"]?.Value<string>();

            var reviews = d["reviews"]?.Value<JArray>();

            courses.Should().NotBeNull();
            bio.Should().NotBeNull();
            reviews.Should().NotBeNull();
        }

        [Fact]
        public async Task Get_About_Regular_Profile()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/profile/160171/about");

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
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var uni = d["universityName"]?.Value<string>();
            var id = d["id"]?.Value<long?>();
            var name = d["name"]?.Value<string>();
            var score = d["score"]?.Value<int?>();

            id.Should().BeGreaterThan(0);
            name.Should().NotBeNull();
            score.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task GetAsync_NotFound()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("api/profile/1");

            //var str = await response.Content.ReadAsStringAsync();

            //var d = JObject.Parse(str);

            //var status = d["status"]?.Value<int?>();

            response.StatusCode.Should().Be(302);
        }

        [Theory]
        [InlineData("api/profile/159039/questions")]
        [InlineData("api/profile/159039/answers")]
        [InlineData("api/profile/159039/documents")]
        [InlineData("api/profile/159039/purchaseDocuments")]
        public async Task GetAsync_UserTabs_OK(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            JArray obj = JArray.Parse(str);

            var id = obj.Children();

            id.Should().NotBeNull();
        }
    }
}
