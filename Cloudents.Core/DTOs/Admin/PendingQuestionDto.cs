using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class PendingQuestionDto
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }
    }
}
