using System.Net;
using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class AddUserLocationCommand : ICommand
    {
        public AddUserLocationCommand(RegularUser user, string country, IPAddress ip, 
            string fingerPrint, string userAgent)
        {
            User = user;
            Country = country;
            Ip = ip;
            FingerPrint = fingerPrint;
            UserAgent = userAgent;
        }

        public RegularUser User { get; }

        public string Country { get; set; }
        public IPAddress Ip { get; set; }
        public string FingerPrint { get; protected set; }
        public string UserAgent { get; protected set; }
    }
}