﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class FeedControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public FeedControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }


        [Theory]
        [InlineData("nice123", false, 0)]
        [InlineData("physics 20II", false, 0)]
        [InlineData("nice123", true, 0)]
        [InlineData("physics 20II", true, 0)]

        public async Task GetAsync_Search_Without_Results(string cousre, bool logIn, int page)
        {

            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync($"api/feed?Course={cousre}&page={page}");
            response.EnsureSuccessStatusCode();
            //var str = await response.Content.ReadAsStringAsync();

            //str.Should().BeEmpty();
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 0)]
        [InlineData(false, 10000)]
        [InlineData(true, 10000)]
        public async Task GetAsync_AggregateAllCourses_Ok(bool logIn, int page)
        {
            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync($"api/feed?page={page}");
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("econ", false, 0)]
        [InlineData("econ", true, 0)]
        [InlineData("this is a long term without results", false, 0)]
        [InlineData("this is a long term without results", true, 0)]
        public async Task GetAsync_Search_WithTerm_Ok(string term, bool logIn, int page)
        {
            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync($"api/feed?term={term}&page={page}");
            response.EnsureSuccessStatusCode();
        }


        [Theory]
        [InlineData("econ", "Economics", false, 0)]
        [InlineData("econ", "Economics", true, 0)]
        [InlineData("this is a long term without results", "nice123", false, 0)]
        [InlineData("this is a long term without results", "nice123", true, 0)]
        public async Task GetAsync_Search_WithTermAndCourse_Ok(string term, string course, bool logIn, int page)
        {
            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync($"api/feed?term={term}&page={page}&course={course}");
            response.EnsureSuccessStatusCode();
        }

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/document"
        };

        [Theory]
        [InlineData("api/feed", false)]
        [InlineData("/api/feed?page=1", false)]
        [InlineData("api/feed", true)]
        [InlineData("/api/feed?page=1", true)]
        public async Task GetAsync_OK(string url, bool authUser)
        {
            if (authUser)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();

            var next = d["nextPageLink"]?.Value<string>();

            result.Should().NotBeNull();

            if (url == _uri.Path + "?page=1")
                next.Should().Be(_uri.Path + "?page=2");
        }
    }
}
