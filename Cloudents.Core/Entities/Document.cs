﻿using Cloudents.Core.Event;
using Cloudents.Core.Exceptions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static Cloudents.Core.Entities.ItemStatus;


namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhiberante proxy")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]

    public class Document : Entity<long>, IAggregateRoot, ISoftDelete
    {
        public Document(string name,
            University university,
            Course course, [NotNull] string type,
            IEnumerable<Tag> tags, BaseUser user, string professor, decimal price)
        : this()
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name.Replace("+", "-");
            University = university ?? throw new ArgumentNullException(nameof(university));
            Course = course ?? throw new ArgumentNullException(nameof(course));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Tags = new HashSet<Tag>(tags);
            User = user;
            Views = 0;
            Professor = professor;

            ChangePrice(price);
            //Price = price;
            var status = Public;// GetInitState(user);
            if (status == Public)
            {
                MakePublic();
            }
            Status = status;
        }

        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
            Tags = new HashSet<Tag>();
        }

        // public virtual long Id { get; set; }
        public virtual string Name { get; set; }

        public virtual University University { get; set; }

        public virtual Course Course { get; set; }

        public virtual string Type { get; protected set; }

        public virtual ISet<Tag> Tags { get; protected set; }

        public virtual DomainTimeStamp TimeStamp { get; protected set; }

        public virtual BaseUser User { get; protected set; }


        public virtual string Professor { get; protected set; }

        public virtual int Views { get; protected set; }
        public virtual int Downloads { get; protected set; }
        public virtual int Purchased { get; protected set; }
        public virtual int? PageCount { get; set; }
        public virtual long? OldId { get; protected set; }

        public virtual string MetaContent { get; set; }

        public virtual decimal Price { get; protected set; }
        // ReSharper disable once CollectionNeverUpdated.Local Resharper
        private readonly IList<Transaction> _transactions = new List<Transaction>();
        public virtual IReadOnlyCollection<Transaction> Transactions => _transactions.ToList();

        public virtual ItemStatus Status { get; protected set; }

        private readonly ICollection<Vote> _votes = new List<Vote>();
        public virtual IEnumerable<Vote> Votes => _votes;

        public virtual int VoteCount { get; protected set; }

        public virtual void Vote(VoteType type, User user)
        {
            if (type == VoteType.Down)
            {
                throw new NotSupportedException();
            }
            if (Status != Public)
            {
                throw new NotFoundException();
            }

           

            var vote = Votes.AsQueryable().FirstOrDefault(w => w.User == user);
            if (vote == null)
            {
                vote = new Vote(user, this, type);
                _votes.Add(vote);
            }
            vote.VoteType = type;
            VoteCount = Votes.Sum(s => (int)s.VoteType);
            //if (VoteCount < VoteCountToFlag)
            //{
            //    Status = Status.Flag(TooManyVotesReason, user);
            //}
        }

        public virtual void MakePublic()
        {
            if (Status != Pending) return;
            Status = Public;
            AddEvent(new DocumentCreatedEvent(this));
        }

        public virtual void Delete()
        {
            Status = ItemStatus.Delete();
            _votes.Clear();
            AddEvent(new DocumentDeletedEvent(this));
        }

        public virtual void Flag(string messageFlagReason, BaseUser user)
        {
            if (User == user)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }
            Status = Status.Flag(messageFlagReason, user);
        }

        public virtual void UnFlag()
        {
            if (Status != Flagged) return;
            if (Status.FlagReason?.Equals(TooManyVotesReason, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                _votes.Clear();
                VoteCount = 0;
            }
            Status = Public;
        }

        public const decimal PriceLimit = 1000M;

        public virtual void ChangePrice(decimal newPrice)
        {
            if (Price == newPrice)
            {
                return;
            }

            if (newPrice > PriceLimit)
            {
                throw new ArgumentOutOfRangeException();
            }

            Price = decimal.Round(newPrice, 2);
            TimeStamp.UpdateTime = DateTime.UtcNow;

        }
    }
}