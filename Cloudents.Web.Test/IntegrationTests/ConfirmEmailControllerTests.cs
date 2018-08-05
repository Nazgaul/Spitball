using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class ConfirmEmailControllerTests : ServerInit
    {
        [TestMethod]
        public async Task Get_WrongLongId_500Page()
        {
            var url =
                "/ConfirmEmail?Id=1013 and 3064%3d3064&code=CfDJ8GwX8MRfevpGmOMc9ZOhCGPS2rnhH%2BTS9R5O9FC2UXZ5Xnw%2BbHKrDwKlyKoIAY5ni78hQFwu3hTro16UrzKLGnsK53D9IWY8xB%2FgJgnRL%2FJuetfe9VIMwFtg1IKBk9TKGCDRhu75WomXbmCKnwTYGDFjosNloE4%2FfBnzotk1mhdcmrsv8LQV1Kh0bO47wuENz4FQGcFdzc43FZ%2FhlEszgKU%3D";
            var response = await Client.GetAsync(url).ConfigureAwait(false);
            var p = response.Headers.Location;
            Assert.IsTrue(p.OriginalString == "/");
        }
    }
}