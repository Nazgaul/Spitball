using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long id, long ownerId,
            string boxUid, Guid? questionId, Guid? answerId, string name, string thumbnail, string url)
            : base(id, ownerId, boxUid, questionId, answerId, name , url)
        {
            Thumbnail = thumbnail;
        }

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
