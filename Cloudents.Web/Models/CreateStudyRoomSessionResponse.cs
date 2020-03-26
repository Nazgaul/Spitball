namespace Cloudents.Web.Models
{
    public class CreateStudyRoomSessionResponse
    {
        public CreateStudyRoomSessionResponse(string jwtToken)
        {
            JwtToken = jwtToken;
        }

        public string   JwtToken { get; }
    }
}
