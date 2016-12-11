using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class ItemDto
    {
        public ItemDto()
        {
            
        }
        //version 2 api mobile
       

       
        //this is for item
        public ItemDto(long id, string name, long ownerId,
             Guid? questionId, Guid? answerId,  string type, string source)
        {
            if (questionId == null) throw new ArgumentNullException(nameof(questionId));
            Id = id;
            Name = name;
            OwnerId = ownerId;
            //Thumbnail = thumbnail;
            QuestionId = questionId;
            AnswerId = answerId;
            Type = type;
            Source = source;
        }
        //this is for question
        public ItemDto(long id, string name, long ownerId,
             Guid? questionId)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Id = id;
            Name = name;
            OwnerId = ownerId;
            QuestionId = questionId;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        //public string Thumbnail { get; set; }
        public long OwnerId { get; set; }

        public Guid? QuestionId { get; set; }
        public Guid? AnswerId { get; set; }


        public string DownloadUrl { get; set; }
        public string Type { get; set; }

        public string Source { get; set; }
    }
}
