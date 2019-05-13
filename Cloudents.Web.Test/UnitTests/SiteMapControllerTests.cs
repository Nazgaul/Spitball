using Cloudents.Web.Test.IntegrationTests;
using System.Xml;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SiteMapControllerTests //:IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        private readonly string sitemapLink = "https://dev.spitball.co/sitemap.xml";

        public SiteMapControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void GetAsync_Sitemap()
        {
            XmlDocument sm = new XmlDocument();

            sm.Load(sitemapLink);
            XmlElement root = sm.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("sitemap");
            
        }
    }
}
