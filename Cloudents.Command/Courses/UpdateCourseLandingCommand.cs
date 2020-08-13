using System;
using System.Runtime.Serialization;

namespace Cloudents.Command.Courses
{
    public class UpdateCourseLandingCommand : ICommand
    {
        [NonSerialized]
        public long UserId;

        [NonSerialized]
        public long CourseId;

        public HeroSection HeroSection { get; set; }
    }

    public class HeroSection
    {
        public string Name { get; set; }
        public string Button { get; set; }
        public string Image { get; set; }
    }
}