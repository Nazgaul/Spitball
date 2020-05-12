using System;

namespace Cloudents.Command.Command.Admin
{
    public class UpdateNameCommand : ICommand
    {
        public UpdateNameCommand(long userId, string firstName, string lastName)
        {
            UserId = userId;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }
        public long UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}
