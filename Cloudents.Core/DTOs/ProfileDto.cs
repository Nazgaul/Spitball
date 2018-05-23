using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class ProfileDto
    {
        public UserProfileDto User { get; set; }

        public IEnumerable<QuestionDto> Ask { get; set; }
        public IEnumerable<QuestionDto> Answer { get; set; }
    }

   
}