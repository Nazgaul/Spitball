using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class Invitation
    {
        internal Invitation()
        {

        }

        public Invitation(User sender, string recipientEmail)
        {
            this.RecipientEmail = recipientEmail;
            this.Sender = sender;
            this.CreationTimeUtc = DateTime.UtcNow;
            this.Permission = UserPermissionSettings.ReadWrite;
        }

        public virtual string RecipientEmail { get; set; }

        public virtual User Sender { get; set; }

        public virtual DateTime CreationTimeUtc { get; set; }

        public virtual int Id { get; set; }

        public virtual UserPermissionSettings Permission { get; set; }

        public override bool Equals(object obj)
        {
            ShareBoxInvitation other = obj as ShareBoxInvitation;
            if (other == null)
                return false;
            return Sender.Equals(other.Sender) && RecipientEmail.Equals(other.RecipientEmail);
        }
        public override int GetHashCode()
        {
            int result = 1;
            const int prime = 37;

            result = prime * result + Sender.GetHashCode();
            result += prime * result + RecipientEmail.GetHashCode();            

            return result;
        }
    }
}
