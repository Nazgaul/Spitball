using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ,Justification = "Resharper")]
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
        public virtual string SecurityStamp{ get; set; }

        public virtual string Image { get; set; }

        public virtual University University { get; set; }
    }

    //public class UserRole
    //{
    //    public Guid Id { get; set; }
    //    public User User { get; set; }
    //    public string Name { get; set; }
    //}
}
