using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        Mock<IIpToLocation> _mock;
        private Mock<IConfiguration> _configurationMock;


        private HomeController _controller;
        public HomeControllerTests()
        {
            _mock = new Mock<IIpToLocation>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new HomeController(_mock.Object, _configurationMock.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

        }

        [TestMethod]
        public async Task Index_OfficeIP_ReturnViewAsync()
        {
            var ip = IPAddress.Parse("31.154.39.170");
            _controller.ControllerContext.HttpContext.Request.Path = "/classnotes";
            _controller.ControllerContext.HttpContext.Request.Host = new HostString("www.spitball.co");
            _controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = ip;
            _mock.Setup(s => s.GetAsync(ip, default)).Returns(Task.FromResult(new IpDto
            {
                CountryCode = "IL"
            }));

            var result = await _controller.Index(default);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Index_ILIP_ReturnRedirectToOldSiteAsync()
        {
            var ip = IPAddress.Parse("31.154.39.150");
            _controller.ControllerContext.HttpContext.Request.Path = "/classnotes";
            _controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = ip;
            _mock.Setup(s => s.GetAsync(ip, default)).Returns(Task.FromResult(new IpDto
            {
                CountryCode = "IL"
            }));

            var result = await _controller.Index(default);


            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            if (result is RedirectResult result2)
            {
                Assert.AreEqual("https://heb.spitball.co/classnotes", result2.Url);
            }

        }


        [TestMethod]
        public async Task Index_USIP_ReturnRedirectToOldSiteAsync()
        {
            var ip = IPAddress.Parse("31.154.39.150");
            _controller.ControllerContext.HttpContext.Request.Path = "/classnotes";
            _controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = ip;
            _mock.Setup(s => s.GetAsync(ip, default)).Returns(Task.FromResult(new IpDto
            {
                CountryCode = "US"
            }));

            var result = await _controller.Index(default);


            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            if (result is RedirectToRouteResult result2)
            {
                Assert.AreEqual("alex", result2.RouteName,true);
            }

        }
    }
}
