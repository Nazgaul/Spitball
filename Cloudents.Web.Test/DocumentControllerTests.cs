using System;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Controllers;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Web.Test
{
    [TestClass]
    public class DocumentControllerTests
    {
        Mock<IStringLocalizer<Seo>> _mockLocalizer = new Mock<IStringLocalizer<Seo>>();
        Mock<IReadRepositoryAsync<DocumentSeoDto, long>> _mockRepository = new Mock<IReadRepositoryAsync<DocumentSeoDto, long>>();


        private DocumentController _controller;
        public DocumentControllerTests()
        {
            _controller = new DocumentController(_mockLocalizer.Object, _mockRepository.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Path = "/item/%D7%94%D7%90%D7%95%D7%A0%D7%99%D7%91%D7%A8%D7%A1%D7%99%D7%98%D7%94-%D7%94%D7%A4%D7%AA%D7%95%D7%97%D7%94/5365/%D7%9E%D7%A2%D7%91%D7%93%D7%94-%D7%91%D7%AA%D7%9B%D7%A0%D7%95%D7%AA-%D7%9E%D7%A2%D7%A8%D7%9B%D7%95%D7%AA-%D7%91%D7%A9%D7%A4%D7%AA-c/609395/check-dl-shelon1.pdf/";

        }

        [TestMethod]
        public async Task Index_RedirectToOldSite()
        {
            var result = await _controller.Index(609395,default);
            Assert.IsInstanceOfType(result, typeof(RedirectResult), "need redirect");

            if (result is RedirectResult result2)
            {
                Assert.AreEqual("https://heb.spitball.co" + _controller.ControllerContext.HttpContext.Request.Path, result2.Url);
            }
        }
    }
}
