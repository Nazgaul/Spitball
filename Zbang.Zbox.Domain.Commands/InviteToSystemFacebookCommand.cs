using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class InviteToSystemFacebookCommand : ICommand
    {

        public InviteToSystemFacebookCommand(long senderId, long recipientFacebookUserId,
            string recipientFacebookUserName, string firstName, string middleName, string lastName, bool sex)
        {
            SenderId = senderId;
            FacebookUserId = recipientFacebookUserId;
            FacebookUserName = recipientFacebookUserName;
            //FacebookName = recepientFacebookName;

            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Sex = sex;
        }


        public long SenderId { get; private set; }
        public long FacebookUserId { get; private set; }
        public string FacebookUserName { get; private set; }
        //public string FacebookName { get; private set; }

        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public bool Sex { get; private set; }

    }
}
