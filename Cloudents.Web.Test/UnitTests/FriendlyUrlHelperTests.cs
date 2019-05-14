using Cloudents.Web.Extensions;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    public class FriendlyUrlHelperTests
    {
        [Fact]
        public void CompareTitle_RegularString_Equals()
        {
            const string input = "gfdgdfgdfg";

            const string dbValue = input;

            var result = FriendlyUrlHelper.CompareTitle(dbValue, input);

            result.Should().Be(FriendlyUrlHelper.TitleCompareResult.Equal);
        }

    }
}
