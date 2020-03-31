namespace Cloudents.Web.Models
{
   

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
