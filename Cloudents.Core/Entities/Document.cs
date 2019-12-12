using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Exceptions;
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
            Course course,
            BaseUser user, decimal price, DocumentType documentType, string description)
      : this()
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name.Replace("+", "-");
            University = user.University;
            Course = course ?? throw new ArgumentNullException(nameof(course));
            User = user;
            Views = 0;
            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }
            ChangePrice(price);
            //Price = price;
            var status = Public;// GetInitState(user);
            if (status == Public)
            {
                MakePublic();
            }
            Status = status;
            DocumentType = documentType;
        }


        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
        }

        // public virtual long Id { get; set; }
        public virtual string Name { get; protected set; }

        public virtual University University { get; protected set; }

        public virtual Course Course { get; protected set; }

        public virtual string Description { get; protected set; }


        public virtual DomainTimeStamp TimeStamp { get; protected set; }

        public virtual BaseUser User { get; protected set; }



        public virtual int Views { get; protected set; }
        public virtual int Downloads { get; protected set; }

        //this is only for document
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

        public virtual void UnDelete()
        {
            Status = ItemStatus.Public;
            AddEvent(new DocumentUndeletedEvent(this));
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

        public virtual DocumentType? DocumentType { get; set; }

        //This is only for video
        public virtual TimeSpan? Duration { get; set; }
        public virtual bool IsShownHomePage { get; protected set; }
    }
}