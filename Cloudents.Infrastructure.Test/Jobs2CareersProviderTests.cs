using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Job;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class Jobs2CareersProviderTests
    {
        [TestMethod]
        public async Task SearchAsync_LocationNonUs_ReturnNull()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var instance = mock.Create<Jobs2CareersProvider>();

                var address = new Address("Some City", "Some Region", "IL");
                var location = new Location(null, address, "Some ip");

                var result = await instance.SearchAsync(null, JobRequestSort.Relevance, null, location, 0, default).ConfigureAwait(false);

                result.Should().BeNull();
            }
        }
    }
}