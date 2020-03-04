using System;

namespace Cloudents.Core.DTOs.Users
{
    public abstract class UserPurchasDto
    {
        public virtual ContentType Type { get; set; }
        public decimal? Price { get; set; }
        public DateTime Date { get; set; }
    }

    public class PurchasedDocumentDto : UserPurchasDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Course { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }
    }

    public class PurchasedSessionDto : UserPurchasDto
    {
        public string TutorName { get; set; }
        public TimeSpan? Duration { get; set; }
        public string TutorImage { get; set; }
        public long TutorId { get; set; }
        public override ContentType Type => ContentType.TutoringSession;
    }
}
