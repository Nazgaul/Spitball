using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public class PrivateStudyRoom : StudyRoom
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public PrivateStudyRoom(Tutor tutor, IEnumerable<User> users, string onlineDocumentUrl,
            string name, decimal price) : base(tutor, price)
        {
            foreach (var user in users)
            {
                _users.Add(new StudyRoomUser(user, this));

            }
            if (_users.Count == 0)
            {
                throw new ArgumentException();
            }

            Name = name;
            Identifier = ChatRoom.BuildChatRoomIdentifier(
                _users.Select(s => s.User.Id).Union(new[] { tutor.Id }));
            OnlineDocumentUrl = onlineDocumentUrl;
            if (_users.Count < 4 && _users.Count > 0)
            {
                TopologyType = StudyRoomTopologyType.SmallGroup;
            }
            else
            {
                TopologyType = StudyRoomTopologyType.GroupRoom;
            }
        }






        protected PrivateStudyRoom() : base()
        {
        }

        public virtual string Name { get; protected set; }

        public override void AddUserToStudyRoom(User user)
        {
            if (Tutor.User.Id == user.Id)
            {
                return;
            }
            var _ = Users.AsQueryable().Single(s => s.User.Id == user.Id);
        }

        public virtual void AddPayment(User user, string receipt)
        {
            var studyRoomPayment = new StudyRoomPayment(this, user, receipt);
            _studyRoomPayments.Add(studyRoomPayment);
        }

        public override StudyRoomType Type => StudyRoomType.Private;
    }
}