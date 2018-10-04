using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Attributes;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class University
    {
        public University(string name, string country) : this()
        {
            Name = name;
            Country = country;
            Pending = true;
            TimeStamp = new DomainTimeStamp();
        }

        [UsedImplicitly]
        protected University()
        {

        }

        public virtual long Id { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual string Name { get; set; }

        /// <summary>
        /// Used as extra synonym to add to university search
        /// </summary>
        public virtual string Extra { get; set; }
        /// <summary>
        /// Used in bing search as synonym
        /// </summary>
        public virtual string ExtraSearch { get; set; }

       // public virtual float? Latitude { get; set; }
       // public virtual float? Longitude { get; set; }

        public virtual  string Country { get; set; }

       // public virtual  string Image { get; set; }

        public virtual bool Pending { get; set; }

        public virtual DomainTimeStamp TimeStamp { get; set; }
    }
}
