using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests.Api
{
    public class QuestionApiTests : IClassFixture<SbWebApplicationFactory>
    {

        private readonly SbWebApplicationFactory _factory;

        public QuestionApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostAsync_Question()
        {
            var client = _factory.CreateClient();

            string crad = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            string question = "{\"subjectId\":\"\",\"course\":\"History of Ethics\",\"text\":\"fdgfdgfdgfdgdfgdf\",\"price\":10,\"files\":[]}";

            var response = await client.PostAsync("/api/login", new StringContent(crad, Encoding.UTF8, "application/json"));

            response = await client.PostAsync("/api/question", new StringContent(question, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }
    }
}
