using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Api.Test.IntegrationTests
{
    [TestClass]
    public class AskControllerTests : ServerInit
    {
        [TestMethod]
        public async Task Get_Empty_Ok()
        {
            var response = await Client.GetAsync("api/ask").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}