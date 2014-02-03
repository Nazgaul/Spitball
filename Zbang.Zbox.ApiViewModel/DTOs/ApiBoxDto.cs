using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ApiViewModel.DTOs
{
    [DataContract]
    public class ApiBoxDto
    {

        [DataMember]
        public virtual string Name { get; set; }


        [DataMember]
        public virtual DateTime UpdateTime { get; set; }

        [DataMember]
        public virtual UserDto Owner { get; set; }

        [DataMember]
        public UserRelationshipType MemberType { get; set; }

        [DataMember]
        public int ItemCount { get; set; }

        [DataMember]
        public int CommentCount { get; set; }

        [DataMember]
        public int MembersCount { get; set; }

        [DataMember]
        public virtual string Thumbnail { get; set; }


        [DataMember]
        public string Uid
        {
            get;
            set;
        }
    }
}
