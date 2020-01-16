using System;
using System.Net;
using System.Threading.Tasks;
using Cloudents.Infrastructure.Data.Test.IntegrationTests;
using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public sealed class QuestionsControllerTests : IDisposable //:  IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;
        private readonly DatabaseFixture _fixture = new DatabaseFixture();


        private readonly object _question = new
        {
            subjectId = "",
            course = "Economics",
            text = "Blah blah blah...",
            price = 1
        };

      

        private readonly object _credentials = new
        {
            email = "blah@cloudents.com",
            password = "123456789",
            fingerPrint = "string"
        };

        public QuestionsControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }


        [Theory]
        [InlineData("api/question/")]
        public async Task GetAsync_Question_OkAsync(string uri)
        {
          

            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var questionId = conn.QueryFirst<long>("select top 1 id from sb.question where state = 'Ok'");
                uri += questionId;
            }

            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            str.IsValidJson().Should().BeTrue();

        }

        [Theory]
        [InlineData("api/question/")]
        public async Task GetAsync_Question_NotFoundAsync(string uri)
        {

            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var questionId = conn.QueryFirst<long>("select top 1 id from sb.question where state = 'deleted'");
                uri += questionId;
            }

            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        }


        [Fact]
        public async Task Ask_Course_Without_UniAsync()
        {
            await _client.PostAsync("api/login", HttpClientExtensions.CreateJsonString(_credentials));
            var response = await _client.PostAsync("api/question", HttpClientExtensions.CreateJsonString(_question));

            response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            _client.Dispose();
            _fixture.Dispose();
        }
    }


}