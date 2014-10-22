using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public abstract class ItemDto
    {
        protected ItemDto(long id, long ownerId,
           Guid? questionId, Guid? answerId, string name, string url)
        {
            Id = id;
            OwnerId = ownerId;
            //BoxId = boxId;
            QuestionId = questionId;
            AnserId = answerId;
            Name = name;
            Url = url;
        }
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Thumbnail { get; set; }
       // public long BoxId { get; private set; }
        public long OwnerId { get; private set; }
        public abstract string Type { get; }

        public Guid? QuestionId { get; set; }
        public Guid? AnserId { get; set; }

        public string Url { get; set; }

        public string DownloadUrl { get; set; }
    }
}
