using System;
using System.Collections;

namespace Cloudents.Core
{
    public class SignalRTransportType
    {
        public SignalRTransportType(SignalRType type, SignalRAction action, IEnumerable data)
        {
            Type = type;
            Action = action;
            Data = data;
        }

        public SignalRTransportType(SignalRType type, SignalRAction action, object data)
        {
            Type = type;
            Action = action;
            Data = new[] { data };
        }

        public SignalRType Type { get; }
        public SignalRAction Action { get; set; }

        public IEnumerable Data { get; }
    }



    public enum SignalRAction
    {
        Add,
        Delete,
        Update,
        Action
    }

    public enum SignalRType
    {
        Question,
        Answer,
        Document,
        User
    }
}
