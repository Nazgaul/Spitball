using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    public class SignalRTransportType<T>
    {
        public SignalRTransportType(string type, SignalRAction action, IEnumerable<T> data)
        {
            Type = type;
            Action = action;
            Data = data;
        }

        public SignalRTransportType(string type, SignalRAction action, T data)
        {
            Type = type;
            Action = action;
            Data = new[] { data };
        }

        public string Type { get; }

        public SignalRAction Action { get; set; }
    
        public IEnumerable<T> Data { get; }
    }


    public enum SignalRAction
    {
        Add,
        Delete,
        Update
    }
}
