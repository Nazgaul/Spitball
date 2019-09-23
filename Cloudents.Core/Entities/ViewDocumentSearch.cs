using System;

namespace Cloudents.Core.Entities
{
    public class ViewDocumentSearch
    {
        protected ViewDocumentSearch()
        {

        }

        public virtual long Id { get; set; }
        public virtual string University { get; set; }
        public virtual string Course { get; set; }
        public virtual string Snippet { get; set; }
        public virtual string Professor { get; set; }
        public virtual string Type { get; set; }
        public virtual string Title { get; set; }
        public virtual long UserId { get; set; }
        public virtual string UserName { get; set; }
        public virtual int UserScore { get; set; }
        public virtual string UserImage { get; set; }
        public virtual int Views { get; set; }
        public virtual int Downloads { get; set; }
        public virtual DateTime? DateTime { get; set; }
        public virtual int Votes { get; set; }
        public virtual decimal Price { get; set; }
        public virtual Guid UniversityId { get; set; }
        public virtual string Country { get; set; }
        public virtual bool IsTutor { get; set; }
        public virtual int Purchased { get; set; }

        public virtual TimeSpan Duration  { get; set; }
        public virtual DocumentType DocumentType  { get; set; }
    }
}
