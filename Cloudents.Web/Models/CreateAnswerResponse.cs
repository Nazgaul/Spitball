using Cloudents.Core;

namespace Cloudents.Web.Models
{
    //public class CreateAnswerResponse
    //{
    //    public IEnumerable<QuestionFeedDto> NextQuestions { get; set; }
    //}


    //public class CoursesCreateResponse
    //{
    //    public long Id { get; set; }
    //}


    public class CallingCallResponse
    {
        public CallingCallResponse(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }

    //public enum NextStep
    //{
    //    EmailConfirmed,
    //    VerifyPhone,
    //    EnterPhone,
    //    EmailPassword,
    //    ExpiredStep,
    //    // ReSharper disable once IdentifierTypo - this is because client sucks
    //    Loginstep,
    //    StartStep
    //}
}
