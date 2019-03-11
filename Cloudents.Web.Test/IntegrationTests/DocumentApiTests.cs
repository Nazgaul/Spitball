﻿using Xunit;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class DocumentApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;


        public DocumentApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("api/document")]
        [InlineData("/api/document?page=1")]
        public async Task GetAsync_OK(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();
            var filters = d["filters"]?.Value<JArray>();
            var type = filters[0]["data"]?.Value<JArray>();
            var next = d["nextPageLink"]?.Value<string>();

            var id = result[0]["id"]?.Value<long?>();
            var university = result[0]["university"]?.Value<string>();
            var course = result[0]["course"]?.Value<string>();
            var title = result[0]["title"]?.Value<string>();
            var user = result[0]["user"]?.Value<JObject>();
            var views = result[0]["views"]?.Value<int?>();
            var downloads = result[0]["downloads"]?.Value<int?>();
            var docUrl = result[0]["url"]?.Value<string>();
            var source = result[0]["source"]?.Value<string>();
            var dateTime = result[0]["dateTime"]?.Value<DateTime?>();
            var vote = result[0]["vote"]?.Value<JObject>();
            var price = result[0]["price"]?.Value<double?>();

            result.Should().NotBeNull();
            filters.Should().NotBeNull();
            type.Should().HaveCountGreaterThan(3);

            id.Should().NotBeNull();
            university.Should().NotBeNull();
            course.Should().NotBeNull();
            title.Should().NotBeNull();
            user.Should().NotBeNull();
            views.Should().NotBeNull();
            downloads.Should().NotBeNull();
            docUrl.Should().NotBeNull();
            source.Should().NotBeNull();
            dateTime.Should().NotBeNull();
            vote.Should().NotBeNull();
            price.Should().BeGreaterOrEqualTo(0);

            if (url == "/api/document?page=1")
                next.Should().Be("http://localhost:80/api/Document?Page=2");
        }

        [Fact]
        public async Task GetAsync_Filters()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/document");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var filters = d["filters"]?.Value<JArray>();
            var type = filters[0]["data"]?.Value<JArray>();

            filters.Should().NotBeNull();
            type.Should().HaveCountGreaterThan(3);
        }
    }
}