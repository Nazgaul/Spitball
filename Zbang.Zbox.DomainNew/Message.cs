using System;

namespace Zbang.Zbox.Domain
{
    public class Message
    {

        //protected Message(Guid id, User sender, User recipient)
        //{
        //    Id = id;
        //    Sender = sender;
        //    Recipient = recipient;
        //    CreationTime = DateTime.UtcNow;
        //    NotRead = false;
        //    New = true;
        //}

         protected Message()
        {

        }
        public Message(Guid id, User sender, User recipient, string text)
           
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            Id = id;
            Sender = sender;
            Recipient = recipient;
            CreationTime = DateTime.UtcNow;
            NotRead = false;
            New = true;
            Text = text;

        }
        public string Text { get; private set; }
// ReSharper disable once UnusedAutoPropertyAccessor.Local
        public DateTime? OpenTime { get; protected set; }
        public Guid Id { get; private set; }
        public virtual User Sender { get; private set; }
        public virtual User Recipient { get; private set; }
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

//    public class Invite : MessageBase
//    {
//        protected Invite()
//        {

//        }
//        public Invite(Guid id, User sender, User recipient, Box box)
//            : base(id, sender, recipient)
//        {
//            Box = box;
//            IsActive = true;
//        }
//        public virtual Box Box { get; private set; }
//// ReSharper disable once UnusedAutoPropertyAccessor.Local
//        public DateTime? OpenTime { get; protected set; }
//        public DateTime? SendTime { get; private set; }
//        public bool IsActive { get; internal set; }

//        public void UpdateSendTime()
//        {
//            SendTime = DateTime.UtcNow;
//        }
//    }

//    public class Message : MessageBase
//    {
//        protected Message()
//        {

//        }
//        public Message(Guid id, User sender, User recipient, string text)
//            : base(id, sender, recipient)
//        {
//            if (string.IsNullOrEmpty(text))
//            {
//                throw new ArgumentNullException("text");
//            }
//            Text = text;

//        }
//        public string Text { get; private set; }
//// ReSharper disable once UnusedAutoPropertyAccessor.Local
//        public DateTime? OpenTime { get; protected set; }

//    }

//    public class InviteToCloudents : MessageBase
//    {
//        protected InviteToCloudents()
//        {
//        }
//        public InviteToCloudents(Guid id, User sender, User recipient)
//            : base(id, sender, recipient)
//        {
//        }
//// ReSharper disable once UnusedAutoPropertyAccessor.Local
//        public DateTime? OpenTime { get; protected set; }
//    }
}
