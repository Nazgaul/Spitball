using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test
{
    public class Base62Tests
    {
        [Fact]
        public void TryParse_InvalidValue_False()
        {
            var invalidString = "המסלול-האקדמי-המכללה-למנהל";
            var result = Base62.TryParse(invalidString, out var p);

            result.Should().BeFalse();
            p.Value.Should().Be(0);
            p.ToString().Should().Be("0");
        }
    }
}
