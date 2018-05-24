using System.Collections.Generic;

namespace Cloudents.Core.Entities.Chat
{
    public class User
    {
        public string Name { get; set; }
        public string[] Email { get; set; }
        public string WelcomeMessage { get; set; }
        public string PhotoUrl { get; set; }
        public string Configuration { get; set; }
        public string[] Phone { get; set; }
        public IDictionary<string,string> Custom { get; set; }
    }
}