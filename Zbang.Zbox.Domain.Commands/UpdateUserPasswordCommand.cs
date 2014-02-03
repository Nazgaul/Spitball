using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserPasswordCommand : ICommand
    {
        public UpdateUserPasswordCommand(long id, string currentPassword, string newPassword)
        {
            Id = id;
           
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
        public long Id { get; private set; }        
        public string CurrentPassword { get; private set; }
        public string NewPassword { get; private set; }
    }
}
