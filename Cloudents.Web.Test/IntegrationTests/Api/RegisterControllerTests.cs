using System;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Web.Models;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class RegisterControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private const string _swaggerLink = "https://localhost:44345/swagger";


        public RegisterControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            
            _client.DefaultRequestHeaders.Referrer = new Uri(_swaggerLink);

        }



        [Fact]
        public async Task PostAsync_DuplicateEmail_BadResult()
        {
            
            var model = new RegisterRequest
            {
                FirstName = "xxx",
                LastName = "yyy",
                Email = "ram@cloudents.com", // TODO: not good hard coded
                Gender = Gender.Female,
                Password = "123123123",
                Captcha = "SomeCaptcha"
            };

            var jsonString = HttpClientExtensions.CreateJsonString(model);
            var result = await _client.PostAsync("api/register", jsonString);
            result.StatusCode.Should().Be(400);

        }

        //[Fact]
        //public async Task Post_Register_With_Email()
        //{
        //    var sign = new
        //    {
        //        email = "elad+99@cloudents.com",
        //        password = "123456789",
        //        confirmPassword = "123456789"
        //    };

        //    var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(sign));

        //    response.EnsureSuccessStatusCode();
        //}

        //[Fact]
        //public async Task PostAsync_Resend_Email()
        //{
        //    await _client.PostAsync("api/Register", HttpClient.CreateJsonString(_cred));

        //    _uri.Path = "api/register/resend";

        //    var response = await _client.PostAsync(_uri.Path, HttpClient.CreateString("{}"));

        //    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //}
    }
}
