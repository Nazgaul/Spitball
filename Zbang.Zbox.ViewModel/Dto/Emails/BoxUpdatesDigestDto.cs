using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Dto.Emails
{
    public class BoxUpdatesDigestDto
    {
        public IEnumerable<ItemDigestDto> Items { get; set; }

        public IEnumerable<QuizDigestDto> Quizzes { get; set; }

        public IEnumerable<QuizDiscussionDigestDto> QuizDiscussions { get; set; }

        public IEnumerable<QnADigestDto> BoxComments { get; set; }

        public IEnumerable<QnADigestDto> BoxReplies { get; set; }
    }
}
