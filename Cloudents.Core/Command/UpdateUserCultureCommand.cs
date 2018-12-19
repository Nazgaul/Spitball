using System.Globalization;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command
{
    public class UpdateUserCultureCommand : ICommand
    {
        public UpdateUserCultureCommand(long userId, CultureInfo cultureInfo)
        {
            UserId = userId;
            CultureInfo = cultureInfo;
        }

        public long UserId { get;  }

        public CultureInfo CultureInfo { get; }
    }
}