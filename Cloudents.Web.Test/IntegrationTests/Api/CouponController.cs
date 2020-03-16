using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{

    [Collection(SbWebApplicationFactory.WebCollection)]
    public class CouponController //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public CouponController(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("api/coupon")]
        public async Task Get_SomeCourse_ReturnResultAsync(string url)
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var str = await response.Content.ReadAsStringAsync();

            str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
        }
    }
}