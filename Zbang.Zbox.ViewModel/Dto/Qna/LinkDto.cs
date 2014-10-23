using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long id, long ownerId,
            Guid? questionId, Guid? answerId, string name, string thumbnail, string url)
            : base(id, ownerId, questionId, answerId, name , url)
        {
            Thumbnail = thumbnail;
        }

        //public override string Type
        //{
        //    get { return "Link"; }
        //}
    }
}
