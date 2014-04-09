using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetQuizQuery
    {
        public GetQuizQuery(long quizId)
        {
            QuizId = quizId;
        }
        public long QuizId { get; set; }
    }
}
