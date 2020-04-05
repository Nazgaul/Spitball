using System;

namespace Cloudents.Core.DTOs
{
    public abstract class SaleDto
    {
        public virtual ContentType Type { get; set; }
       // public virtual PaymentStatus PaymentStatus { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }

    public class DocumentSaleDto : SaleDto
    {
        public string Course { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }
      //  public override PaymentStatus PaymentStatus => PaymentStatus.Approved;
    }

    public class QuestionSaleDto : SaleDto
    {
        public long Id{ get; set; }
        public string Course { get; set; }
        public string Text { get; set; }
        public string AnswerText { get; set; }
       // public override PaymentStatus PaymentStatus => PaymentStatus.Paid;
        public override ContentType Type => ContentType.Question;
    }
    public class SessionSaleDto : SaleDto
    {
        public string StudyRoomName { get; set; }
        public Guid SessionId { get; set; }
        public string StudentName { get; set; }
        public TimeSpan Duration { get; set; }

        public bool ShouldSerializeDuration() => false;

        public double TotalMinutes => Duration.TotalMinutes;
        public string StudentImage { get; set; }
        public long StudentId { get; set; }
        public override ContentType Type => ContentType.TutoringSession;

        public virtual PaymentStatus PaymentStatus { get; set; }
    }

    public enum PaymentStatus
    {
        PendingSystem,
        PendingTutor,
        Approved
    }
    public enum ContentType
    {
        Document,
        Video,
        TutoringSession,
        Question,
        BuyPoints
    }
}
