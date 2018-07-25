using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class QuestionsApiTests : ServerInit
    {
        [TestMethod]
        public async Task GetAsync_QueryXss()
        {
            var response = await Client.GetAsync("/api/Question?term=javascript:alert(219)").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
       
    }
}