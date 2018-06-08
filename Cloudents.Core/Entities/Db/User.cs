using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ,Justification = "Resharper")]
    public class User
    {

        public User(string email, string name, string privateKey)
        {
            Email = email;
            Name = name;
            PrivateKey = privateKey;
            TwoFactorEnabled = true;
        }

        protected User()
        {

        }

        public virtual long Id { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }
        public virtual string NormalizedEmail{ get; set; }
        public virtual string SecurityStamp{ get; set; }

        public virtual string Image { get; set; }

        public virtual University University { get; set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string AuthenticatorKey { get; set; }

        public virtual string PrivateKey { get; set; }
    }
}
