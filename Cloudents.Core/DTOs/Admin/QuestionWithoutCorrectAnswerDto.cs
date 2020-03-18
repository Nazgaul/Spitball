using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs.Admin
{
    public class QuestionWithoutCorrectAnswerDto
    {


        public long Id { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public bool IsFictive { get; set; }
        public DateTime Create { get; set; }

        public int ImagesCount { get; set; }


        public IEnumerable<AnswerOfQuestionWithoutCorrectAnswer> Answers { get; set; }
    }

    [DataContract]
    public class AnswerOfQuestionWithoutCorrectAnswer
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Text { get; set; }

        public long QuestionId { get; set; }

        [DataMember]
        public int ImagesCount { get; set; }

    }
}