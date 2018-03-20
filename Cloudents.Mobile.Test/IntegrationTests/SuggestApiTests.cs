using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Mobile.Test.IntegrationTests
{
    [TestClass]
    public class SuggestApiTests :ServerInit
    {
        [TestMethod]
        public async Task GetAsync_NoVertical_Ok()
        {
           var response = await Client.GetAsync("api/Suggest?sentence=hi");
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task GetAsync_SomeVertical_Ok()
        {
            var response = await Client.GetAsync("api/Suggest?sentence=hi&vertical=job");
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task GetAsync_TutorVertical_Ok()
        {
            var response = await Client.GetAsync("api/Suggest?sentence=aj&vertical=tutor");
            response.EnsureSuccessStatusCode();
        }
    }
}