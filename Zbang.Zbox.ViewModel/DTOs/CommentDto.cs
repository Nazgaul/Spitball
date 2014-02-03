using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Zbox.ViewModel.DTOs
{
    [Serializable]
    [DataContract]
    public class CommentDto : TextDto
    {
        [DataMember]
        public virtual string CommentText { get; set; }
        
        [DataMember]
        public virtual long? ParentId { get; set; }
        
        [DataMember]
        public virtual DateTime UpdateTime { get; set; }
        
        [DataMember]
        public override string CommentType
        {
            get { return "Comment"; }
        }

        [DataMember]
        public virtual bool Deleted { get; private set; }
    }


    public class CommentDto2 : CommentDto
    {
        public CommentDto2()
        {
            Replies = new List<CommentDto2>();
        }
        

        public IList<CommentDto2> Replies { get; set; }
    }
}
