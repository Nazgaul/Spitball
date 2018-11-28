using Cloudents.Infrastructure.Mapper;
using Xunit;

namespace Cloudents.Infrastructure.Test
{
    public class MapperProfileTests
    {
        [Fact]
        public void AssertConfigurationIsValid()
        {
            AutoMapper.Mapper.Initialize(cfg => cfg.AddProfile(new MapperProfile()));
            AutoMapper.Mapper.Configuration.AssertConfigurationIsValid();

           // Assert.True(true);
        }
    }
}