using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public class TailorEdStudyRoom : StudyRoom
    {
        public TailorEdStudyRoom(Tutor tutor,
            //IList<SystemUser> users,
            int amountOfUsers,
            string code,
            string onlineDocumentUrl) : base(tutor,  0)
        {
            if (amountOfUsers == 0)
            {
                throw new ArgumentException();
            }
            Identifier = Guid.NewGuid().ToString();
            ChatRoom = ChatRoom.FromStudyRoom(this);
            ChatRoom.AddUserToChat(tutor.User);
            //foreach (var user in users)
            //{
            //    _users.Add(new StudyRoomUser(user, this));
              
            //    ChatRoom.AddUserToChat(user);

            //}
            Name = "Tailor Ed";
            OnlineDocumentUrl = onlineDocumentUrl;
            Code = code;
            if (amountOfUsers < 4 /*&& _users.Count > 0*/)
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

        public virtual string Code { get; set; }


        public virtual void AddFictiveUser(SystemUser user)
        {
            _users.Add(new StudyRoomUser(user, this));
              
            ChatRoom.AddUserToChat(user);
        }


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