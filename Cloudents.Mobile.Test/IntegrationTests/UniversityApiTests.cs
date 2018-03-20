using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Api.Test.IntegrationTests
{
    [TestClass]
    public class UniversityApiTests : ServerInit
    {
        [TestMethod]
        public async Task GetAsync_SomeLocation_Ok()
        {
            var result = await Client.GetAsync("api/university?Location.Longitude=-74.005&Location.Latitude=40.712");
            result.EnsureSuccessStatusCode();
        }
    }
}