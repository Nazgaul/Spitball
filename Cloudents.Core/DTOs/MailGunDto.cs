using JetBrains.Annotations;

namespace Cloudents.Core.DTOs
{
    [UsedImplicitly]
    public class MailGunDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string MailBody { get; set; }

        public string MailSubject { get; set; }

        public string MailCategory { get; set; }
    }
}