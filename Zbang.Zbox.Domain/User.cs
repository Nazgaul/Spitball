using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class User
    {
        //Ctor
        protected User()
        {
            
        }

        public User(string email)
        {
            this.Email = email;
        }

        //Properties
        public virtual Guid UserId { get; protected set; }        
        public virtual string Email { get; protected set; }
        public virtual bool IsEmailVerified { get; protected set; }
        
        //Methods
        public override bool Equals(object obj)
        {
            User other = obj as User;
            if (other == null)
                return false;

            if (this.Email == null || other.Email == null)
                throw new InvalidOperationException("Cannot compare users with no emails");

            return Email.ToLower().Equals(other.Email.ToLower());
        }
        public override int GetHashCode()
        {
            return 31 * Email.GetHashCode();
        }

        public void MarkEmailAsVerified()
        {
            this.IsEmailVerified = true;
        }
    }
}
