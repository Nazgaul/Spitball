using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public class UserLocation : Entity<Guid>
    {
        public UserLocation(User user, string ip, string country,  string userAgent)
        {
            User = user;
            Ip = ip;
            Country = country;
            TimeStamp = new DomainTimeStamp();
            //FingerPrint = fingerPrint;
            UserAgent = userAgent;
        }

        protected UserLocation()
        {

        }

        //  public virtual Guid Id { get; protected set; }
        public virtual User User { get; protected set; }
        public virtual DomainTimeStamp TimeStamp { get; protected set; }
        public virtual string Ip { get; protected set; }
        public virtual string Country { get; protected set; }
        public virtual string UserAgent { get; protected set; }
    }
}