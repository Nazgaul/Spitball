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
    }
}
