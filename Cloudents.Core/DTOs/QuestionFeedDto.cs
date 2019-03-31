using System;
using System.Globalization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class QuestionFeedDto
    {
       // private bool _isRtl;

        

        public long Id { get; set; }
        public QuestionSubject? Subject { get; set; }
        public decimal Price { get; set; }
        public string Text { get; set; }
        public int Files { get; set; }
        public int Answers { get; set; }
        public UserDto User { get; set; }

        public DateTime DateTime { get; set; }

        public string Course { get; set; }

        public bool HasCorrectAnswer { get; set; }

        public bool IsRtl
        {
            get => CultureInfo?.TextInfo.IsRightToLeft ?? false;
        }

        public CultureInfo CultureInfo { get; set; }

        public VoteDto Vote { get; set; }
    }
}