﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class QuestionsApiTests :  IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public QuestionsApiTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/Question?term=javascript:alert(219)")]
        [InlineData("/api/Question?term=main() { int a%3D4%2Cb%3D2%3B a%3Db<<a %2B b>>2%3B printf(\"%25d\"%2C a)%3B } a) 32 b) 2 c) 4 d) none")]
        public async Task GetAsync_QueryXss(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}