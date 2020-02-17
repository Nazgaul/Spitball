using System.Linq;
using Cloudents.Core.Extension;
using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("Ram")]
        [InlineData("Ram Yaari")]
        [InlineData("Ram Yaari...")]
        [InlineData("...")]
        [InlineData("    ")]
        [InlineData(null)]
        public void ReverseOnlyHebrew_DoNothing(string input)
        {
            //var input = "Ram";
            input.ReverseOnlyHebrew().Should().Be(input);
        }

        


        [Theory]
        [InlineData("פל")]
        [InlineData("פל    ")]
        [InlineData("פל פל")]
        [InlineData("פל פל...")]
        [InlineData("בקרוב תראו תוצאות וציונים שיעלו לכם חיוך על הפנים :) (אפילו אם כרגע זה נראה בלתי אפשרי). בעל ניסיון של 6 שנים! ")]
        public void ReverseOnlyHebrew_CheckOnlyHebrew(string input)
        {

            //var input = "פל";
            input.ReverseOnlyHebrew().Should().Be(new string(input.Trim().ToCharArray().Reverse().ToArray()));

        }

        [Theory]
        [InlineData("פל fala"," לפfala")]
        [InlineData("fala פל", "fala לפ")]
        [InlineData("fala פל...", "fala ...לפ")]
        public void ReverseOnlyHebrew_CheckMix(string input, string output)
        {
            input.ReverseOnlyHebrew().Should().Be(output);
        }

      
        
    }
}