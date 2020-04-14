using Cloudents.Core.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "unit test")]
    public class StudyRoomSessionTests
    {
        private readonly Mock<Tutor> _tutorMoq;
        private readonly Mock<StudyRoom> _studyRoom;
        public StudyRoomSessionTests()
        {
            
            _tutorMoq = new Mock<Tutor>();
            
            _studyRoom = new Mock<StudyRoom>();

            _tutorMoq.Setup(s => s.User.Id).Returns(2);
            _studyRoom.Setup(s => s.Tutor).Returns(_tutorMoq.Object);
            var mockUser = new Mock<StudyRoomUser>();
            mockUser.Setup(s => s.User.Id).Returns(1);
            _studyRoom.Setup(s => s.Users).Returns(new HashSet<StudyRoomUser> { mockUser.Object, mockUser.Object });
        }
        [Fact]
        public void EndSession_WithSubsidizedPrice_Ok()
        {
           


            _tutorMoq.Setup(s => s.Price).Returns(new TutorPrice(10,5));
            var studyRoomSession = new StudyRoomSession(_studyRoom.Object, "testId");
            var prop = studyRoomSession.GetType().GetProperty("Created");
            prop.SetValue(studyRoomSession, DateTime.UtcNow.AddHours(-1));
            studyRoomSession.EndSession();
            studyRoomSession.Price.Should().NotBeNull();
            studyRoomSession.Price.Should().Be(5M);


        }

        [Fact]
        public void EndSession_WithoutSubsidizedPrice_Ok()
        {

            _tutorMoq.Setup(s => s.Price).Returns(new TutorPrice(10, 5));
            var studyRoomSession = new StudyRoomSession(_studyRoom.Object, "testId");
            var prop = studyRoomSession.GetType().GetProperty("Created");
            prop.SetValue(studyRoomSession, DateTime.UtcNow.AddHours(-1));
            studyRoomSession.EndSession();
            studyRoomSession.Price.Should().NotBeNull();
            studyRoomSession.Price.Should().Be(5M);

        }
    }
}
