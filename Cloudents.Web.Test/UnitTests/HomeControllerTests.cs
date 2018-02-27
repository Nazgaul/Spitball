using Cloudents.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Web.Test.UnitTests
{
    /// <summary>
    /// Summary description for HomeControllerTests
    /// </summary>
    [TestClass]
    public class HomeControllerTests
    {
        public HomeControllerTests()
        {
            var configurationMock = new Mock<IConfiguration>();

            configurationMock.Setup(f => f["Ips"]).Returns("31.154.39.170");
            var controller = new HomeController(configurationMock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }
    }
}
