using Cloudents.Web.Test.IntegrationTests;
using System.Xml;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    public class SiteMapControllerTests :IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public SiteMapControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void GetAsync_Sitemap()
        {
            XmlDocument sm = new XmlDocument();

            sm.Load("https://dev.spitball.co/sitemap.xml");
            XmlElement root = sm.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("sitemap");
            
        }
    }
}
