using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Mobile.Test.IntegrationTests
{
    [TestClass]
    public class BookApiTests : ServerInit
    {
        [TestMethod]
        public async Task Search_Gibrish_ReturnResult()
        {
            var response = await Client.GetAsync("api/book/search?term=kjhgfd").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task Search_Gibrish2_ReturnResult()
        {
            var response = await Client.GetAsync("api/book/search?term=%2Cmnhbg").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task Search_WithSpacePage_ReturnResult()
        {
            var response = await Client.GetAsync("/api/book/search?page=2&term=super mario 64 ds").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task Buy_SomeIsbn_ReturnResult()
        {
            var response = await Client.GetAsync("/api/book/buy?isbn13=9781292099170").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}