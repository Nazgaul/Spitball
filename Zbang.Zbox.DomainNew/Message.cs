using System;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public abstract class MessageBase
    {
        protected MessageBase()
        {

        }
        protected MessageBase(Guid id, User sender, User recepient)
        {
            Id = id;
            Sender = sender;
            Recepient = recepient;
            CreationTime = DateTime.UtcNow;
            NotRead = false;
            New = true;
        }
        public Guid Id { get; private set; }
        public virtual User Sender { get; private set; }
        public virtual User Recepient { get; private set; }
        public DateTime CreationTime { get; private set; }
        public bool NotRead { get; private set; }
        public bool New { get; private set; }

        public void UpdateMessageAsRead()
        {
            NotRead = true;
        }
        public void UpdateMessageAsOld()
        {
            New = false;
        }


    }

    public class Invite : MessageBase
    {
        protected Invite()
        {

        }
        public Invite(Guid id, User sender, User recepient, Box box)
            : base(id, sender, recepient)
        {
            Box = box;
            IsActive = true;
        }
        public virtual Box Box { get; private set; }
// ReSharper disable once UnusedAutoPropertyAccessor.Local
        public DateTime? OpenTime { get; private set; }
        public DateTime? SendTime { get; private set; }
        public bool IsActive { get; internal set; }

        public void UpdateSendTime()
        {
            SendTime = DateTime.UtcNow;
        }
    }

    public class Message : MessageBase
    {
        protected Message()
        {

        }
        public Message(Guid id, User sender, User recepient, string text)
            : base(id, sender, recepient)
        {
            Throw.OnNull(text, "text");
            Text = text;

        }
        public string Text { get; private set; }
// ReSharper disable once UnusedAutoPropertyAccessor.Local
        public DateTime? OpenTime { get; private set; }

    }

    public class InviteToCloudents : MessageBase
    {
        protected InviteToCloudents()
        {
        }
        public InviteToCloudents(Guid id, User sender, User recepient)
            : base(id, sender, recepient)
        {
        }
// ReSharper disable once UnusedAutoPropertyAccessor.Local
        public DateTime? OpenTime { get; private set; }
    }
}
