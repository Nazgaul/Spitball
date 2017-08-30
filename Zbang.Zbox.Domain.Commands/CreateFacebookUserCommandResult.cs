
namespace Zbang.Zbox.Domain.Commands
{
    public class CreateFacebookUserCommandResult : CreateUserCommandResult
    {
        public CreateFacebookUserCommandResult(User user           
                )
            : base(user) { }

    }

    public class CreateGoogleUserCommandResult : CreateUserCommandResult
    {
        public CreateGoogleUserCommandResult(User user
                )
            : base(user) { }

    }
}
