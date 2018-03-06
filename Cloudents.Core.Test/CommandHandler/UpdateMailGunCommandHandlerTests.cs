using System;
using System.Reflection;
using Cloudents.Core.CommandHandler;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Core.Test.CommandHandler
{
    [TestClass]
    public class UpdateMailGunCommandHandlerTests
    {
        [TestMethod]
        public void ConvertDateTimeToString_SpecificDate_ConvertCorrectly()
        {
            var dateTime = new DateTime(2017, 10, 2, 15, 1, 0);

            var type = new PrivateType(typeof(UpdateMailGunCommandHandler));
            var result = type.InvokeStatic("ConvertDateTimeToString", BindingFlags.Static | BindingFlags.NonPublic, dateTime) as string;

            result.Should().BeEquivalentTo("Oct  2 2017  3:01PM");
        }

        [TestMethod]
        public void ConvertDateTimeToString_SpecificDate2_ConvertCorrectly()
        {
            var dateTime = new DateTime(2018, 2, 21, 19, 1, 0);
            var type = new PrivateType(typeof(UpdateMailGunCommandHandler));
            var result = type.InvokeStatic("ConvertDateTimeToString", BindingFlags.Static | BindingFlags.NonPublic, dateTime) as string;

            result.Should().BeEquivalentTo("Feb 21 2018  7:01PM");
        }

        [TestMethod]
        public void ConvertDateTimeToString_SpecificDate3_ConvertCorrectly()
        {
            var dateTime = new DateTime(2018, 3, 1, 20, 2, 0);
            var type = new PrivateType(typeof(UpdateMailGunCommandHandler));
            var result = type.InvokeStatic("ConvertDateTimeToString", BindingFlags.Static | BindingFlags.NonPublic, dateTime) as string;

            result.Should().BeEquivalentTo("Mar  1 2018  8:02PM");
        }
    }
}