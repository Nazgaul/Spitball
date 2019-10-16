﻿using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class QuestionsApiTests //:  IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "/api/question"
        };

        private readonly object _question = new
        {
            course = "Economics",
            text = "This is a testing question",
            files = ""
        };


        public QuestionsApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        //[Fact]
        //public async Task GetAsync_Filters()
        //{
        //    var response = await _client.GetAsync(_uri.Path);

        //    var str = await response.Content.ReadAsStringAsync();

        //    var d = JObject.Parse(str);

        //    var filters = d["filters"]?.Value<JArray>();
        //    var type = filters[0]["data"]?.Value<JArray>();
        //    //var subject = filters[1]["data"]?.Value<JArray>();

        //    filters.Should().HaveCount(1);
        //    type.Should().HaveCount(3);
        //    //subject.Should().HaveCount(24);
        //}

        //[Theory]
        //[InlineData("/api/Question?term=javascript:alert(219)")]
        //[InlineData("/api/Question?term=main() { int a%3D4%2Cb%3D2%3B a%3Db<<a %2B b>>2%3B printf(\"%25d\"%2C a)%3B } a) 32 b) 2 c) 4 d) none")]
        //public async Task GetAsync_QueryXss(string url)
        //{
        //    var response = await _client.GetAsync(url);
        //    response.EnsureSuccessStatusCode();
        //}

        [Theory]
        [InlineData("/api/Question/9339")]
        public async Task GetAsync_Url_Success(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var id = d["id"]?.Value<long?>();            
            var user = d["user"]?.Value<JObject>();            
            var rtl = d["isRtl"]?.Value<bool?>();
            //var vote = d["vote"]?.Value<JObject>();

            response.EnsureSuccessStatusCode();

            id.Should().NotBeNull();
            user.Should().NotBeNull();
            rtl.Should().NotBeNull();
           // vote.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_Not_Found()
        {
            var response = await _client.GetAsync(_uri.Path + "/123");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PostAsync_New_OK()
        {
            await _client.LogInAsync();
            
            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_question));
            
            response.EnsureSuccessStatusCode();
        }
    }


}