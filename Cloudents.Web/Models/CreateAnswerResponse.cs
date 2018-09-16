using Cloudents.Core.DTOs;
using System.Collections.Generic;

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
        public UploadAskFileResponse(IEnumerable<string> files)
        {
            Files = files;
        }

        public IEnumerable<string> Files { get; set; }
    }

    public enum NextStep
    {
        EmailConfirmed,
        VerifyPhone,
        EnterPhone
    }
}
