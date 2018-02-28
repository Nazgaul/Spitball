using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Mobile.Test.IntegrationTests
{
    [TestClass]
    public class TutorApiTests : ServerInit
    {
        [TestMethod]
        public async Task Search_QueryWithoutFilter_ReturnResult()
        {
            var response = await Client.GetAsync("/api/Tutor?term=financial%20accounting&sort=null&page=0&location.latitude=31.9155609&location.longitude=34.8049517").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}