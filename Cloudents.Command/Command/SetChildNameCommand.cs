namespace Cloudents.Command.Command
{
    public class SetChildNameCommand : ICommand
    {
        public SetChildNameCommand(long userId, string firstName, string lastName, short grade)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Grade = grade;
        }
        public long UserId { get;  }
        public string FirstName { get;  }
        public string LastName { get; }

        public short Grade { get;  }
    }
}
