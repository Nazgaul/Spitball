using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
