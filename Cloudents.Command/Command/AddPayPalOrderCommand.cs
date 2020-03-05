
using System;

namespace Cloudents.Command.Command
{
    public class AddPayPalOrderCommand : ICommand
    {
        public AddPayPalOrderCommand(string token, Guid roomId)
        {
            Token = token;
            RoomId = roomId;
        }
        public string Token { get; }
        public Guid RoomId { get; }
    }
}
