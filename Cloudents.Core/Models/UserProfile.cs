using System;
using System.Collections.Generic;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Models
{
    public class UserProfile
    {
        public IEnumerable<string>? Courses { get; set; }


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