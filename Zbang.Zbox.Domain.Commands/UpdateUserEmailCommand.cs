using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserEmailCommand : ICommand
    {

        public UpdateUserEmailCommand(long id, string email,bool tempFromFacebookLogin=false)
        {
            Id = id;
            Email = email;
            TempFromFacebookLogin = tempFromFacebookLogin;
        }
        public long Id { get; private set; }
        public string Email { get; private set; }

        public bool TempFromFacebookLogin { get; private set; }

    }
}
