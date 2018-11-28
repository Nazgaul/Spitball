﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class SuggestApiTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public SuggestApiTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("api/Suggest?sentence=hi")]
        [InlineData("api/Suggest?sentence=hi&vertical=job")]
        [InlineData("api/Suggest?sentence=aj&vertical=tutor")]
        [InlineData("/api/suggest")]
        public async Task GetAsync_Ok(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}