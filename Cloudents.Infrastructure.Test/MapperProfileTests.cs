using Cloudents.Infrastructure.Mapper;
using Cloudents.Query.Stuff;
using Xunit;

namespace Cloudents.Infrastructure.Test
{
    public class MapperProfileTests
    {
        [Fact]
        public void AssertConfigurationIsValid()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
                //cfg.AddProfile<ConfigureMapper>();
            });

            AutoMapper.Mapper.Configuration.AssertConfigurationIsValid();

           // Assert.True(true);
        }
    }
}