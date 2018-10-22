using System.Net;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class AddUserLocationCommand : ICommand
    {
        public AddUserLocationCommand(User user, string country, IPAddress ip)
        {
            User = user;
            Country = country;
            Ip = ip;
        }

        public User User { get; }

        public string Country { get; set; }
        public IPAddress Ip { get; set; }
    }
}