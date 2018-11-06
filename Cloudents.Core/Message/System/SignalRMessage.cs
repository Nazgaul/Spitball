using System.Diagnostics.CodeAnalysis;
using NotImplementedException = System.NotImplementedException;

namespace Cloudents.Core.Message.System
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Json serialize")]
    public class SignalRMessage : BaseSystemMessage
    {
        public SignalRMessage(SignalRType messageType, SignalRAction action, object data)
        {
            Action = action;
            Data = data;
            MessageType = messageType;
        }

        protected SignalRMessage()
        {
            
        }
        public override SystemMessageType Type => SystemMessageType.SignalR;

        public SignalRAction Action { get; private set; }
        public SignalRType MessageType { get; private set; }

        public object Data { get; }

        //new SignalRTransportType<QuestionDto>(SignalRType.Question, SignalRAction.Add, dto)

        public override dynamic GetData()
        {
            return new SignalRTransportType(MessageType, Action, Data);
        }
    }
}