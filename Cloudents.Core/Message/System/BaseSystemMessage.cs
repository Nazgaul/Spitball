namespace Cloudents.Core.Message.System
{
    public abstract class BaseSystemMessage
    {
        public abstract SystemMessageType Type { get; }

        public abstract dynamic GetData();
    }
}