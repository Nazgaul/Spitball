using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Cloudents.Core.Entities.ItemState2;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhiberante proxy")]

    public class Document : AggregateRoot, ISoftDelete
    {
        public Document(string name,
            University university,
            Course course, DocumentType type,
            IEnumerable<Tag> tags, User user, string professor, decimal price)
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

            Price = price;
            State = GetInitState(user);
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
            Tags = new HashSet<Tag>();
        }

       // public virtual long Id { get; set; }
        public virtual string Name { get; set; }


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

        public virtual decimal Price { get; set; }
        public virtual IList<Transaction> Transactions { get; set; }
        public virtual ItemState2 State { get; protected set; }

        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get; set; }

        //public override void ChangeState(ItemState state)
        //{
        //    //Item.ChangeState(state);
        //}

        public virtual void MakePublic()
        {
            State = Public();
                AddEvent(new DocumentCreatedEvent(this));
        }

        public virtual void Delete()
        {
            Votes.Clear();
            AddEvent(new DocumentDeletedEvent(this));
        }

        public virtual void Flag(string messageFlagReason, User user)
        {
            State = State.Flag(messageFlagReason, user);
        }
    }
}