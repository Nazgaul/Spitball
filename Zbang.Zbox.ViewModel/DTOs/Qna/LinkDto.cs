using System;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long uid, long ownerId,
            string boxUid, Guid? questionId, Guid? answerId, string name, string thumbnail)
            : base(uid, ownerId, boxUid, questionId, answerId, name)
        {
            Thumbnail = thumbnail;
        }

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
