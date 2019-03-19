using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    public class HomeControllerTests :IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public HomeControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Logout_Redirect()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("logout");

            response.StatusCode.Should().Be(302);
        }
    }
}
