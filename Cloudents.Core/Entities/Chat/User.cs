using System.Collections.Generic;

namespace Cloudents.Core.Entities.Chat
{
    public class User
    {
        public User(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public string[] Email { get; set; }
        public string WelcomeMessage { get; set; }
        public string PhotoUrl { get; set; }
        public string Configuration => "buyer";
        public string[] Phone { get; set; }
        public IDictionary<string,string> Custom { get; set; }
    }
}