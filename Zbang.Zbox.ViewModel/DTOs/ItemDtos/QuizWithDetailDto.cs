using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class QuizWithDetailDto
    {
        public QuizWithDetailDto()
        {
            Questions = new List<QuestionWithDetailDto>();
            
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public string Owner { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfViews { get; set; }
        public float Rate { get; set; }


        public IEnumerable<QuestionWithDetailDto> Questions { get; set; }
        

    }

    public class QuestionWithDetailDto
    {
        public QuestionWithDetailDto()
        {
            Answers = new List<AnswerWithDetailDto>();
        }
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid CorrectAnswer { get; set; }

        public List<AnswerWithDetailDto> Answers { get; set; }
    }

    public class AnswerWithDetailDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid QuestionId { get; set; }
    }
}
