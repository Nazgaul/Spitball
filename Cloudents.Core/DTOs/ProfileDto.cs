using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto")]
    public class ProfileDto
    {
        public UserProfileDto User { get; set; }

        public IEnumerable<QuestionFeedDto> Questions { get; set; }
        public IEnumerable<QuestionFeedDto> Answers { get; set; }

    }
}