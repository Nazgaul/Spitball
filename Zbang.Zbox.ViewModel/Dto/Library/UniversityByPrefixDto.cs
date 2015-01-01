
using System;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class UniversityByPrefixDto : IEquatable<UniversityByPrefixDto>
    {
        public UniversityByPrefixDto(string name, string image, long id)
        {
            Name = name;
            Image = image;
            Id = id;
        }
        public string Name { get; private set; }
        public string Image { get; private set; }
        public long Id { get; private set; }


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
