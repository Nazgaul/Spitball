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

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/login"
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
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }


        [Theory]
        [InlineData("api/question/")]
        public async Task GetAsync_Question_Ok(string uri)
        {
          

            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var questionId = conn.QueryFirst<long>("select top 1 id from sb.question where state = 'Ok'");
                uri += questionId;
            }

            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

        }

        [Theory]
        [InlineData("api/question/")]
        public async Task GetAsync_Question_NotFound(string uri)
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
        public async Task Ask_Course_Without_Uni()
        {
            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_credentials));

            _uri.Path = "api/question";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_question));

            response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            _client.Dispose();
            _fixture.Dispose();
        }
    }


}