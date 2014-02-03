using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.DTOs
{
    [DataContract]
    [KnownType(typeof(UserDto))]
    public class UserPublicSettingDto : UserPublicDto
    {
        [DataMember]
        public virtual UserRelationshipType Type { get; set; }
    }

    [DataContract]
    [KnownType(typeof(UserPublicSettingDto))]
    public class UserPublicDto
    {
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Image { get; set; }

        [DataMember]
        public string Uid { get; set; }
    }
}
