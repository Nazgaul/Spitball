using Cloudents.Core.Enum;
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
            Tutor tutor,
            //decimal price,
            DocumentType documentType,
            string? description
            //PriceType priceType
            )
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (course == null) throw new ArgumentNullException(nameof(course));
            if (tutor == null) throw new ArgumentNullException(nameof(tutor));
            Course = course ?? throw new ArgumentNullException(nameof(course));
            User = tutor.User;
            TimeStamp = new DomainTimeStamp();
            DocumentDownloads = new HashSet<UserDownloadDocument>();
            Name = Path.GetFileNameWithoutExtension(name.Replace("+", "-"));
            Views = 0;
            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }
            Status = GetInitState(tutor.User);
            DocumentType = documentType;
            //DocumentPrice = new DocumentPrice(0, PriceType.Free, tutor);
            AddEvent(new DocumentCreatedEvent(this));
          
        }



        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
            DocumentDownloads ??= new HashSet<UserDownloadDocument>();
        }

        // public virtual long Id { get; set; }
        public virtual string Name { get; protected set; }


        


        public virtual Course Course { get; set; }

        

        public virtual string? Description { get; protected set; }


        public virtual DomainTimeStamp TimeStamp { get; protected set; }

        public virtual BaseUser User { get; protected set; }



        public virtual int Views { get; protected set; }
        public virtual int Downloads { get; protected set; }

        //this is only for document
        public virtual int? PageCount { get; set; }

        public virtual string? MetaContent { get; set; }



        //public virtual 

        // ReSharper disable once CollectionNeverUpdated.Local 
        private readonly IList<Transaction> _transactions = new List<Transaction>();
        public virtual IEnumerable<Transaction> Transactions => _transactions;

        public virtual ItemStatus Status { get; protected set; }



       // public virtual int? PurchaseCount { get; protected set; }

        protected internal virtual ISet<UserDownloadDocument> DocumentDownloads { get; set; }

        [Obsolete]
        public virtual DocumentPrice DocumentPrice { get; protected set; }

        public virtual void AddDownload(User user)
        {
            if (!User.Equals(user))
            {
                var download = new UserDownloadDocument(user, this);
                DocumentDownloads.Add(download);
            }
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
            //_votes.Clear();
            DocumentDownloads.Clear();
            AddEvent(new DocumentDeletedEvent(this));
        }

       

        public virtual void UnFlag()
        {
            if (Status != Flagged) return;
            Status = Public;
        }

       
        public virtual void Rename(string name)
        {
            Name = Path.GetFileNameWithoutExtension(name);
        }

        //public virtual void PurchaseDocument(User buyer)
        //{
        //    var t = buyer.Transactions.TransactionsReadOnly.AsQueryable().Where(w => w is DocumentTransaction)
        //        .Any(f => ((DocumentTransaction)f).Document.Id == Id);

        //    if (t)
        //    {
        //        throw new DuplicateRowException();
        //    }


        //    SyncPurchaseCount();
        //    PurchaseCount++;
        //    buyer.MakeTransaction(DocumentTransaction.Buyer(this));
        //    User.MakeTransaction(DocumentTransaction.Seller(this));
        //    User.MakeTransaction(new CommissionTransaction(DocumentPrice.Price));
        //}

        //public virtual void SyncPurchaseCount()
        //{
        //    PurchaseCount = _transactions.Count / 2;
        //}

        public virtual DocumentType DocumentType { get; set; }

        //This is only for video
        public virtual TimeSpan? Duration { get; set; }
       // public virtual bool? IsShownHomePage { get; protected set; }

        public virtual string? Md5 { get; set; }


        //public virtual void ChangeToSubscribeMode(Tutor tutor)
        //{
        //    DocumentPrice = new DocumentPrice(0, PriceType.Subscriber, tutor);
        //}
    }

    public class DocumentPrice
    {
       

        protected DocumentPrice()
        {

        }

        private decimal _price;

        public virtual decimal Price
        {
            get => _price;
            protected set
            {
                //if (value > PriceLimit || value < 0)
                //{
                //    throw new ArgumentOutOfRangeException();
                //}
                _price = decimal.Round(value, 2);
            }
        }

        public PriceType? Type { get; protected set; }



        
    }
}