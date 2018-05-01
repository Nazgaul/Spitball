using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class AiControllerTests : ServerInit
    {
        [TestMethod]
        public async Task Get_EmptySentence_BadRequest()
        {
            var response = await Client.GetAsync("api/ai?sentence=").ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [TestMethod]
        public async Task Get_SpacesSentence_BadRequest()
        {
            var response = await Client.GetAsync("api/ai?sentence=      ").ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}