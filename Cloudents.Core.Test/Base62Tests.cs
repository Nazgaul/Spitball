using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test
{
    public class Base62Tests
    {
        [Fact]
        public void TryParse_InvalidValue_False()
        {
            const string invalidString = "המסלול-האקדמי-המכללה-למנהל";
            var result = Base62.TryParse(invalidString, out var p);

            result.Should().BeFalse();
            p.Value.Should().Be(0);
            p.ToString().Should().Be("0");
        }
    }
}
