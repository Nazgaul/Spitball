﻿using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{

    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TutorControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public TutorControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetAsync_ReturnResult_OK(bool logIn)
        {
            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync("api/tutor");
            response.EnsureSuccessStatusCode();
        }


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetAsyncWithCourse_ReturnResult_OK(bool logIn)
        {
            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync("api/tutor?course=Economics");
            response.EnsureSuccessStatusCode();
        }
        

        [Theory]
        [InlineData("gfc",false ,0)]
        [InlineData("Economics", false,0)]
        [InlineData("Math", false, 0)]
        [InlineData("", false, 0)]
        [InlineData("gfc", true, 0)]
        [InlineData("Economics", true, 0)]
        [InlineData("Math", true, 0)]
        [InlineData("", true, 0)]
        [InlineData("", true, 1)]
        public async Task GetAsync_Search_Without_Results(string term, bool logIn, int page)
        {

            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync($"api/tutor/search?term={term}&page={page}");
            response.EnsureSuccessStatusCode();
            //var str = await response.Content.ReadAsStringAsync();

            //str.Should().BeEmpty();
        }

        [Theory]
        [InlineData("api/tutor")]
        [InlineData("api/tutor/search")]
        [InlineData("api/tutor/search?term=ram")]
        [InlineData("api/tutor/search?term=saul%20goodman")]
        [InlineData("api/tutor/reviews")]
        public async Task GetAsync_Tutor_Ok(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("api/tutor/calendar/list")]
        [InlineData("api/tutor/calendar/events?from=2019-11-18T05:48:47.649Z&to=2019-11-18T07:48:47.649Z")]
        public async Task GetAsync_Tutor_Unauthorized(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("api/tutor/calendar/list")]
        [InlineData("api/tutor/calendar/events?from=2019-11-18T05:48:47.649Z&to=2019-11-18T07:48:47.649Z&tutorid=159489")]
        public async Task GetAsync_Tutor_Calendar_OK(string uri)
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}