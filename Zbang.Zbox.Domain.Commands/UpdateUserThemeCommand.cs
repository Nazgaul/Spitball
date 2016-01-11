using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserThemeCommand : ICommand
    {
        public UpdateUserThemeCommand(long id, Theme theme)
        {
            Theme = theme;
            Id = id;
        }

        public long Id { get; private set; }
        public Theme Theme { get; private set; }
    }
}
