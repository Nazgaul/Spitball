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
    public class TutoringTestsApi : IClassFixture<SbWebApplicationFactory>
    {

        private readonly SbWebApplicationFactory _factory;
        private readonly string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

        public TutoringTestsApi(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_Create_Room()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsync("api/tutoring/create", new StringContent("", Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Post_Review()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";;

            string review = "{\"review\":\"Good one\",\"rate\": 3,\"tutor\": 160116}";

            await client.PostAsync("api/Login", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/tutoring/review", new StringContent(review, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }
    }
}
