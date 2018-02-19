using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class DocumentControllerTests : ServerInit
    {
        [TestMethod]
        public async Task LinkToHeb_RedirectResult()
        {
            var response = await _client.GetAsync("/item/המרכז-האקדמי-לב-מכון-טל/25405/סימולציה/401065/consoleapplication1.xml").ConfigureAwait(false);
            var p = response.Headers.Location;
            Assert.IsTrue(p.AbsolutePath.EndsWith("/"));
        }
    }
}