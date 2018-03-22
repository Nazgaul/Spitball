using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Api.Test.IntegrationTests
{
    [TestClass]
    public class TutorApiTests : ServerInit
    {
        [TestMethod]
        public async Task Search_QueryWithoutFilter_ReturnResult()
        {
            var response = await Client.GetAsync("/api/Tutor?term=ajax&sort=null&page=0&location.latitude=31.9155609&location.longitude=34.8049517").ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.Fail(content);
            }
        }


        [TestMethod]
        public async Task Search_QueryWithValidSort_ReturnResult()
        {
            var response = await Client.GetAsync("/api/Tutor?term=ajax&sort=price&page=0&location.latitude=31.9155609&location.longitude=34.8049517").ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.Fail(content);
            }
        }
    }
}