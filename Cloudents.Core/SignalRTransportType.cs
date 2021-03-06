﻿using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core
{
    public class SignalRTransportType
    {
        private SignalRTransportType(SignalRType type, SignalRAction action, object[] data)
        {
            Type = type;
            Action = action;
            Data = data;
        }

        public SignalRTransportType(SignalRType type, SignalRAction action, object data) : this(type, action, new[] { data })
        {
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



        public SignalRType Type {  get; }
        public SignalRAction Action {  get; set; }

        public object[] Data {  get; }
    }



    public enum SignalRAction
    {
        Add,
        Update,
        Action
    }

    public enum SignalREventAction
    {
        Logout,
        Toaster,
        OnlineStatus,
       // StartSession,
        PaymentReceived,
       // EnterStudyRoom
    }

    public enum SignalRType
    {
        User,
        System,
        Chat,
        StudyRoom
    }
}
