using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class ItemDto
    {
        public ItemDto(Int64 id, String name, Int64 ownerId, String thumbnail, 
             Guid questionId, Guid answerId, String url)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            Thumbnail = thumbnail;
            QuestionId = questionId;
            AnswerId = answerId;
            Url = url;
        }
        protected ItemDto(long id, long ownerId, Guid? questionId, Guid? answerId, string name, string url)
        {
            Id = id;
            OwnerId = ownerId;
            QuestionId = questionId;
            AnswerId = answerId;
            Name = name;
            Url = url;
        }
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Thumbnail { get; set; }
        public long OwnerId { get; private set; }
       // public virtual string Type { get; set; }

        public Guid? QuestionId { get; set; }
        public Guid? AnswerId { get; set; }

        public string Url { get; set; }

        public string DownloadUrl { get; set; }
    }
}
