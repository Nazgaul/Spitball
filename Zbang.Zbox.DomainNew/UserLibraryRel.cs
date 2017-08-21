using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class UserLibraryRel
    {
        protected UserLibraryRel()
        {

        }

        public UserLibraryRel(Guid id, User user, Library library, UserLibraryRelationType userType)
        {
            User = user;
            Library = library;
            UserType = userType;
            Id = id;
        }

        public virtual Guid Id { get; protected set; }
        public virtual UserLibraryRelationType UserType { get; set; }

        public virtual User User { get; set; }
        public virtual Library Library { get; set; }
    }
}
