using Cloudents.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Web.Test
{
    /// <summary>
    /// Summary description for HomeControllerTests
    /// </summary>
    [TestClass]
    public class HomeControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock;

        private readonly HomeController _controller;
        public HomeControllerTests()
        {
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(f => f["Ips"]).Returns("31.154.39.170");
            _controller = new HomeController(_configurationMock.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }
    }
}
