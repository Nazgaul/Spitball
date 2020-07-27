using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cloudents.Persistence")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class OldCourse : Entity<string>
    {
        public const int MinLength = 4;
        public const int MaxLength = 150;


        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected OldCourse()
        {
        }

        //public OldCourse(string name)
        //{
        //    Id = name.Trim();//.Replace("+", string.Empty);
        //    if (Id.Length > MaxLength || Id.Length < MinLength)
        //    {
        //        throw new ArgumentException($"Name is {Id}", nameof(Id));
        //    }
        //    // State = ItemState.Pending;
        //    Created = DateTime.UtcNow;
        //    Users = new HashSet<UserCourse>();
        //}

        protected bool Equals(OldCourse? other)
        {
            return string.Equals(Id, other?.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((OldCourse)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Nhibernate")]
        public override int GetHashCode()
        {
            return Id != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Id) : 0;
        }

        public virtual int Count { get; protected internal set; }

        public virtual DateTime Created { get; protected set; }

       // protected internal virtual ISet<UserCourse> Users { get; set; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nhibernate proxy")]
        public virtual byte[] Version { get; protected set; }
    }
}
