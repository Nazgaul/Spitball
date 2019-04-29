using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SignTestsApi //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;
        private readonly string cred = "{\"email\":\"blah@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

        public SignTestsApi(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_Login_With_Email()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Post_Register_With_Email()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Referrer = new Uri("https://localhost/swagger");

            string sign = "{\"email\":\"elad+6@cloudents.com\",\"password\":\"123456789\",\"confirmPassword\":\"123456789\"}";

            var response = await client.PostAsync("api/register", new StringContent(sign, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Get_Sms_Code()
        {
            var client = _factory.CreateClient();

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.GetAsync("api/sms/code");

            var str = await response.Content.ReadAsStringAsync();

            str.Should().Be("{\"code\":\"972\"}");
            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Post_Send_Sms_Code()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Referrer = new Uri("https://localhost/swagger");
            string sign = "{  \"email\": \"elad+6@cloudents.com\",  \"password\": \"123456789\",  \"confirmPassword\": \"123456789\"}";
            
            string phone = "{\"phoneNumber\":\"542473699\",\"countryCode\":\"972\"}";

            await client.PostAsync("api/register", new StringContent(sign, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Sms", new StringContent(phone, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }
    }
}
