﻿
using System;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class UniversityByPrefixDto : IEquatable<UniversityByPrefixDto>
    {
        public UniversityByPrefixDto(string name, string image, long id, long memberCount)
        {
            Name = name;
            Image = image;
            Id = id;
            MemberCount = memberCount;
        }
        public string Name { get; private set; }
        public string Image { get; private set; }
        public long Id { get; private set; }

        //public bool NeedCode { get; private set; }
        public long MemberCount { get; private set; }

        //public string Country { get; set; }
        public bool Equals(UniversityByPrefixDto other)
        {
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            //Get hash code for the Code field.
            return Id.GetHashCode();

        }
    }

    public class UniversityLuceneDto
    {
        public string Image { get; private set; }
        public long MemberCount { get; private set; }

        public string Name { get; set; }

        public long Id { get; set; }
    }
}
