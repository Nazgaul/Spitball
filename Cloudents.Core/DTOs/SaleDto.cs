using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs
{
    public abstract class SaleDto
    {
        public virtual string Type { get; set; }
        public virtual string Status { get; set; }
        public DateTime Date { get; set; }
        public decimal? Price { get; set; }
    }

    public class DocumentSaleDto : SaleDto
    {
        public string Course { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Preview { get; set; }
        public override string Status => "Paid";
    }

    public class QuestionSaleDto : SaleDto
    {
        public string Course { get; set; }
        public string Text { get; set; }
        public string AnswerText { get; set; }
        public override string Status => "Paid";
        public override string Type => "Question";
    }
    public class SessionSaleDto : SaleDto
    {
        public string StudentName { get; set; }
        public TimeSpan? Duration { get; set; }
        public override string Type => "TutoringSession";
    }
}
