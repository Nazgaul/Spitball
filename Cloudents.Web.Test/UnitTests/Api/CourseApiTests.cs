using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests.Api
{
    public class CourseApiTests : IClassFixture<SbWebApplicationFactory>
    {

        private readonly SbWebApplicationFactory _factory;

        public CourseApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostAsync_Course()
        {
            var client = _factory.CreateClient();

            string crad = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            string course = "[{\"name\":\"Social\"}]";

            var response = await client.PostAsync("/api/login", new StringContent(crad, Encoding.UTF8, "application/json"));

            response = await client.PostAsync("/api/course", new StringContent(course, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetAsync_Course()
        {
            var client = _factory.CreateClient();

            string crad = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            var response = await client.PostAsync("/api/login", new StringContent(crad, Encoding.UTF8, "application/json"));

            response = await client.GetAsync("/api/course/search?term=eco");

            response.StatusCode.Should().Be(200);
        }

    }
}
