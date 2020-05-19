using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class AnswerTests
    {
        [Theory]
        [InlineData('a', 0)]
        [InlineData('a', 14)]
        [InlineData('a', 551)]
        public void IllegalCharsAnswerTests(char c, int num)
        {
            string str = new string(c, num);
            Assert.Throws<ArgumentException>(() => new Answer(null, str, null));
        }
    }
}
