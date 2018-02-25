using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class PlacesApiTests : ServerInit
    {
        [TestMethod]
        public async Task ReturnResult()
        {
            var response =
                await Client.GetAsync(
                    "/api/places?location.latitude=31.915606900000004&location.longitude=34.80483160000001&term=burger");
            response.EnsureSuccessStatusCode();
        }
    }
}