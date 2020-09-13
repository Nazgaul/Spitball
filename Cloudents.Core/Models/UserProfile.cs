using System.Collections.Generic;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Models
{
    public class UserProfile
    {
        //public HashSet<long>? Subscribers { get; set; }


        public string? Country { get; set; }

        public Country CountryRegion
        {
            get
            {
                Country c = Entities.Country.FromCountry(Country);
                return c;
            }
        }

    }

    
}