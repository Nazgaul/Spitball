using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core.Entities;
using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class StudyRoomTests
    {

        [Fact]
        public void OnlineDocumentSetter_NoUrl_Exception()
        {
            var moq = new Moq.Mock<StudyRoom>()
            {
                CallBase = true
            };
            Assert.Throws<ArgumentException>(() => moq.Object.OnlineDocumentUrl = "blabla");
        }


        [Fact]
        public void OnlineDocumentSetter_ActualUrl_Ok()
        {
            const string url = "https://docs.google.com/document/d/SomeValue/edit?usp=drivesdk.";
            var moq = new Moq.Mock<StudyRoom>()
            {
                CallBase = true
            };
            moq.Object.OnlineDocumentUrl = url;

            moq.Object.OnlineDocumentUrl.Should().Be(url);
        }
    }
}
