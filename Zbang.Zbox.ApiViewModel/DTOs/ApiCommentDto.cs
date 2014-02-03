using System.Runtime.Serialization;

namespace Zbang.Zbox.ApiViewModel.DTOs
{
    [DataContract]
    public class ApiCommentDto
         : ApiTextDto
    {
        [DataMember]
        public string CommentText { get; set; }
        [DataMember]
        public long? ParentCommentUid { get; set; }
    }
}
