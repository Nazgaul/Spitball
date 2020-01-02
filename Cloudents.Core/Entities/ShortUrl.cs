using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nHibernate proxy")]
    public class ShortUrl : Entity<Guid>
    {
        public ShortUrl(string identifier, string destination, DateTime? expiration)
        {
            Identifier = identifier;
            Destination = destination;
            Expiration = expiration;
        }

        protected ShortUrl()
        {

        }

        public virtual string Identifier { get; set; }
        public virtual string Destination { get; set; }

        public virtual DateTime? Expiration { get; set; }

    }
}