namespace Cloudents.Core.DTOs
{
    public class StudyRoomSeoDto
    {
        public string? CourseName { get; set; }
        public string? PrivateName { get; set; }

        public string Name => CourseName ?? PrivateName!;
        public string TutorName { get; set; }
        public string Description { get; set; }
    }
}