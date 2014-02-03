using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class Subscriber
    {
        internal protected Subscriber()
        {
            //Rule = new NotificationRules();
        }
        public virtual Box Box { get; set; }
        public virtual int SubscriberId { get; set; }
        public virtual UserPermissionSettings permission { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<NotificationRules> Rule { get; set; }



        public override bool Equals(object obj)
        {
            Subscriber subscriber = obj as Subscriber;
            if (subscriber == null) return false; 
            
            if (!this.Box.Equals(subscriber.Box)) return false;
            if (!this.User.Equals(subscriber.User)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            //return base.GetHashCode();
            unchecked
            {
                int result;
                result = Box.GetHashCode();
                result = 29 * result + User.GetHashCode();
                return result;
            }
        }

    }
}
