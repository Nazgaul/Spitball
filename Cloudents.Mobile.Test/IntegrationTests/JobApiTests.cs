﻿using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Api.Test.IntegrationTests
{
    [TestClass]
    public class JobApiTests : ServerInit
    {
        [TestMethod]
        public async Task Search_SomeQuery_ReturnResult()
        {
            var response = await Client.GetAsync("/api/Job?Term=Android&Location.Point.Longitude=40.0&Location.Point.Latitude=-51.&Highlight=true&Page=0").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}