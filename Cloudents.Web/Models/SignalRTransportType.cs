using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class SignalRTransportType<T>
    {
        private readonly SignalRAction _action;
        public SignalRTransportType(string type, SignalRAction action, IEnumerable<T> data)
        {
            Type = type;
            _action = action;
            Data = data;
        }

        public SignalRTransportType(string type, SignalRAction action, T data)
        {
            Type = type;
            _action = action;
            Data = new[] { data };
        }

        public string Type { get; }

        public string Action => _action.ToString("G");
        public IEnumerable<T> Data { get; }
    }


    public enum SignalRAction
    {
        Add,
        Delete,
        Update
    }
}
