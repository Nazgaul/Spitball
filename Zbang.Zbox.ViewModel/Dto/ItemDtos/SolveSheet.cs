using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class SolveSheet
    {
        public SolveSheet(long timeTaken, int score)
        {
            TimeTaken = TimeSpan.FromTicks(timeTaken);
            Score = score;
        }
        public TimeSpan TimeTaken { get; set; }
        public int Score { get; set; }
        public IEnumerable<SolveQuestion> Questions { get; set; }

        public QuizUserStats Stats { get; set; }
      
    }

    public class QuizUserStats
    {
        public float UserPosition { get; set; }

        public float Stdevp { get; set; }

        public float Avg { get; set; }
    }
}