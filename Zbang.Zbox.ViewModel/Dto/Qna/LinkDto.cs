using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long uid, long ownerId,
            string boxUid, Guid? questionId, Guid? answerId, string name, string thumbnail, string url)
            : base(uid, ownerId, boxUid, questionId, answerId, name , url)
        {
            Thumbnail = thumbnail;
        }

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
