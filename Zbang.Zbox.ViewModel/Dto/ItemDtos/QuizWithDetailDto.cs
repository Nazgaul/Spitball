using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class QuizWithDetailSolvedDto
    {
        public QuizWithDetailDto Quiz { get; set; }
        public SolveSheet Sheet { get; set; }
       
    }

    public class QuizQuestionWithSolvedAnswersDto
    {
        public SolveSheet Sheet { get; set; }
        public IEnumerable<QuestionWithDetailDto> Questions { get; set; }
        public IEnumerable<SolveQuestion> UserAnswers { get; set; }
    }

    

    public class QuizWithDetailDto
    {
        private DateTime m_Date;
        public string Name { get; set; }
        public long OwnerId { get; set; }

        public string Owner { get; set; }
       
        public long Id { get; set; }
        public DateTime Date
        {
            get { return m_Date; }
            set { m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }

        public int NumberOfViews { get; set; }

        //public string BoxUrl { get; set; }



        public bool Publish { get; set; }

        public IEnumerable<QuestionWithDetailDto> Questions { get; set; }


        public IEnumerable<QuizBestUser> TopUsers { get; set; }

        public string Banner { get; set; }

    }

    public class QuizBestUser
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class QuestionWithDetailDto
    {
        public QuestionWithDetailDto()
        {
            Answers = new List<AnswerWithDetailDto>();
        }
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid? CorrectAnswer { get; set; }

        public List<AnswerWithDetailDto> Answers { get; set; }
    }

    public class AnswerWithDetailDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid QuestionId { get; set; }
    }

    public class SolveQuestion
    {
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }
    }


    public class QuizSolversWithCountDto
    {
        public IEnumerable<QuizBestUser> Users { get; set; }
        public int SolversCount { get; set; }
    }
}
