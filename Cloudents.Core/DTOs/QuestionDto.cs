using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class QuestionDto
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Subject { get; set; }
        public int SubjectId { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public int Files { get; set; }
        [DataMember] public int Answers { get; set; }
        [DataMember] public UserDto User { get; set; }

        [DataMember] public DateTime DateTime { get; set; }

        [DataMember] public QuestionColor? Color { get; set; }

        [DataMember] public bool HasCorrectAnswer { get; set; }
    }


    public class QuestionDtoEqualityComparer : IEqualityComparer<QuestionDto>
    {
        public bool Equals(QuestionDto x, QuestionDto y)
        {
            return y != null && (x != null && x.Id == y.Id);
        }

        public int GetHashCode(QuestionDto obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}