using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class FeedApiTests
    {
        private readonly System.Net.Http.HttpClient _client;
        public FeedApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        [Theory]
        [InlineData("/api/feed?page=0", FeedType.Document)]
        [InlineData("/api/feed?page=0", FeedType.Video)]
        [InlineData("/api/feed?page=0", FeedType.Tutor)]
        [InlineData("/api/feed?page=0", FeedType.Question)]
        [InlineData("/api/feed?page=0", null)]
        public async Task AggregateAllCoursesAsync_Ok(string url, FeedType? filter)
        {
            if (filter != null)
            {
                url += $"&filter={filter}";
            }

            var response = await _client.GetAsync(url);
            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            var result = d["result"]?.Value<JArray>();

            result.Should().NotBeNull();
            result.Should().OnlyHaveUniqueItems();
            result.Should().HaveCount(21);
        }

        [Theory]
        [InlineData("/api/feed?page=0", FeedType.Document, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Video, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Question, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Tutor, "Temp")]
        [InlineData("/api/feed?page=0", null, "Temp")]
        public async Task SpecificCourseAsync_Ok(string url, FeedType? filter, string course)
        {
            url += $"&course={course}";
            if (filter != null)
            {
                url += $"&filter={filter}";
            }

            var response = await _client.GetAsync(url);
            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            var result = d["result"]?.Value<JArray>();

            result.Should().NotBeNull();
            result.Should().OnlyHaveUniqueItems();

            if (filter == FeedType.Document || filter == FeedType.Video || filter == null)
            {
                result.Should().HaveCount(21);
            }
        }
    }
}
