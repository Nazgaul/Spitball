﻿using System;

namespace Cloudents.Core.Entities.Db
{
    public class UserLocation
    {
        public UserLocation(User user, string ip, string country)
        {
            User = user;
            Ip = ip;
            Country = country;
            TimeStamp = new DomainTimeStamp();
        }

        protected UserLocation()
        {
            
        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual DomainTimeStamp TimeStamp { get; set; }
        public virtual string Ip { get; set; }
        public virtual string Country { get; set; }
    }
}