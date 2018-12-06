using System.Diagnostics.CodeAnalysis;


namespace Cloudents.Core.Message.System
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Json serialize")]
    public class SignalRMessageTransport : ISystemQueueMessage
    {
        public SignalRMessageTransport(SignalRType messageType, SignalRAction action, object data)
        {
            Action = action;
            Data = data;
            MessageType = messageType;
        }


        public SignalRAction Action { get; set; }
        public SignalRType MessageType { get; set; }

        public object Data { get; }

        public long? UserId { get; set; }

        //new SignalRTransportType<QuestionDto>(SignalRType.Question, SignalRAction.Add, dto)

        public dynamic GetData()
        {
            return new SignalRTransportType(MessageType, Action, Data);
        }
    }
}