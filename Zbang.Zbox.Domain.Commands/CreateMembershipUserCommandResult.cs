namespace Zbang.Zbox.Domain.Commands
{
    public class CreateMembershipUserCommandResult : CreateUserCommandResult
    {
        public CreateMembershipUserCommandResult(User user) : base(user) { }
    }
}
