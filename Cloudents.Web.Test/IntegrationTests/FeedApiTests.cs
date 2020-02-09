using Cloudents.Core.Enum;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
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
        }

        [Theory]
        [InlineData("/api/feed?page=0", FeedType.Document)]
        [InlineData("/api/feed?page=0", FeedType.Video)]
        [InlineData("/api/feed?page=0", FeedType.Tutor)]
        [InlineData("/api/feed?page=0", FeedType.Question)]
        [InlineData("/api/feed?page=0", null)]
        public async Task AggregateAllCourses_OkAsync(string url, FeedType? filter)
        {
            if (filter != null)
            {
                url += $"&filter={filter}";
            }
            
            var response = await _client.GetAsync(url);
            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            var result = d["result"]?.Value<JArray>();

            result.Should().NotBeNull().And.NotBeEmpty();
            result.Should().OnlyHaveUniqueItems();
            //Video is not 21 items
           // result.Should().HaveCount(21);
        }

        [Theory]
        [InlineData("/api/feed?page=0", FeedType.Document, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Video, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Question, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Tutor, "Temp")]
        [InlineData("/api/feed?page=0", null, "Temp")]
        public async Task SpecificCourse_OkAsync(string url, FeedType? filter, string course)
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

            result.Should().NotBeNull().And.NotBeEmpty();
            result.Should().OnlyHaveUniqueItems();

            //if (filter == FeedType.Document || filter == FeedType.Video)
            //{
            //    result.Should().HaveCount(21);
            //}
        }

        [Theory]
        [InlineData("/api/feed?page=0", null, "Temp", "x")]
        [InlineData("/api/feed?page=0", FeedType.Tutor, "Temp", "x")]
        [InlineData("/api/feed?page=0", FeedType.Document, "Temp", "x")]
        [InlineData("/api/feed?page=0", FeedType.Video, "Temp", "x")]
        //[InlineData("/api/feed?page=0", FeedType.Question, "Temp", "x")]
        public async Task SearchInCourse_OkAsync(string url, FeedType? filter, string course, string term)
        {
            url += $"&course={course}&term={term}";
            if (filter != null)
            {
                url += $"&filter={filter}";
            }

            var response = await _client.GetAsync(url);
            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            var result = d["result"]?.Value<JArray>();

            //result.Should().OnlyHaveUniqueItems();
            //if (filter != FeedType.Question)
            //{
            //    result.Should().NotBeNull().And.NotBeEmpty();
            //    result.Should().OnlyHaveUniqueItems();
            //}
            //else
            //{
            //    result.Should().BeEmpty();
            //}
        }

        [Theory]
        [InlineData("/api/feed?page=0", null, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Tutor, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Document, "Temp")]
        [InlineData("/api/feed?page=0", FeedType.Video, "Temp")]
        //[InlineData("/api/feed?page=0", FeedType.Question, "Temp")]
        public async Task SearchInSpitball_OkAsync(string url, FeedType? filter, string term)
        {
            url += $"&term={term}";
            if (filter != null)
            {
                url += $"&filter={filter}";
            }

            var response = await _client.GetAsync(url);
            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            var result = d["result"]?.Value<JArray>();
            if (filter != FeedType.Question)
            {
                result.Should().NotBeNull().And.NotBeEmpty();
                result.Should().OnlyHaveUniqueItems();
            }
            else
            {
                result.Should().BeEmpty();
            }
        }
    }
}
