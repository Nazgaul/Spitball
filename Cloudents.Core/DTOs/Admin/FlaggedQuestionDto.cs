using System;

namespace Cloudents.Application.DTOs.Admin
{
    public class FlaggedQuestionDto
    {
        public long Id { get; set; }

        public string Reason { get; set; }
        public string FlaggedUserEmail { get; set; }
    }

    public class FlaggedDocumentDto
    {
        public long Id { get; set; }
        public string Reason { get; set; }
        public string FlaggedUserEmail { get; set; }
    }

    public class FlaggedAnswerDto
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
        public string FlaggedUserEmail { get; set; }
    }
}
