using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class RegisterMobileDeviceCommand : ICommand
    {
        public RegisterMobileDeviceCommand(long userId, MobileOperatingSystem operatingSystem)
        {
            OperatingSystem = operatingSystem;
            UserId = userId;
        }

        public long UserId { get; private set; }
        public MobileOperatingSystem OperatingSystem { get; private set; }
    }
}