using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Job;
using FluentAssertions;
using Xunit;

namespace Cloudents.Infrastructure.Test
{
    public class Jobs2CareersProviderTests
    {
        [Fact]
        public async Task SearchAsync_LocationNonUs_ReturnNull()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var instance = mock.Create<Jobs2CareersProvider>();

                var address = new Address("Some City", "Some Region", "IL");
                var location = new Location(null, address, "Some ip", "972");
                var request = new JobProviderRequest(null, JobRequestSort.Relevance, null, location, 0);
                var result = await instance.SearchAsync(request, default);

                result.Should().BeNull();
            }
        }
    }
}