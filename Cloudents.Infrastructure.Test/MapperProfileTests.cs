using AutoMapper;
using Cloudents.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class MapperProfileTests
    {
        [TestMethod]
        public void AssertConfigurationIsValid()
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new MapperProfile()));
            Mapper.Configuration.AssertConfigurationIsValid();

            Assert.IsTrue(true);
        }
    }
}