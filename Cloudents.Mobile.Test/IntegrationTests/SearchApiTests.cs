using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Mobile.Test.IntegrationTests
{
    [TestClass]
    public class SearchApiTests : ServerInit
    {
        [TestMethod]
        public async Task SearchDocumentAsync_OnlyFormat_Ok()
        {
            var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task SearchDocumentAsync_CheckCorsLocalHost_Ok()
        {
            Client.DefaultRequestHeaders.Add("Origin", "https://localhost");
            var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
            var header = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            header.Should().Be("https://localhost");
            //response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task SearchDocumentAsync_CheckCorsLocalHostPort_Ok()
        {
            Client.DefaultRequestHeaders.Add("Origin", "https://localhost:44345");
            var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
            var header = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            header.Should().Be("https://localhost:44345");
        }

        [TestMethod]
        public async Task SearchDocumentAsync_CheckCorsDev_Ok()
        {
            Client.DefaultRequestHeaders.Add("Origin", "https://dev.spitball.co");
            var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
            var header = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            header.Should().Be("https://dev.spitball.co");
        }

        [TestMethod]
        public async Task SearchDocumentAsync_CheckCorsProduction_Ok()
        {
            Client.DefaultRequestHeaders.Add("Origin", "https://www.spitball.co");
            var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
            var header = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            header.Should().Be("https://www.spitball.co");
        }


        [TestMethod]
        public async Task SearchDocumentAsync_CheckCorsSomeDomain_Invalid()
        {
            Client.DefaultRequestHeaders.Add("Origin", "https://www.microsoft.com");
            var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var p);

            p.Should().BeNull();
        }


    }
}