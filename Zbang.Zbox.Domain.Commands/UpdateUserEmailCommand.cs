using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserEmailCommand : ICommandAsync
    {

        public UpdateUserEmailCommand(long id, string email)
        {
            Id = id;
            Email = email;
        }
        public long Id { get; private set; }
        public string Email { get; private set; }


    }
}
