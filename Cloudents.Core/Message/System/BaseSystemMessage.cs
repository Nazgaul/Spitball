using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Message.System
{
    public abstract class BaseSystemMessage
    {
        public abstract SystemMessageType Type { get; }

        public abstract dynamic GetData();
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Json serialize")]
    public class SignalRMessage : BaseSystemMessage
    {
        public SignalRMessage(SignalRType messageType, SignalRAction action, object data)
        {
            Action = action;
            Data = data;
            MessageType = messageType;
        }

        public override SystemMessageType Type => SystemMessageType.SignalR;

        public SignalRAction Action { get; }
        public SignalRType MessageType { get; }

        public object Data { get; }

        //new SignalRTransportType<QuestionDto>(SignalRType.Question, SignalRAction.Add, dto)

        public override dynamic GetData()
        {
            return new SignalRTransportType(MessageType, Action, Data);
        }
    }
}