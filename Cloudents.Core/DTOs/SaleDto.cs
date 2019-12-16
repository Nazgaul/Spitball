﻿using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs
{
    public abstract class SaleDto
    {
        public virtual string Type { get; set; }
        public virtual string PaymentStatus { get; set; }
        public DateTime Date { get; set; }
        public decimal? Price { get; set; }
    }

    public class DocumentSaleDto : SaleDto
    {
        public string Course { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }
        public override string PaymentStatus => "Paid";
    }

    public class QuestionSaleDto : SaleDto
    {
        public long Id{ get; set; }
        public string Course { get; set; }
        public string Text { get; set; }
        public string AnswerText { get; set; }
        public override string PaymentStatus => "Paid";
        public override string Type => "Question";
    }
    public class SessionSaleDto : SaleDto
    {
        public string StudentName { get; set; }
        public TimeSpan? Duration { get; set; }
        public string StudentImage { get; set; }
        public long StudentId { get; set; }
        public override string Type => "TutoringSession";
    }
}
