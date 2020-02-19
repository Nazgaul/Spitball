using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class UpdateUserCultureCommand : ICommand
    {
        public UpdateUserCultureCommand(long userId, Language cultureInfo)
        {
            UserId = userId;
            CultureInfo = cultureInfo;
        }

        public long UserId { get; }

        public Language CultureInfo { get; }
    }
}