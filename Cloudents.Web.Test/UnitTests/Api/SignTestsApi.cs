using System;
using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;


namespace Cloudents.Web.Test.UnitTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SignTestsApi //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        private readonly System.Net.Http.HttpClient _client;

        private readonly string _swaggerLink = "https://localhost/swagger";

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/register"
        };




        public SignTestsApi(SbWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Post_Login_With_Email()
        {
            await _client.LogInAsync();
        }

        [Fact]
        public async Task Post_Register_With_Email()
        {
            _client.DefaultRequestHeaders.Referrer = new Uri(_swaggerLink);

            var sign = new
            {
                email = "elad+6@cloudents.com",
                password = "123456789",
                confirmPassword = "123456789"
            };

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(sign));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get_Sms_Code()
        {
            var responseObject = new
            {
                code = 972
            };


            await _client.LogInAsync();

            _uri.Path = "api/sms/code";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            str.Should().Be(JsonConvert.SerializeObject(responseObject));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Post_Send_Sms_Code()
        {
            _client.DefaultRequestHeaders.Referrer = new Uri(_swaggerLink);

            var sign = new
            {
                email = "elad+6@cloudents.com",
                password = "123456789",
                confirmPassword = "123456789"
            };
            
            var phone = new
            {
                phoneNumber = "542473699",
                countryCode = "972"
            };

            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(sign));

            _uri.Path = "api/sms";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(phone));

            response.EnsureSuccessStatusCode();
        }
    }
}
