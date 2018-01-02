using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Cloudents.Core.Storage;
using Cloudents.Web.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Web.Test
{
    [TestClass]
    public class HelpControllerTests
    {
        readonly Mock<IBlobProvider> _mock = new Mock<IBlobProvider>();


        private static Stream InitializeXmlFile()
        {
            var asm = Assembly.GetExecutingAssembly();
            var xmlFile = "help.xml";
            var resource = $"Cloudents.Web.Test.{xmlFile}";
            return asm.GetManifestResourceStream(resource);

        }

        [TestMethod]
        public void ParseXmlDocument_ReturnFirstQuestion()
        {
           // var controller = new HelpController(_mock.Object);

            using (var xmlDoc = InitializeXmlFile())
            {
                var result = HelpController.PassXmlDoc(xmlDoc);

                var firstResult = result.First();

                Assert.AreEqual(firstResult.Question, "What happened to the old Spitball and where did all my documents go?");


            }
        }
    }
}
