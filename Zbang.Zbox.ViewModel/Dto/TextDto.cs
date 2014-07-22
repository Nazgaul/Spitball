using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Zbox.ViewModel.DTOs
{
    [DataContract]
    [KnownType(typeof(CommentDto))]
    public abstract class TextDto//: ISearchable
    {
        
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public virtual string AuthorName { get; set; }

        [DataMember]
        public virtual DateTime CreationTime { get; set; }

        [DataMember]
        public virtual string BoxUid
        {
            get;set;          
        }

        [DataMember]
        public virtual string ItemUid
        {
            get;set;          
        }

        [DataMember]
        public virtual string BoxName { get; set; }

        [DataMember]
        public virtual string ItemName { get; set; }

        public abstract string CommentType { get; }

        [DataMember]
        public virtual string UserImage { get; set; }
    }
}
