﻿namespace Cloudents.Core.DTOs.Admin
{

    //public class UserAnswersDto
    //{
    //    [EntityBind(nameof(Answer.Id))]
    //    public Guid Id { get; set; }
    //    [EntityBind(nameof(Answer.Text))]
    //    public string Text { get; set; }
    //    [EntityBind(nameof(Answer.Created))]
    //    public DateTime Created { get; set; }
    //    [EntityBind(nameof(Answer.Question.Id))]
    //    public long QuestionId { get; set; }
    //    [EntityBind(nameof(Answer.Question.Text))]
    //    public string QuestionText { get; set; }
    //    [EntityBind(nameof(Answer.Status.State))]
    //    public ItemState State { get; set; }

    //}

    //public class UserQuestionsDto
    //{
    //    [EntityBind(nameof(Question.Id))]
    //    public long Id { get; set; }
    //    [EntityBind(nameof(Question.Text))]
    //    public string Text { get; set; }
    //    [EntityBind(nameof(Question.Created))]
    //    public DateTime Created { get; set; }
    //    [EntityBind(nameof(Question.Status.State))]
    //    public ItemState State { get; set; }
    //}

    public class UserPurchasedDocsDto
    {
        public long DocumentId { get; set; }
        public string Title { get; set; }

        public string Class { get; set; }
        public decimal Price { get; set; }
    }
}
