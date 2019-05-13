﻿using Xunit;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class DocumentApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;
        
        private readonly object _doc1 = new
        {
            mime_type = "",
            name = "ACloudFan.pdf",
            phase = "start",
            size = 1027962
        };
        private readonly object _doc2 = new
        {
            mime_type = "",
            name = "Capture 2.png",
            phase = "start",
            size = 1027962
        };
        private readonly object _doc3 = new
        {
            mime_type = "",
            name = "ספיטבול.docx",
            phase = "start",
            size = 1027962
        };
        private readonly object _doc4 = new
        {
            mime_type = "",
            name = "Doc4",
            phase = "start",
            size = 1027962
        };

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/document"
        };



        public DocumentApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("api/document")]
        [InlineData("/api/document?page=1")]
        public async Task GetAsync_OK(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            
            var result = d["result"]?.Value<JArray>();

            var next = d["nextPageLink"]?.Value<string>();

            result.Should().NotBeNull();

            if (url == _uri.Path + "?page=1")
                next.Should().Be(_uri.Path + "?page=2");
        }

        [Fact]
        public async Task GetAsync_Filters()
        {
            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var filters = d["filters"]?.Value<JArray>();

            var type = filters[0]["data"]?.Value<JArray>();

            filters.Should().NotBeNull();

            type.Should().HaveCountGreaterThan(3);
        }

        [Fact]
        public async Task PostAsync_Upload_Regular_FileName()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClient.CreateJsonString(_doc1));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_FileName_With_Space()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClient.CreateJsonString(_doc2));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_Hebrew_FileName()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClient.CreateJsonString(_doc3));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_Without_File_Extension()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClient.CreateJsonString(_doc4));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
