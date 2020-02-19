namespace Cloudents.Command.Command
{
    public class RequestTutorCommand : ICommand
    {
        public RequestTutorCommand(string course,
            string chatText,
            long userId,
            string referer,
            string leadText,

            long? tutorId,
            string utmSource, bool moreTutors)
        {
            Course = course;
            ChatText = chatText;

            UserId = userId;


            Referer = referer;
            LeadText = leadText;
            TutorId = tutorId;
            UtmSource = utmSource;
            MoreTutors = moreTutors;
        }

        public string Course { get; }
        public long UserId { get; }
        public string ChatText { get; }
        public string Referer { get; }
        public string LeadText { get; }
        public long? TutorId { get; }
        public string UtmSource { get; }

        public bool MoreTutors { get; }
    }


}