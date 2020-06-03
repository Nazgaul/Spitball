using Cloudents.Core.Entities;
using System.Net;

namespace Cloudents.Command.Command
{
    public class AddUserLocationCommand : ICommand
    {
        public AddUserLocationCommand(User user,  IPAddress ip,
            string? userAgent)
        {
            User = user;
           
            Ip = ip;
            UserAgent = userAgent;
        }

        public User User { get; }

       
        public IPAddress Ip { get;  }
        public string? UserAgent { get;  }
    }
}