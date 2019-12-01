using Cloudents.Core.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class StudyRoomSessionTests
    {
        [Fact]
        public void EndSession_WithSubsidizedPrice_Ok()
        {
            
            var studyRoom = new Mock<StudyRoom>();
            var user = new User("hadar@cloudents.com", "firstName", "lastName", Language.Hebrew,"IN");
            var tutor = new Tutor("this is bio", user, 100);

            var prop = tutor.Price.GetType().GetProperty("SubsidizedPrice");
            prop.SetValue(tutor.Price, 10M);

            studyRoom.SetupAllProperties();
            prop = studyRoom.Object.GetType().GetProperty("Tutor");
            prop.SetValue(studyRoom.Object, tutor);
           
            var studyRoomSession = new StudyRoomSession(studyRoom.Object, "testId");
            prop = studyRoomSession.GetType().GetProperty("Created");
            prop.SetValue(studyRoomSession, DateTime.UtcNow.AddHours(-1));
            studyRoomSession.EndSession();
            studyRoomSession.Price.Should().NotBeNull();
            studyRoomSession.Price.Should().Equals(10M);


        }

        [Fact]
        public void EndSession_WithoutSubsidizedPrice_Ok()
        {

            var studyRoom = new Mock<StudyRoom>();
            var user = new User("hadar@cloudents.com", "firstName", "lastName", Language.Hebrew,"IL");
            var tutor = new Tutor("this is bio", user, 10);

            studyRoom.SetupAllProperties();
            var prop = studyRoom.Object.GetType().GetProperty("Tutor");
            prop.SetValue(studyRoom.Object, tutor);

            var studyRoomSession = new StudyRoomSession(studyRoom.Object, "testId");
            prop = studyRoomSession.GetType().GetProperty("Created");
            prop.SetValue(studyRoomSession, DateTime.UtcNow.AddHours(-1));
            studyRoomSession.EndSession();
            studyRoomSession.Price.Should().NotBeNull();
            studyRoomSession.Price.Should().Equals(10M);

        }
    }
}
