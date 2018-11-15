using Cloudents.Core.Query.Sync;
using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test
{
    public class SyncAzureQueryTests
    {
        [Fact]
        public void ConvertFromString_Null_Init()
        {
            var result = SyncAzureQuery.ConvertFromString(null);
            var expected = new SyncAzureQuery(0,0);
            result.Should().BeEquivalentTo(expected);
            //Assert.AreEqual(result,expected);
        }

        [Fact]
        public void ConvertFromString_EmptyString_Init()
        {
            var result = SyncAzureQuery.ConvertFromString(string.Empty);
            var expected = new SyncAzureQuery(0, 0);
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ConvertFromString_SomeValue_Init()
        {
            var input = "2|4";
            var result = SyncAzureQuery.ConvertFromString(input);
            var expected = new SyncAzureQuery(2, 4);
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ConvertFromString_FullCycle_Init()
        {
            var expected = new SyncAzureQuery(2, 4);
            var result = SyncAzureQuery.ConvertFromString(expected.ToString());
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ConvertFromString_InvalidInput_Init()
        {
            var input = "2|4|2";
            var result = SyncAzureQuery.ConvertFromString(input);
            var expected = new SyncAzureQuery(0, 0);
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ConvertFromString_InvalidInput2_Init()
        {
            var input = "2&4";
            var result = SyncAzureQuery.ConvertFromString(input);
            var expected = new SyncAzureQuery(0, 0);
            result.Should().BeEquivalentTo(expected);
        }
    }
}