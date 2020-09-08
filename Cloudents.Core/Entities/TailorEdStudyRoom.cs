using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public class TailorEdStudyRoom : StudyRoom
    {
        public TailorEdStudyRoom(Tutor tutor,
            IList<(SystemUser user,string code)> users,
            string onlineDocumentUrl) : base(tutor,  0)
        {
            if (users.Count == 0)
            {
                throw new ArgumentException();
            }
            Identifier = Guid.NewGuid().ToString();
            ChatRoom = ChatRoom.FromStudyRoom(this);

            foreach (var user in users)
            {
                _users.Add(new StudyRoomUser(user.user, this)
                {
                    Code = user.code
                });
                ChatRoom.AddUserToChat(tutor.User);
                ChatRoom.AddUserToChat(user.user);

            }
            Name = "Tailor Ed";
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

        protected TailorEdStudyRoom()
        {
            
        }
        public override StudyRoomType Type => StudyRoomType.TailorEd;

        public virtual string Name { get; protected set; }

        public override void AddUserToStudyRoom(User user)
        {
            if (Tutor.User.Id == user.Id)
            {
                return;
            }
            var _ = Users.AsQueryable().Single(s => s.User.Id == user.Id);
        }

    }
}