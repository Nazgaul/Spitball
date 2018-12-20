using System.Net;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Command
{
    public class AddUserLocationCommand : ICommand
    {
        public AddUserLocationCommand(RegularUser user, string country, IPAddress ip)
        {
            User = user;
            Country = country;
            Ip = ip;
        }

        public RegularUser User { get; }

        public string Country { get; set; }
        public IPAddress Ip { get; set; }
    }
}