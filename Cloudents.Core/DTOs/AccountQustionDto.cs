using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs
{
    public class AccountQustionDto
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public QuestionUserDto User { get; set; }
    }
}
