using System;
using System.Collections.Generic;

namespace Cloudents.Core.Models
{
    public class UserProfile
    {
        public IEnumerable<string>? Courses { get; set; }


      //  public Guid? UniversityId { get; set; }

        public string? Country { get; set; }

        public string CountryRegion
        {
            get
            {
                Cloudents.Core.Entities.Country c = Country;
                return c.Name;
            }
        }

    }

    //public class UserUniversityQueryProfileDto
    //{
    //    public UserUniversityQueryProfileDto()
    //    {
    //    }
    //    public UserUniversityQueryProfileDto(Guid id, string extraName, string name, string country)
    //    {
    //        Id = id;
    //        ExtraName = extraName?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Union(new[] { name });
    //        Name = name;
    //        Country = country;
    //    }

    //    public Guid Id { get;  set; }
    //    public IEnumerable<string> ExtraName { get;  set; }
    //    public string Name { get;  set; }

    //    public string Country { get;  set; }
    //}
}