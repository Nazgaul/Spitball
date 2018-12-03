using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhiberante proxy")]

    public class Document : IEvents
    {
        public Document([NotNull] string name, [NotNull] string blobName, [NotNull] University university,
            [NotNull] Course course, DocumentType type,
            [NotNull] IEnumerable<Tag> tags, User user, string professor)
        : this()
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name.Replace("+", string.Empty);
            BlobName = blobName ?? throw new ArgumentNullException(nameof(blobName));
            University = university ?? throw new ArgumentNullException(nameof(university));
            Course = course ?? throw new ArgumentNullException(nameof(course));
            Type = type;
            Tags = new HashSet<Tag>(tags);
            User = user;
            Views = 0;
            Professor = professor;
            Events.Add(new DocumentCreatedEvent(this));
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
            Tags = new HashSet<Tag>();
            Events = new List<IEvent>();
        }

        public virtual long Id { get; set; }
        public virtual string Name { get; set; }


        //public virtual RowDetail RowDetail { get; protected set; }
        public virtual string BlobName { get; protected set; }

        public virtual University University { get; set; }

        public virtual Course Course { get; set; }

        public virtual DocumentType Type { get; set; }

        public virtual ISet<Tag> Tags { get; set; }

        public virtual DomainTimeStamp TimeStamp { get; set; }

        public virtual User User { get; set; }


        public virtual string Professor { get; set; }

        public virtual int Views { get; set; }
        public virtual int Downloads { get; set; }
        public virtual int Purchased { get; set; }
        public virtual int? PageCount { get; set; }
        public virtual long? OldId { get; set; }

        public virtual ItemState? State { get; set; }


        public virtual CultureInfo Language { get; set; }
        public virtual IList<IEvent> Events { get; }
    }
}