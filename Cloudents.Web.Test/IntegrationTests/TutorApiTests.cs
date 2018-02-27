using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class TutorApiTests : ServerInit
    {
        

        [TestMethod]
        public async Task ReturnResult()
        {
            var response = await Client.GetAsync("/api/Tutor?term=financial accounting&sort=null&page=0").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}