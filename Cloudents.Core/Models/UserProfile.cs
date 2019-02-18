using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Cloudents.Core.Models
{
    public class UserProfile
    {
        public IList<string> Courses { get; set; }
        public IEnumerable<string> Tags { get; set; }

        [CanBeNull]
        public UserUniversityQueryProfileDto University { get; set; }

        public string Country { get; set; }
    }

    public class UserUniversityQueryProfileDto
    {
        public UserUniversityQueryProfileDto()
        {
        }
        public UserUniversityQueryProfileDto(Guid id, string extraName, string name, string country)
        {
            Id = id;
            ExtraName = extraName?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Union(new[] { name });
            Name = name;
            Country = country;
        }

        public Guid Id { get;  set; }
        public IEnumerable<string> ExtraName { get;  set; }
        public string Name { get;  set; }

        public string Country { get;  set; }
    }
}