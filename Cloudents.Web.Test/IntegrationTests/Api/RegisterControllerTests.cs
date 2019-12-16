﻿using System;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class RegisterControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private readonly string _swaggerLink = "https://localhost:44345/swagger";

/*
        private readonly object _cred = new
        {
            confirmPassword = "123456789",
            email = "elad+99@cloudents.com",
            password = "123456789"
        };
*/

        //private UriBuilder _uri = new UriBuilder()
        //{
        //    Path = "api/register"
        //};




        public RegisterControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Referrer = new Uri(_swaggerLink);

        }

        [Fact]
        public async Task Post_Login_With_Email()
        {
            await _client.LogInAsync();
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
