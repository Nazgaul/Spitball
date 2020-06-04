namespace Cloudents.Web.Models
{
    public class CallingCallResponse
    {
        public CallingCallResponse(string? code, string? country)
        {
            Code = code;
            Country = country;
        }

        public string? Code { get;  }

        public string? Country { get; }
    }
}
