using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class SolvedQuestion
    {
        protected SolvedQuestion()
        {

        }
        public SolvedQuestion(Guid id, User user, Question question,Answer answer, bool correct, SolvedQuiz solvedQuiz)
        {

        }
        public Guid Id { get; set; }
        public User User { get; set; }
        public Question Question { get; set; }
        public Answer Answer { get; set; }
        public Quiz Quiz { get; set; }
        public bool Correct { get; set; }
        public SolvedQuiz SolvedQuiz { get; set; }
    }
}
