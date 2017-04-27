using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class GetGeneralDepartmentCommand: ICommand
    {

        public GetGeneralDepartmentCommand(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}
