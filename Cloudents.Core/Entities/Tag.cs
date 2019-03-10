using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public class Tag :Entity<string>
    {
        public const int MinLength = 2;
        public const int MaxLength = 150;
        protected Tag()
        {

        }

        public static bool ValidateTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return false;
            }
            tag = tag.Trim();
            if (tag.Length > MaxLength || tag.Length < MinLength)
            {
                return false;
            }

            if (tag.Contains(","))
            {
                return false;
            }

            if (tag.Count(x => x == ' ') > 6)
            {
                return false;
            }

            return true;
        }

        
        protected bool Equals(Tag other)
        {
            return string.Equals(Name.Trim(), other.Name.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Tag) obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode Nhibernate
            var name = Name;
            return (name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(name.Trim()) : 0);
        }
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

        public Tag(string name)
        {
            if (ValidateTag(name))
            {
                Name = name.Trim();
            }
            else
            {
                throw new ArgumentException($"not a valid tag name: {name}",nameof(name));
            }
        }


        public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }

    }
}