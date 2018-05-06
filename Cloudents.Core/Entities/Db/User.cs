using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Entities.Db
{
    public class User
    {
        public virtual long Id { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PublicKey { get; set; }
        public virtual string PhoneNumberHash { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }
        public virtual string NormalizedEmail{ get; set; }
    }
}
