using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests 
{
    public class BookControllerTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;
        private readonly HttpClient client;

        public BookControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
            client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAsync_Redirect_To_Default()
        {
            var response = await client.GetAsync("book");

            var p = response.Headers.Location;

            Assert.True(p.OriginalString == "/");
        }

        [Fact]
        public async Task GetAsync_OK_200()
        {
            var response = await client.GetAsync("book");

            string crad = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\"}";

            response = await client.PostAsync("api/LogIn", new StringContent(crad, Encoding.UTF8, "application/json"));

            response = await client.GetAsync("book");

            response.StatusCode.Should().Be(200);
        }
    }
}
