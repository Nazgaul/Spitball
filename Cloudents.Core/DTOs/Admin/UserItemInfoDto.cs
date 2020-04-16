using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;


namespace Cloudents.Core.DTOs.Admin
{

    public class UserAnswersDto
    {
        [EntityBind(nameof(Answer.Id))]
        public Guid Id { get; set; }
        [EntityBind(nameof(Answer.Text))]
        public string Text { get; set; }
        [EntityBind(nameof(Answer.Created))]
        public DateTime Created { get; set; }
        [EntityBind(nameof(Answer.Question.Id))]
        public long QuestionId { get; set; }
        [EntityBind(nameof(Answer.Question.Text))]
        public string QuestionText { get; set; }
        [EntityBind(nameof(Answer.Status.State))]
        public ItemState State { get; set; }

    }

    public class UserQuestionsDto
    {
        [EntityBind(nameof(Question.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(Question.Text))]
        public string Text { get; set; }
        [EntityBind(nameof(Question.Created))]
        public DateTime Created { get; set; }
        [EntityBind(nameof(Question.Status.State))]
        public ItemState State { get; set; }
    }
    public class UserDocumentsDto
    {
        [EntityBind(nameof(Document.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(Document.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(Document.TimeStamp.CreationTime))]
        public DateTime Created { get; set; }
        //[EntityBind(nameof(Document.University.Name))]
        //public string University { get; set; }
        [EntityBind(nameof(Document.Course.Id))]
        public string Course { get; set; }
        [EntityBind(nameof(Document.Price))]
        public decimal? Price { get; set; }
        [EntityBind(nameof(Document.Status.State))]
        public ItemState State { get; set; }

        public Uri Preview { get; set; }
        public string SiteLink { get; set; }
    }

    public class UserPurchasedDocsDto
    {
        [EntityBind(nameof(Document.Id))]
        public long DocumentId { get; set; }
        [EntityBind(nameof(Document.Name))]
        public string Title { get; set; }
        //[EntityBind(nameof(Document.University.Name))]
        //public string University { get; set; }
        [EntityBind(nameof(Document.Course))]
        public string Class { get; set; }
        [EntityBind(nameof(Document.Price))]
        public decimal Price { get; set; }
    }
}
