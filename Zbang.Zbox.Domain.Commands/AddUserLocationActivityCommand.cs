using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddUserLocationActivityCommand : ICommandAsync
    {
        public AddUserLocationActivityCommand(string ipAddress, long userId, string userAgent)
        {
            IpAddress = ipAddress;
            UserId = userId;
            UserAgent = userAgent;
        }
        public AddUserLocationActivityCommand(long userId, string userAgent)
        {
            UserId = userId;
            UserAgent = userAgent;
        }

        public string IpAddress { get; private set; }
        public long UserId { get; private set; }
        public string UserAgent { get; private set; }

        public string Country { get; set; }
    }
}
