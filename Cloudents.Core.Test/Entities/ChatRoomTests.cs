using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class ChatRoomTests
    {
        [Fact]
        public void AddMessage_ChatRoomWithoutStudyRoom_UnreadCountIncrease()
        {
            var tutor = new Mock<Tutor>();
            var users = new List<User>();
            for (int i = 0; i < 2; i++)
            {
                var moq = new Mock<User>();
                moq.Setup(s2 => s2.Id).Returns(i + 1);
                users.Add(moq.Object);
            }
            var chatRoom = new ChatRoom(users, tutor.Object);
            chatRoom.AddTextMessage(users[0], "Hi Man");
            var resultExpected = chatRoom.Users.Any(a => a.Unread > 0);
            chatRoom.Messages.Count.Should().Be(1);
            resultExpected.Should().BeTrue();
        }


        [Fact]
        public void AddMessage_ChatRoomBroadcast_NoUnread()
        {

            var users = new List<User>();
            for (int i = 0; i < 2; i++)
            {
                var moq = new Mock<User>();
                moq.Setup(s2 => s2.Id).Returns(i + 1);
                users.Add(moq.Object);
            }

            var moqStudyRoom = new Mock<BroadCastStudyRoom>();

            var chatRoom = ChatRoom.FromStudyRoom(moqStudyRoom.Object);

            foreach (var user in users)
            {
                chatRoom.AddUserToChat(user);
            }



            chatRoom.AddTextMessage(users[0], "Hi Man");
            var resultExpected = chatRoom.Users.All(a => a.Unread == 0);
            chatRoom.Messages.Count.Should().Be(1);
            resultExpected.Should().BeTrue();
        }
    }
}