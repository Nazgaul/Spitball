using System.IO;
using System.Linq;
using System.Reflection;
using Cloudents.Web.Api;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    public class HelpControllerTests
    {
        private static Stream InitializeXmlFile()
        {
            var asm = Assembly.GetExecutingAssembly();
            const string xmlFile = "help.xml";
            var resource = $"Cloudents.Web.Test.UnitTests.{xmlFile}";
            return asm.GetManifestResourceStream(resource);
        }

        [Fact]
        public void ParseXmlDocument_ReturnFirstQuestion()
        {
           // var controller = new HelpController(_mock.Object);

            using (var xmlDoc = InitializeXmlFile())
            {
                var result = HelpController.PassXmlDoc(xmlDoc);

                var firstResult = result.First();

                Assert.Equal("What happened to the old Spitball and where did all my documents go?", firstResult.Question);
            }
        }
    }
}
