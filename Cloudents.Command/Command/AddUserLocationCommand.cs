using Cloudents.Core.Entities;
using System.Net;

namespace Cloudents.Command.Command
{
    public class AddUserLocationCommand : ICommand
    {
        public AddUserLocationCommand(User user, string country, IPAddress ip,
            string userAgent)
        {
            User = user;
            Country = country;
            Ip = ip;
            UserAgent = userAgent;
        }

        public User User { get; }

        public string Country { get; set; }
        public IPAddress Ip { get; set; }
        public string UserAgent { get; protected set; }
    }
}