namespace Cloudents.Command.Courses
{
    public class UpdateCoursePositionCommand : ICommand
    {
        public UpdateCoursePositionCommand(long tutorId, int oldPosition, int newPosition)
        {
            TutorId = tutorId;
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public long TutorId { get;  }
        public int OldPosition { get; }
        public int NewPosition { get; }
    }

    //public class UpdateDocumentPositionCommand : ICommand
    //{
    //    public UpdateDocumentPositionCommand(long tutorId, long courseId, int oldPosition, int newPosition)
    //    {
    //        TutorId = tutorId;
    //        CourseId = courseId;
    //        OldPosition = oldPosition;
    //        NewPosition = newPosition;
    //    }

    //    public long TutorId { get;  }
    //    public long CourseId { get; }
    //    public int OldPosition { get; }
    //    public int NewPosition { get; }
    //}
}