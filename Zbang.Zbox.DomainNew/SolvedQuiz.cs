using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class SolvedQuiz
    {
        protected SolvedQuiz()
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            SolvedQuestions = new List<SolvedQuestion>();
        }
        public SolvedQuiz(Guid id, Quiz quiz, User user, TimeSpan timeTaken)
            : this()
        {
            Id = id;
            Quiz = quiz;
            User = user;
            TimeTaken = timeTaken;
            CreationTime = DateTime.UtcNow;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor


        }

        
        public virtual Guid Id { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual User User { get; set; }
        public virtual TimeSpan TimeTaken { get; set; }
        public virtual int Score { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual ICollection<SolvedQuestion> SolvedQuestions { get; set; }

        public virtual void AddSolvedQuestion(SolvedQuestion answer)
        {
            SolvedQuestions.Add(answer);
        }
    }
}

