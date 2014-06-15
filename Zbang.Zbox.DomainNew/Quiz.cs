using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class Quiz
    {
        protected Quiz()
        {

        }
        public Quiz(string name, long id, Box box, User owner)
            : this()
        {
            Id = id;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = null;
            }
            if (name != null)
            {
                name = name.Trim();
               
            }
            Name = name;
            Owner = owner;
            Box = box;

            DateTimeUser = new UserTimeDetails(owner.Email);
        }
        public virtual long Id { get; private set; }
        public virtual string Name { get; private set; }

        public virtual bool Publish { get; set; }

        public virtual User Owner { get; private set; }
        public virtual Box Box { get; private set; }

        public virtual string Content { get; set; }

// ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual float Rate { get; private set; }

        public virtual int NumberOfViews { get; private set; }
        public virtual int NumberOfComments { get; private set; }
        public virtual UserTimeDetails DateTimeUser { get; private set; }
        public virtual ICollection<Question> Questions { get; private set; }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual void UpdateText(string newText)
        {
            //Throw.OnNull(newText, "newText", false);
            if (newText != null)
            {
                newText = newText.Trim();
            }
            Name = newText;
            DateTimeUser.UpdateTime = DateTime.UtcNow;
        }
        public virtual void UpdateNumberOfViews()
        {
            NumberOfViews++;
        }

        
    }
}
