using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Globalization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Email
{
    public class EmailDto
    {
        [EntityBind(nameof(User.Email))] public string ToEmailAddress { get; set; }
        [EntityBind(nameof(User.Language))] public string Language { get; set; }


        [EntityBind(nameof(User.Id))] public long UserId { get; set; }
    }

    public class DocumentPurchaseEmailDto : EmailDto
    {
        [EntityBind(nameof(Document.Course.Id))]
        public string CourseName { get; set; }

        [EntityBind(nameof(Document.Name))] public string DocumentName { get; set; }

        [EntityBind(nameof(Transaction.Price))]
        public decimal Tokens { get; set; }
    }

    //public class AnswerAcceptedEmailDto : EmailDto
    //{
    //    [EntityBind(nameof(Question.Text))] private string _questionText;
    //    [EntityBind(nameof(Answer.Text))] private string _answerText;

    //    [EntityBind(nameof(Question.Id))] public long QuestionId { get; set; }

    //    public string QuestionText
    //    {
    //        get => _questionText.Replace("\n", "<br>").Truncate(40, true);
    //        set => _questionText = value;
    //    }

    //    public string AnswerText
    //    {
    //        get => _answerText.Replace("\n", "<br>").Truncate(40, true);
    //        set => _answerText = value;
    //    }

    //    //public decimal Tokens { get; set; }
    //}

    public class UpdateUserEmailDto
    {
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; }

        [EntityBind(nameof(User.Email))]
        public string ToEmailAddress { get; set; }
        [EntityBind(nameof(User.Language))]
        public CultureInfo Language { get; set; }

        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; }

        public DateTime Since { get; set; }
    }

    public class QuestionUpdateEmailDto : UpdateEmailDto
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }

    }

    public class DocumentUpdateEmailDto : UpdateEmailDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DocumentType DocumentType { get; set; }
        //public string Image { get; set; }

    }

    public abstract class UpdateEmailDto
    {
        public string Course { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public long UserId { get; set; }
    }
}