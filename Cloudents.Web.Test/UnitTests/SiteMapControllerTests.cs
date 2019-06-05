using System.Xml;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    public class SiteMapControllerTests //:IClassFixture<SbWebApplicationFactory>
    {

        private readonly string sitemapLink = "https://dev.spitball.co/sitemap.xml";

    

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
