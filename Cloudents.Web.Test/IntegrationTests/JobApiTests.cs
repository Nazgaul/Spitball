﻿using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class JobApiTests :  IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly WebApplicationFactory<Startup> _factory;

        public JobApiTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/Job?Term=Android&Location.Point.Longitude=40.0&Location.Point.Latitude=-51.&Highlight=true&Page=0")]
        [InlineData("/api/Job?Location.Point.Latitude=33.71&Location.Point.Longitude=-117.9478&Facet=Part+Time%2CInternship&Page=1")]
        [InlineData("api/job?term=")]
        [InlineData("api/job?location.Point.latitude=40.712&location.Point.longitude=-74.005&term=")]
        public async Task Search_ReturnResult(string url)
        {
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }

        


        [Fact]
        public async Task Search_OnlyLocationUSEmptyTerm_ReturnResult()
        {
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/job?location.Point.latitude=40.712&location.Point.longitude=-74.005&term=");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var d = JObject.Parse(result);
            var p = d["result"]["result"].Values();
            
            p.Should().HaveCountGreaterOrEqualTo(1);
        }

        
    }
}