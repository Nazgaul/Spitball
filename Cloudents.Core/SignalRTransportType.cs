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

        //[JsonConverter(typeof(StringEnumConverter), true)]
        public SignalRType Type { get; }
        //[JsonConverter(typeof(StringEnumConverter), true)]
        public SignalRAction Action { get; set; }

        public IEnumerable Data { get; }
    }



    public enum SignalRAction
    {
        Add,
        Delete,
        Update
    }

    public enum SignalRType
    {
        Question
    }
}
