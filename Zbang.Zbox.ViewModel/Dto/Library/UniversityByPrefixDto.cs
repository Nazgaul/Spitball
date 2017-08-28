
using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class UniversityByPrefixDto : IEquatable<UniversityByPrefixDto>
    {
        public string Name { get;  set; }
        public string Image { get;  set; }
        public long Id { get;  set; }

        public int NumOfUsers { get; set; }

        public IEnumerable<string> UserImages { get; set; }

        public bool Equals(UniversityByPrefixDto other)
        {
            if (ReferenceEquals(other, null)) return false;

            return ReferenceEquals(this, other) || Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
