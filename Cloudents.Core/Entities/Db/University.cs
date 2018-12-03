using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class University
    {
        public University(string name, string country) : this()
        {
            Name = name.Replace("+","");
            Country = country;
            RowDetail = new DomainTimeStamp();
        }

        [UsedImplicitly]
        protected University()
        {

        }

        public virtual Guid Id { get; protected set; }


        public virtual string Name { get; protected set; }

        /// <summary>
        /// Used as extra synonym to add to university search
        /// </summary>
        public virtual string Extra { get; set; }
        
        public virtual  string Country { get; protected set; }

        public virtual DomainTimeStamp RowDetail { get; protected set; }
    }
}
