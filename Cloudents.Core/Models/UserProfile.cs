using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Cloudents.Core.Models
{
    public class UserProfile
    {
        //public UserQueryProfileDto(IEnumerable<string> courses, 
        //    IEnumerable<string> tags, 
        //    UserUniversityQueryProfileDto university)
        //{
        //    Courses = courses;//.Select(s=>s.Name);
        //    Tags = tags;//.Select(s => s.Name);
        //    University = university;
        //}

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

        public Guid Id { get; set; }
        public IEnumerable<string> ExtraName { get; set; }
        public string Name { get; set; }

        public string Country { get; private set; }
    }
}