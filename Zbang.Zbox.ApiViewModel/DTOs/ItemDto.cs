using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ApiViewModel.DTOs
{
    [DataContract]
    [KnownType(typeof(FileDto))]
    [KnownType(typeof(LinkDto))]
    public abstract class ItemDto
    {
        public ItemDto(string name, string uid, long commentCount, DateTime creationTime, DateTime updateTime,
            string userName, long userUid, string userImage, UserType userType)
        {
            Name = name;
            Uid = uid;
            Owner = new UserDto(userName, userUid, userImage, userType);
            CommentCount = commentCount;
            CreationTime = creationTime;
            UpdateTime = updateTime;
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Uid { get; set; }
        
        [DataMember]
        public abstract string ItemType { get; protected set; }
        [DataMember]
        public long CommentCount { get; set; }
        [DataMember]
        public DateTime CreationTime { get; set; }
        [DataMember]
        public DateTime UpdateTime { get; set; }

        [DataMember]
        public UserDto Owner { get; set; }

    }
}
