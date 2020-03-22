using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class FlaggedQuestionDto
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string? Reason { get; set; }
        public string FlaggedUserEmail { get; set; }
    }

    public class FlaggedDocumentDto
    {
        public long Id { get; set; }
        public string Reason { get; set; }
        public string FlaggedUserEmail { get; set; }
        public Uri Preview { get; set; }
        public string SiteLink { get; set; }
    }

    public class FlaggedAnswerDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string Reason { get; set; }
        public string FlaggedUserEmail { get; set; }
        public string MarkerEmail { get; set; }
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }
    }
}
