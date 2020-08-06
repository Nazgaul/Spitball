using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Cloudents.Core.Entities.ItemStatus;


namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhiberante proxy")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate Proxy")]
    public class Document : Entity<long>, IAggregateRoot, ISoftDelete
    {

        //public const int MinLength = 4;
        public const int MaxLength = 150;

        public Document(string name,
            Course course,
           
            DocumentType documentType, bool visible)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Course = course ?? throw new ArgumentNullException(nameof(course));
            User = course.Tutor.User;
            TimeStamp = new DomainTimeStamp();
            DocumentDownloads = new HashSet<UserDownloadDocument>();
            Name = Path.GetFileNameWithoutExtension(name.Replace("+", "-"));
            Views = 0;
            Status = visible ? Public : Pending;
            DocumentType = documentType;
            AddEvent(new DocumentCreatedEvent(this));
          
        }



        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected Document()
        {
            TimeStamp = new DomainTimeStamp();
            DocumentDownloads ??= new HashSet<UserDownloadDocument>();
        }

        public virtual string Name { get; protected set; }

        public virtual Course Course { get; set; }


        public virtual DomainTimeStamp TimeStamp { get; protected set; }

        public virtual User User { get; protected set; }



        public virtual int Views { get; protected set; }
        public virtual int Downloads { get; protected set; }

        //this is only for document
        public virtual int? PageCount { get; set; }

        public virtual string? MetaContent { get; set; }



        //public virtual 

        //[Obsolete]
        //// ReSharper disable once CollectionNeverUpdated.Local 
        //private readonly IList<Transaction> _transactions = new List<Transaction>();
        //[Obsolete]
        //public virtual IEnumerable<Transaction> Transactions => _transactions;

        public virtual ItemStatus Status { get;  set; }



       // public virtual int? PurchaseCount { get; protected set; }

        protected internal virtual ISet<UserDownloadDocument> DocumentDownloads { get; set; }

        public virtual void AddDownload(User user)
        {
            if (!User.Equals(user))
            {
                var download = new UserDownloadDocument(user, this);
                DocumentDownloads.Add(download);
            }
        }

        //public virtual void MakePublic()
        //{
        //    if (Status != Pending) return;
        //    Status = Public;
        //    AddEvent(new DocumentCreatedEvent(this));
        //}

        public virtual void Delete()
        {
            Status = ItemStatus.Delete();
            DocumentDownloads.Clear();
            AddEvent(new DocumentDeletedEvent(this));
        }

       

        //public virtual void UnFlag()
        //{
        //    if (Status != Flagged) return;
        //    Status = Public;
        //}

       
        public virtual void Rename(string name)
        {
            Name = Path.GetFileNameWithoutExtension(name);
        }

        public virtual DocumentType DocumentType { get; set; }

        //This is only for video
        public virtual TimeSpan? Duration { get; set; }

        public virtual string? Md5 { get; set; }
        public virtual int? Position { get; set; }
    }
}