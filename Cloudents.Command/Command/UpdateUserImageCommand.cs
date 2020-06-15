﻿namespace Cloudents.Command.Command
{
    public class UpdateUserImageCommand : ICommand
    {
        public UpdateUserImageCommand(long userId, string fileName)
        {
            UserId = userId;
            FileName = fileName;
        }

        public long UserId { get; }
        public string FileName { get; }
    }

    public class UpdateUserCoverImageCommand : ICommand
    {
        public UpdateUserCoverImageCommand(long userId, string fileName)
        {
            UserId = userId;
            FileName = fileName;
        }

        public long UserId { get; }
        public string FileName { get; }
    }
}