using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Web.Models
{
    public class CreateAnswerResponse
    {
        public IEnumerable<QuestionDto> NextQuestions { get; set; }
    }


    public class CoursesResponse
    {
        public IEnumerable<CourseDto> Courses { get; set; }
    }


    public class CoursesCreateResponse
    {
        public long Id { get; set; }
    }


    public class ReturnSignUserResponse
    {
        public ReturnSignUserResponse(NextStep step, bool isNew)
        {
            Step = step;
            IsNew = isNew;
        }

        public NextStep Step { get; set; }
        public bool IsNew { get; set; }

    }

    public class CallingCallResponse
    {
        public CallingCallResponse(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }

    public class UniversityResponse
    {
        public UniversityResponse(IEnumerable<UniversityDto> universities)
        {
            Universities = universities;
        }

        public IEnumerable<UniversityDto> Universities { get; set; }
    }

    public class UploadAskFileResponse
    {
        public UploadAskFileResponse(string[] files)
        {
            Files = files;
        }

        public string[] Files { get; set; }
    }

    public enum NextStep
    {
        EmailConfirmed,
        VerifyPhone,
        EnterPhone
    }
}
