using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Emails
{
    public class QuizDiscussionDigestDto
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public long UserId { get; set; }

        public string QuizName { get; set; }
        public long QuizId { get; set; }
    }

}
