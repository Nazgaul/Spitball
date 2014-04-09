﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class SolvedQuiz
    {
        protected SolvedQuiz()
        {
            SolvedQuestion = new List<SolvedQuestion>();
        }
        public SolvedQuiz(Guid id, Quiz quiz, User user, TimeSpan timeTaken)
            : this()
        {
            Id = id;
            Quiz = quiz;
            User = user;
            TimeTaken = timeTaken;
            CreationTime = DateTime.UtcNow;

        }
        public virtual Guid Id { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual User User { get; set; }
        public virtual TimeSpan TimeTaken { get; set; }
        public virtual int Score { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual ICollection<SolvedQuestion> SolvedQuestion { get; set; }
    }
}

