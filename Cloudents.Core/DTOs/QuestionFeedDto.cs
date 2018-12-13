using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class QuestionFeedDto
    {
        public QuestionFeedDto(long id, QuestionSubject subject, decimal price, string text, int files,
            int answers, UserDto user, DateTime dateTime, QuestionColor? color, bool hasCorrectAnswer, CultureInfo culture, int votes, int answerCount)
        :this(id,subject,price,text,files,answers,dateTime,color,hasCorrectAnswer,culture, votes, answerCount)
        {
          
            User = user;
            
        }

        public QuestionFeedDto(long id, QuestionSubject subject, decimal price, string text,
            int files, int answers, DateTime dateTime, QuestionColor? color, bool hasCorrectAnswer, CultureInfo culture,
            int votes, int answerCount)
        {
            Id = id;
            Subject = subject;
            Price = price;
            Text = text;
            Files = files;
            Answers = answers;
            DateTime = dateTime;
            Color = color;
            HasCorrectAnswer = hasCorrectAnswer;
            IsRtl = culture?.TextInfo.IsRightToLeft ?? false;
            Vote = new VoteDto()
            {
                Votes = votes
            };
            AnswerCount = answerCount;
        }

        
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "We need this for next question query")]
        public QuestionFeedDto()
        {
            
        }

        public long Id { get; set; }
        public QuestionSubject Subject { get; set; }
        public decimal Price { get; set; }
        public string Text { get; set; }
        public int Files { get; set; }
        public int Answers { get; set; }
        public UserDto User { get; set; }

        public DateTime DateTime { get; set; }

        public QuestionColor? Color { get; set; }

        public bool HasCorrectAnswer { get; set; }

        public bool IsRtl { get; set; }

        public VoteDto Vote { get; set; }
        public int AnswerCount{ get; set; }
    }
}