﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class UserApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public UserApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/profile/159039")]
        [InlineData("/api/profile/160171")]
        public async Task GetAsync_OK(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var uni = d["universityName"]?.Value<string>();
            var id = d["id"]?.Value<long?>();
            var name = d["name"]?.Value<string>();
            var score = d["score"]?.Value<int?>();

            id.Should().BeGreaterThan(0);
            name.Should().NotBeNull();
            score.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task GetAsync_NotFound()
        {
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync("api/profile/1");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var status = d["status"]?.Value<int?>();

            status.Should().Be(404);
        }

        [Theory]
        [InlineData("api/profile/159039/questions")]
        [InlineData("api/profile/159039/answers")]
        [InlineData("api/profile/159039/documents")]
        [InlineData("api/profile/159039/purchaseDocuments")]
        public async Task GetAsync_UserTabs_OK(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            JArray obj = JArray.Parse(str);

            var id = obj.Children();
            
            id.Should().NotBeNull();
        }
    }
}