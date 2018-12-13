using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class FlaggedQuestionDto
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }
    }

    public class FlaggedDocumentDto
    {
        public long Id { get; set; }
        public Uri Preview { get; set; }
    }

    public class FlaggedAnswerDto
    {
        public Guid Id { get; set; }

        public long UserId { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }
    }
}
