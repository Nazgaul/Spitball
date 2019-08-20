﻿
namespace Cloudents.Command.Command
{
    public class UpdatePhoneCommand : ICommand
    {
        public UpdatePhoneCommand(long userId, string newPhone)
        {
            UserId = userId;
            NewPhone = newPhone;
        }
        public long UserId{ get; }
        public string NewPhone { get; }
    }
}