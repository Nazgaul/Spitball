﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UnregFeedTests
    {
        private readonly System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder();
        

        public UnregFeedTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("api/question")]
        [InlineData("api/question?page=1")]
        public async Task GetAsync_Ask_OK(String url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();

            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData("api/document")]
        [InlineData("api/document?page=1")]
        public async Task GetAsync_Note_OK(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();

            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData("api/tutor/search")]
        [InlineData("api/tutor/search?page=1")]
        public async Task GetAsync_Tutor_OK(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();

            result.Should().NotBeNull();
        }
    }
}
