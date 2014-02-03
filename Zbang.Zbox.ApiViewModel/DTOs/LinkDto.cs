using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ApiViewModel.DTOs
{
    [DataContract]
    public class LinkDto : ItemDto
    {

        public LinkDto(string uid, string name, DateTime creationTime, DateTime updateTime, long commentCount,
            string userName, long userUid, string userImage, UserType userType)
            : base(name, uid, commentCount, creationTime, updateTime, userName, userUid, userImage, userType)
        {

        }
        [DataMember]
        public override string ItemType
        {
            get { return "link"; }
            protected set { }

        }
        [DataMember]
        public string LinkUrl { get { return Name; } protected set { } }
    }
}
