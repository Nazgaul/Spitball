﻿using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Exceptions;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static Cloudents.Core.Entities.ItemStatus;


namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhiberante proxy")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate Proxy")]
    public class Document : Entity<long>, IAggregateRoot, ISoftDelete
    {

        public const int MinLength = 4;
        public const int MaxLength = 150;

        public Document(string name,
            Course course,
            User user, decimal price, DocumentType documentType, string? description)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Course = course ?? throw new ArgumentNullException(nameof(course));
            User = user ?? throw new ArgumentNullException(nameof(user));
            if (user.Tutor == null)
            {
                throw new UnauthorizedAccessException("Only tutor can upload files");
            }

            TimeStamp = new DomainTimeStamp();
            DocumentDownloads = new HashSet<UserDownloadDocument>();

            Name = Path.GetFileNameWithoutExtension(name.Replace("+", "-"));

            Views = 0;
            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }
            ChangePrice(price);
            Status = GetInitState(user);
            if (Status == Public)
            {
                MakePublic();
            }
            DocumentType = documentType;
      
        }

      

        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
            DocumentDownloads ??= new HashSet<UserDownloadDocument>();
        }

        // public virtual long Id { get; set; }
        public virtual string Name { get; protected set; }


        public virtual Course Course { get; protected set; }

        public virtual string? Description { get; protected set; }


        public virtual DomainTimeStamp TimeStamp { get; protected set; }

        public virtual BaseUser User { get; protected set; }



        public virtual int Views { get; protected set; }
        public virtual int Downloads { get; protected set; }

        //this is only for document
        public virtual int? PageCount { get; set; }
        //public virtual long? OldId { get; protected set; }

        public virtual string? MetaContent { get; set; }

        public virtual decimal Price { get; protected set; }
        // ReSharper disable once CollectionNeverUpdated.Local Resharper
        private readonly IList<Transaction> _transactions = new List<Transaction>();
        public virtual IEnumerable<Transaction> Transactions => _transactions;

        public virtual ItemStatus Status { get; protected set; }

        private readonly ICollection<Vote> _votes = new List<Vote>();
        public virtual IEnumerable<Vote> Votes => _votes;

        public virtual int VoteCount { get; protected set; }

        protected internal virtual ISet<UserDownloadDocument> DocumentDownloads { get; set; }

        public virtual short Boost { get; set; }

        public virtual void AddDownload(User user)
        {
            if (!User.Equals(user))
            {
                var download = new UserDownloadDocument(user, this);
                DocumentDownloads.Add(download);
            }
        }


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
            DocumentDownloads.Clear();
            AddEvent(new DocumentDeletedEvent(this));
        }

        public virtual void Flag(string messageFlagReason, BaseUser user)
        {
            if (User == user)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }
            Status = Status.Flag(messageFlagReason, user);
            AddEvent(new DocumentFlaggedEvent(this));
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
            AddEvent(new DocumentPriceChangeEvent(this));

        }

        public virtual void Rename(string name)
        {
            Name = Path.GetFileNameWithoutExtension(name);
        }

        public virtual void PurchaseDocument(User buyer)
        {
            var t = buyer.Transactions.TransactionsReadOnly.AsQueryable().Where(w => w is DocumentTransaction)
                .Any(f => ((DocumentTransaction)f).Document.Id == Id);

            if (t)
            {
                throw new DuplicateRowException();
            }

            buyer.MakeTransaction(DocumentTransaction.Buyer(this));
            User.MakeTransaction(DocumentTransaction.Seller(this));
            User.MakeTransaction(new CommissionTransaction(Price));
        }

        public virtual DocumentType? DocumentType { get; set; }

        //This is only for video
        public virtual TimeSpan? Duration { get; set; }
        public virtual bool IsShownHomePage { get; protected set; }

        public virtual string? Md5 { get; set; }
    }
}