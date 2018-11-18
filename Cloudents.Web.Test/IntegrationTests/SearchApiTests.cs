using System.Net;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Web.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class SearchApiTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public SearchApiTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/Search/documents?Format=none")]
        [InlineData("/api/search/document")]
        [InlineData("/api/search/flashcards")]
        [InlineData("api/search/documents?query=>\"'><script>alert(72)<%2Fscript>&university=>\"'><script>alert(72)<%2Fscript>")]
        public async Task SearchDocumentAsync_Ok(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }

        //[TestMethod]
        //public async Task SearchDocumentAsync_CheckCorsLocalHost_Ok()
        //{
        //    Client.DefaultRequestHeaders.Add("Origin", "https://localhost");
        //    var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
        //    var header = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        //    header.Should().Be("https://localhost");
        //    //response.EnsureSuccessStatusCode();
        //}

        //[TestMethod]
        //public async Task SearchDocumentAsync_CheckCorsLocalHostPort_Ok()
        //{
        //    Client.DefaultRequestHeaders.Add("Origin", "https://localhost:44345");
        //    var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
        //    var header = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        //    header.Should().Be("https://localhost:44345");
        //}

        //[TestMethod]
        //public async Task SearchDocumentAsync_CheckCorsDev_Ok()
        //{
        //    Client.DefaultRequestHeaders.Add("Origin", "https://dev.spitball.co");
        //    var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
        //    var header = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        //    header.Should().Be("https://dev.spitball.co");
        //}

        //[TestMethod]
        //public async Task SearchDocumentAsync_CheckCorsProduction_Ok()
        //{
        //    Client.DefaultRequestHeaders.Add("Origin", "https://www.spitball.co");
        //    var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
        //    var header = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        //    header.Should().Be("https://www.spitball.co");
        //}


        //[TestMethod]
        //public async Task SearchDocumentAsync_CheckCorsSomeDomain_Invalid()
        //{
        //    Client.DefaultRequestHeaders.Add("Origin", "https://www.microsoft.com");
        //    var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
        //    response.Headers.TryGetValues("Access-Control-Allow-Origin", out var p);

        //    p.Should().BeNull();
        //}


    }
}