﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Api.Test.IntegrationTests
{
    [TestClass]
    public class UniversityTests : ServerInit
    {
        [TestMethod]
        public async Task GetAsync_Empty_OK()
        {
            var response =
                await Client.GetAsync(
                    "/api/university");
            response.EnsureSuccessStatusCode();
        }
    }
}