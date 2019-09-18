using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class LogIn
    {
        private readonly System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/LogIn"
        };

        private readonly object user = new
        {
            email = "elad13@cloudents.com",
            password = "123456789"
        };

        private readonly object wrongUser = new
        {
            email = "elad13@cloudents.com",
            password = "abcdefgh"
        };


        public LogIn(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        [Fact]
        public async Task PostAsync_Login_OK()
        {
            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(user));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Login_Fail()
        {
            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(wrongUser));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAsync_Validate_Email()
        {
            _uri.Path = "api/login";

            var response = await _client.GetAsync(_uri.Path + "/ValidateEmail?email=elad%40cloudents.com");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_Non_Validate_Email()
        {
            _uri.Path = "api/login";

            var response = await _client.GetAsync(_uri.Path + "/ValidateEmail?email=fsdfs%40cloudents.com");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            //var email = d["Email"]?.Value<JArray>();

            var error = d["Email"][0].Value<string>();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            error.Should().Be("Account cannot be found, please sign up");
        }
    }
}
