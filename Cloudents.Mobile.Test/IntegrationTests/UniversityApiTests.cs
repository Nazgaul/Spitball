using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Mobile.Test.IntegrationTests
{
    [TestClass]
    public class UniversityApiTests : ServerInit
    {
        [TestMethod]
        public async Task ByApproximateAsync_SomeLocation_Ok()
        {
            var result = await Client.GetAsync("api/university/approximate?point.Longitude=-74.005&point.Latitude=40.712");
            result.EnsureSuccessStatusCode();
        }
    }
}