using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.MobileApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Mobile.Test
{
    [TestClass]
    public class AiControllerTests
    {
        readonly Mock<IEngineProcess> _mock = new Mock<IEngineProcess>();

        [TestMethod]
        public async Task GetAsync_NullSentence_BadRequest()
        {
            var controller = new AiController(_mock.Object);
            var result = await controller.GetAsync(null, CancellationToken.None);
            Assert.IsInstanceOfType(result,typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetAsync_EmptySpaces_BadRequest()
        {
            var controller = new AiController(_mock.Object);
            var result = await controller.GetAsync("        ", CancellationToken.None);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
    }
}