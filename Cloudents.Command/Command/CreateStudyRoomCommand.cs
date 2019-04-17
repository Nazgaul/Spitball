namespace Cloudents.Command.Command
{
    public class CreateStudyRoomCommand : ICommand
    {
        public CreateStudyRoomCommand(long tutorId, long studentId)
        {
            TutorId = tutorId;
            StudentId = studentId;
        }

        public long TutorId { get; }
        public long StudentId { get; }
    }
}