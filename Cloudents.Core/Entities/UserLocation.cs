using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public class UserLocation
    {
        public UserLocation(RegularUser user, string ip, string country)
        {
            User = user;
            Ip = ip;
            Country = country;
            TimeStamp = new DomainTimeStamp();
        }

        protected UserLocation()
        {

        }

        public virtual Guid Id { get; protected set; }
        public virtual RegularUser User { get; protected set; }
        public virtual DomainTimeStamp TimeStamp { get; protected set; }
        public virtual string Ip { get; protected set; }
        public virtual string Country { get; protected set; }
    }
}