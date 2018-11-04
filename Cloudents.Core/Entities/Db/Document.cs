using System;
using Cloudents.Core.Enum;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities.Db
{
    public class Document
    {
        public Document([NotNull] string name, [NotNull] string blobName, [NotNull] University university,
            [NotNull] Course course, DocumentType type,
            [NotNull] IEnumerable<Tag> tags, User user, string professor)
        : this()
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            BlobName = blobName ?? throw new ArgumentNullException(nameof(blobName));
            University = university ?? throw new ArgumentNullException(nameof(university));
            Course = course ?? throw new ArgumentNullException(nameof(course));
            Type = type;
            Tags = new HashSet<Tag>(tags);
            User = user;
            Views = 0;
            Professor = professor;
        }

        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
            //Courses = new HashSet<Course>();
            Tags = new HashSet<Tag>();
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
        public virtual int Purchased { get; set; }
        public virtual int? PageCount { get; set; }

        public virtual CultureInfo Language { get; protected set; }
    }
}