using AutoMapper;
using Cloudents.Infrastructure.Mapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class MapperProfileTests
    {
        [TestMethod]
        public void AssertConfigurationIsValid()
        {
            AutoMapper.Mapper.Initialize(cfg => cfg.AddProfile(new MapperProfile()));
            AutoMapper.Mapper.Configuration.AssertConfigurationIsValid();

            Assert.IsTrue(true);
        }
    }
}