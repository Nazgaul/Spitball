using System;
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Enums;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs
{
    [DataContract]
    [Serializable]
    public class BoxWithDetailDto
    {
        public BoxWithDetailDto()
        {
            Items = new PagedDto<ItemDto>();
        }
        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual string Owner { get; set; }

        [DataMember]
        public virtual string OwnerPicture { get; set; }

        [DataMember]
        public virtual BoxPrivacySettings PrivacySetting { get; set; }

        [DataMember]
        public virtual long MembersCount { get; set; }

        [DataMember]
        public virtual string Uid { get; set; }

        [DataMember]
        public IList<CommentDto> Comments { get; set; }

        [DataMember]
        public PagedDto<ItemDto> Items { get; set; }

        [DataMember]
        public virtual string Image { get; set; }


        public UserRelationshipType UserType { get; set; }
    }



}
