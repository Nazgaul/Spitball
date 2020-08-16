using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cloudents.Command.Courses
{
    public class UpdateCourseLandingCommand : ICommand
    {
        [NonSerialized]
        public long UserId;

        [NonSerialized]
        public long CourseId;

        public HeroSection? HeroSection { get; set; }

        public IEnumerable<LiveClassSection>? LiveClassSection { get; set; }

        public ClassContent? ClassContent { get; set; }
    }

    public class HeroSection
    {
        public string Name { get; set; }
        public string? Button { get; set; }
        public string? Image { get; set; }

        public string Description { get; set; }
    }

    public class LiveClassSection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

    }

    public class ClassContent
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}