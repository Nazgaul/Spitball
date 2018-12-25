using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhiberante proxy")]

    public class Document : ItemObject
    {
        public Document(string name,
            University university,
            Course course, DocumentType type,
            IEnumerable<Tag> tags, User user, string professor)
        : this()
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name.Replace("+", "-");
            University = university ?? throw new ArgumentNullException(nameof(university));
            Course = course ?? throw new ArgumentNullException(nameof(course));
            Type = type;
            Tags = new HashSet<Tag>(tags);
            User = user;
            Views = 0;
            Professor = professor;

            ChangeState(Privileges.GetItemState(user.Score));


        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
            Tags = new HashSet<Tag>();
            Item = new ItemComponent();
        }

        public virtual long Id { get; set; }
        public virtual string Name { get; set; }


        //public virtual RowDetail RowDetail { get; protected set; }
        //  public virtual string BlobName { get; protected set; }

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


        public virtual string MetaContent { get; set; }
        public override void DeleteAssociation()
        {
            Item.Votes.Clear();
        }

        public override void ChangeState(ItemState state)
        {
            //Item.ChangeState(state);
        }
    }
}