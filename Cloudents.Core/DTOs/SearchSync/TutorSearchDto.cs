using System.Collections.Generic;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class TutorSearchDto
    {
        public TutorSearchDto(IList<string> courses, IList<string> subjects)
        {
            Courses = courses;
            Subjects = subjects;
        }

        public long Id { get; set; } //key readonly

        public string Name { get; set; } //key readonly
        public string Bio { get; set; } //key readonly
        public string Image { get; set; } //key readonly
        public double Price { get; set; } //key readonly
        public double Rate { get; set; } //key readonly
        public int ReviewsCount { get; set; } //key readonly
        public byte[] Version { get; set; } //key readonly
        public IList<string> Courses { get; set; } //key readonly
        public IList<string> Subjects { get; set; } //key readonly
    }
}