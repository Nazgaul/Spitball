using Cloudents.Core.DTOs;
using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    public class CreateAnswerResponse
    {
        public IEnumerable<QuestionFeedDto> NextQuestions { get; set; }
    }


    public class CoursesResponse
    {
        public IEnumerable<CourseDto> Courses { get; set; }
    }


    public class CoursesCreateResponse
    {
        public long Id { get; set; }
    }


    public class CallingCallResponse
    {
        public CallingCallResponse(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }

    public enum NextStep
    {
        EmailConfirmed,
        VerifyPhone,
        EnterPhone,
        EmailPassword,
        ExpiredStep,
        // ReSharper disable once IdentifierTypo - this is because client sucks
        Loginstep
    }
}
