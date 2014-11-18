using System;

namespace Zbang.Zbox.Domain
{
    public class Message
    {

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
}
