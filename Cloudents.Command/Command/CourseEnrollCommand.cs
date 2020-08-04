using System;

namespace Cloudents.Command.Command
{
    public class CourseEnrollCommand : ICommand
    {
        public CourseEnrollCommand(long userId, long courseId, string? receipt = null)
        {
            UserId = userId;
            CourseId = courseId;
            Receipt = receipt;
        }

        public long UserId { get;  }

        public long CourseId { get; }

        public string? Receipt { get; }

        
    }
}