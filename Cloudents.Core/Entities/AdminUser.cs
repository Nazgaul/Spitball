using System;

namespace Cloudents.Core.Entities
{
    public class AdminUser
    {
        public virtual Guid Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string Country { get; set; }
        public virtual AdminLanguage Language { get; set; }

    }
}