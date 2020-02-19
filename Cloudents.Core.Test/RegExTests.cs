using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test
{
    public class RegExTests
    {
        [Theory]
        [InlineData("اختبار النص")]
        [InlineData("טסט טקסט")]
        public void CheckRtlText_Return_True(string text)
        {
            var result = RegEx.RtlLetters.IsMatch(text);
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Text in English")]
        public void CheckLtrText_Return_False(string text)
        {
            var result = RegEx.RtlLetters.IsMatch(text);
            result.Should().BeFalse();
        }
    }
}
