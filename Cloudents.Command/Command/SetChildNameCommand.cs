namespace Cloudents.Command.Command
{
    public class SetChildNameCommand : ICommand
    {
        public SetChildNameCommand(long userId, string firstName,  short grade)
        {
            UserId = userId;
            FirstName = firstName;
            Grade = grade;
        }
        public long UserId { get;  }
        public string FirstName { get;  }

        public short Grade { get;  }
    }
}
