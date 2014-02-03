using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Zbang.Zbox.ApiViewModel.DTOs
{
    [DataContract]
    [KnownType(typeof(ApiCommentDto))]
    [KnownType(typeof(ApiActionDto))]
    public abstract class ApiTextDto
    {
        [DataMember]
        public long Uid { get; set; }
        [DataMember]
        public UserDto Owner { get; set; }
        [DataMember]
        public DateTime CreationTime { get; set; }
        [DataMember]
        public DateTime UpdateTime { get; set; }
        [DataMember]
        public string BoxUid { get; set; }
        [DataMember]
        public string ItemUid { get; set; }
    }
}
