using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class AccountControllerTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;
        private readonly HttpClient client;

        public AccountControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
            client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAsync_Unauthorized_401()
        {
            var response = await client.GetAsync("api/account");

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task GetAsync_OK_200()
        {
            string crad = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\"}";

            var response = await client.PostAsync("api/LogIn", new StringContent(crad, Encoding.UTF8, "application/json"));

            response = await client.GetAsync("api/account");

            response.StatusCode.Should().Be(200);
        }
    }
}
