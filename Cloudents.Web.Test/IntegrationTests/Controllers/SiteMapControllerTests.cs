using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Controllers
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SiteMapControllerTests //:IClassFixture<SbWebApplicationFactory>
    {
        public SiteMapControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }


        private readonly System.Net.Http.HttpClient _client;

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetAsync_SiteMapAsync(bool isFrymo)
        {
            var url = "sitemap.xml";
            if (isFrymo)
            {
                url += "?site=frymo";
            }
            var result = await _client.GetStringAsync(url);
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var doc = XDocument.Parse(result);


            foreach (var xElement in doc.Descendants(ns + "loc"))
            {
                //foreach (var url in xElement.Value)
                //{
                var result2 = await _client.GetAsync(xElement.Value);
                result2.EnsureSuccessStatusCode();
                var str = await result2.Content.ReadAsStringAsync();
                var doc2 = XDocument.Parse(str);
                // }

            }

        }
    }
}
