﻿using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core
{
    public class SignalRTransportType
    {
        public SignalRTransportType(SignalRType type, SignalRAction action, object[] data)
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

        public SignalRTransportType(SignalRType type, SignalREventAction action, object data)
        {
            Type = type;
            Action = SignalRAction.Action;

            Data = new object[]
            {
                new
                {
                    type = action.ToString("G").ToLowerInvariant(), data
                }
            };
        }
        
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Need for serialzation")]
        protected SignalRTransportType()
        {
            
        }

       

        public SignalRType Type { get; }
        public SignalRAction Action { get; set; }

        public object[] Data { get; }
    }



    public enum SignalRAction
    {
        Add,
        Delete,
        Update,
        Action
    }

    public enum SignalREventAction
    {
        Logout,
        MarkAsCorrect,
        Toaster
    }

    public enum SignalRType
    {
        Question,
        Answer,
        Document,
        User,
        System,
        Chat
    }
}
