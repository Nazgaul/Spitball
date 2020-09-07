using System;

namespace Cloudents.Core.DTOs
{
    public abstract class SaleDto
    {
        public virtual ContentType Type { get; set; }
        public DateTime Date { get; set; }
        public abstract double Price { get; }
    }

    public class CourseSaleDto : SaleDto
    {
        public long Id { get; set; }

        public string Preview { get; set; }
        public override double Price => price;
        public string Name { get; set; }

        [NonSerialized] public double price;
        [NonSerialized] public int version;
    }

    public class DocumentSaleDto : SaleDto
    {
        public string Course { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }


        [NonSerialized] public decimal _price;

        public override double Price => (double)_price;
    }

    
    public class SessionSaleDto : SaleDto
    {
        public string StudyRoomName { get; set; }
        public Guid SessionId { get; set; }
        public string StudentName { get; set; }
        public TimeSpan? Duration { get; set; }

        public bool ShouldSerializeDuration() => false;

        public double TotalMinutes => Duration.GetValueOrDefault().TotalMinutes;
        public string? StudentImage { get; set; }
        public long StudentId { get; set; }
        public override ContentType Type => ContentType.TutoringSession;

        public virtual PaymentStatus PaymentStatus { get; set; }

        [NonSerialized] public decimal _oldPrice;

        [NonSerialized] public double? _price;

        //  public override PaymentStatus PaymentStatus => PaymentStatus.Approved;
        public override double Price => _price.GetValueOrDefault((double)_oldPrice);


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
        BuyPoints,
        Course

    }
}
