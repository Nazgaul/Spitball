using System;

namespace Cloudents.Command.Command
{
    public class CreateShortUrlCommand : ICommand
    {
        public CreateShortUrlCommand(string identifier, string destination, DateTime? expiration)
        {
            Identifier = identifier;
            Destination = destination;
            Expiration = expiration;
        }

        public string Identifier { get; }
        public string Destination { get; }
        public DateTime? Expiration { get; }
    }
}