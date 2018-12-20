using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Item.Commands.FlagItem
{
    public class BaseFlagItemCommand<T> : ICommand
    {
        //protected BaseFlagItemCommand(long userId, T id, string flagReason)
        //{
        //    UserId = userId;
        //    Id = id;
        //    FlagReason = flagReason;
        //}

        //protected BaseFlagItemCommand( T id, string flagReason)
        //{
        //    Id = id;
        //    FlagReason = flagReason;
        //}

        public long? UserId { get; protected set; }
        public T Id { get; protected set; }
        public string FlagReason { get; protected set; }
    }
}